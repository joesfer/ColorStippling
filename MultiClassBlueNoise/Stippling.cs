/* 
	================================================================================
	Copyright (c) 2013, Jose Esteve. http://www.joesfer.com
	All rights reserved. 

	Redistribution and use in source and binary forms, with or without modification, 
	are permitted provided that the following conditions are met: 

	* Redistributions of source code must retain the above copyright notice, this 
	  list of conditions and the following disclaimer. 
	
	* Redistributions in binary form must reproduce the above copyright notice, 
	  this list of conditions and the following disclaimer in the documentation 
	  and/or other materials provided with the distribution. 
	
	* Neither the name of the organization nor the names of its contributors may 
	  be used to endorse or promote products derived from this software without 
	  specific prior written permission. 

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
	ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
	WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
	DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR 
	ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
	(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
	LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
	ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
	(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
	SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
	================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Threading;
using System.IO;

namespace MultiClassBlueNoise
{
    /// <summary>
    /// Base class from which the stippling methods derive.
    /// It wraps common functionality.
    /// </summary>
    public abstract class Stippling
    {
        public class StipplingSettings_t
        {
            [DisplayName("Source File"), CategoryAttribute("Source"), DescriptionAttribute("Input image file to stipple"),
             EditorAttribute(typeof(LoadImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string SourceFile { get; set; }
            [DisplayName("Image Resizing Factor"), CategoryAttribute("Source"), DescriptionAttribute("Factor of enlargements of the output image. Stippling points will be further apart than the source pixels, so you may need a larger output canvas to represent the same perceived color density.")]
            public float ImageResizingFactor { get; set; }
            [DisplayName("Total Samples"), CategoryAttribute("Stippling"), DescriptionAttribute("Total number of stippling samples to place.")]
            public int TotalSamples { get; set; }
            [DisplayName("Spacing"), CategoryAttribute("Stippling"), DescriptionAttribute("Distance between samples.")]
            public float Spacing { get; set; }
            [DisplayName("Retries"), CategoryAttribute("Stippling"), DescriptionAttribute("Number of attempts for random placing of each new sample before we give up and force it by removing existing samples.")]
            public int Retries { get; set; }
            [DisplayName("Output Image File"), CategoryAttribute("Output"), DescriptionAttribute("Path to the output image file."),
             EditorAttribute(typeof(SaveImageFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string OutputImageFile { get; set; }
            [DisplayName("Output Data File"), CategoryAttribute("Output"), DescriptionAttribute("Path to the output data file. This file contains an XML description of the stippling result which could be displayed with other applications"),
            EditorAttribute(typeof(SaveXMLFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
            public string OutputDataFile { get; set; }
            [DisplayName("Update Frequency"), CategoryAttribute("Progress"), DescriptionAttribute("How often we update the preview image, in number of samples placed.")]
            public int UpdateFrequency { get; set; }

            public float dotSizeMultiplier;

            public StipplingSettings_t()
            {
                TotalSamples = 1000000;
                ImageResizingFactor = 1.5f;
                UpdateFrequency = 500;
                Spacing = 1.0f;
                Retries = 30;
                SourceFile = "";
                string prefix = DateTime.Now.ToFileTime() + "_";
                OutputDataFile = prefix + "stipplingData.xml";
                OutputImageFile = prefix + "stippling.png";
                dotSizeMultiplier = 1.0f;
            }

        }

        public struct Sample_t
        {
            public double x, y;
            public int c; // class
        };



        public class StipplingData_t
        {
            public virtual Color[] colorBasis { get { return _colorBase; } set { _colorBase = value; } } // we need to override it in the serializer...
            public bool additiveBlending { get { return _additiveBlending; } set { _additiveBlending = value; } }
            public List<Sample_t> samples { get { return _samples; } set { _samples = value; } }
            public StipplingSettings_t settings { get { return _settings; } set { _settings = value; } }

            public StipplingData_t()
            {
                colorBasis = null;
                additiveBlending = true;
                samples = new List<Sample_t>();
                settings = new StipplingSettings_t();
            }

            private Color[] _colorBase;
            private bool _additiveBlending;
            private List<Sample_t> _samples;
            private StipplingSettings_t _settings;
        }

        public abstract void StartStippling( BackgroundWorker worker);
        public abstract bool PrepareStippling(double spacing, BackgroundWorker worker);
        public abstract double MinDistance(int x, int y, int classi, int classj);
        public abstract void Clear();
        public StipplingData_t stipplingData = new StipplingData_t();

        internal static int HashCells { get { return 100; } }
        List<Sample_t>[,] sampleHash = null;
        internal void InitSampleHash()
        {
            sampleHash = new List<Sample_t>[HashCells, HashCells];
            for (int i = 0; i < HashCells; ++i)
                for (int j = 0; j < HashCells; ++j)
                    sampleHash[i, j] = new List<Sample_t>();
        }

        internal void AddSample(Sample_t s)
        {
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);
            lock (stipplingData) // prevent the collection from being modified while we may be baking a preview
            {
                stipplingData.samples.Add(s);
            }
            sampleHash[hashX, hashY].Add(s);
        }
        internal void RemoveSample(Sample_t s)
        {
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);
            lock (stipplingData) // prevent the collection from being modified while we may be baking a preview
            {
                stipplingData.samples.Remove(s);
            }
            sampleHash[hashX, hashY].Remove(s);
        }

        internal List<Sample_t> NearbySamples(double x, double y, double radius)
        {
            List<Sample_t> nearby = new List<Sample_t>();
            double cellSize = 1.0 / HashCells;
            int cellSpan = (int)Math.Ceiling(radius / cellSize);
            int hashX = (int)(x * HashCells);
            int hashY = (int)(y * HashCells);
            double radius2 = radius * radius;
            for (int j = Math.Max(0, hashY - cellSpan); j <= Math.Min(HashCells - 1, hashY + cellSize); ++j)
                for (int i = Math.Max(0, hashX - cellSpan); i <= Math.Min(HashCells - 1, hashX + cellSize); ++i)
                    foreach (Sample_t s in sampleHash[i, j])
                    {
                        double dx = x - s.x;
                        double dy = y - s.y;
                        double d = dx * dx + dy * dy;
                        if (d <= radius2) nearby.Add(s);
                    }

            return nearby;
        }

        internal List<Sample_t> GetConflictSamples(Sample_t s)
        {
            List<Sample_t> conflict = new List<Sample_t>();
            double radius = 0;
            for (int c = 0; c < NumClasses; ++c) radius = Math.Max(radius, MinDistance((int)(s.x * OutputImageWidth), (int)(s.y * OutputImageHeight), s.c, c));
            double cellSize = 1.0 / HashCells;
            int cellSpan = (int)Math.Ceiling(radius / cellSize);
            int hashX = (int)(s.x * HashCells);
            int hashY = (int)(s.y * HashCells);


            for (int j = Math.Max(0, hashY - cellSpan); j <= Math.Min(HashCells - 1, hashY + cellSpan); ++j)
                for (int i = Math.Max(0, hashX - cellSpan); i <= Math.Min(HashCells - 1, hashX + cellSpan); ++i)
                    foreach (Sample_t s_ in sampleHash[i, j])
                    {
                        double dx = s.x - s_.x;
                        double dy = s.y - s_.y;
                        double d = Math.Sqrt(dx * dx + dy * dy);

                        double radiusS = MinDistance((int)(s.x * OutputImageWidth), (int)(s.y * OutputImageHeight), s.c, s_.c);
                        double radiusS_ = MinDistance((int)(s_.x * OutputImageWidth), (int)(s_.y * OutputImageHeight), s_.c, s.c);

                        if (d <= (radiusS + radiusS_) * 0.5) conflict.Add(s_);
                    }
            return conflict;
        }
        public RectangleF? cropRegion = null;
        public virtual int OutputImageWidth { get { return 0; } }
        public virtual int OutputImageHeight { get { return 0; } }
        public virtual int NumClasses { get { return 0; } }
    };

}
