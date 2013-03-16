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
using System.Text;
using System.Drawing;
using System.IO;

namespace MultiClassBlueNoise
{
    /// <summary>
    /// Uniform sampling implementation.
    /// This is a naive approach similar to single-class Dart Throwing where
    /// only the distance between samples is considered. The class for each
    /// sample generated is given by a random probability following the underlying
    /// pixel's color density.
    /// </summary>
    class Uniform : Stippling
    {
        int numClasses;
        public override int NumClasses { get { return this.stipplingData.colorBasis.Length; } }

        private int imageWidth, imageHeight;
        public override int OutputImageWidth { get { return imageWidth; } }
        public override int OutputImageHeight { get { return imageHeight; } }

        double[,] r = null;
        public override double MinDistance(int x, int y, int classi, int classj) { return r[x, y]; }

        private int PickSampleColor(int x, int y, Random r)
        {
            Color col = resizedSource.GetPixel(x, y);
            float[] distance = new float[stipplingData.colorBasis.Length];
            for (int c = 0; c < stipplingData.colorBasis.Length; ++c)
            {
                double component;

                double dR = (((double)stipplingData.colorBasis[c].R / 255) - ((double)col.R / 255));
                double dG = (((double)stipplingData.colorBasis[c].G / 255) - ((double)col.G / 255));
                double dB = (((double)stipplingData.colorBasis[c].B / 255) - ((double)col.B / 255));
                component = Math.Max(0.0001, Math.Sqrt(dR * dR + dG * dG + dB * dB));
                distance[c] = (float)component;
            }
            float total = 0;
            float M = distance.Sum();
            for (int c = 0; c < distance.Length; ++c)
            {
                float d = M / distance[c];
                distance[c] = total + d;
                total += d;
            }
            double rnd = r.NextDouble() * total;
            for (int c = 0; c < distance.Length; ++c)
                if (rnd <= distance[c] ) return c;
            return distance.Length - 1;
        }

        public override void StartStippling(System.ComponentModel.BackgroundWorker worker)
        {
            InitSampleHash();

            Random rnd = new Random();

            if (r == null || imageWidth == 0 || imageHeight == 0) PrepareStippling(stipplingData.settings.Spacing, worker);
          
            //
            int numSamples = stipplingData.samples.Count; // in case we're resuming stippling
            do
            {
                //  pick next sample class

                int numAttempts = 0;
                Sample_t s;
                do
                {
                    if (worker.WorkerSupportsCancellation && worker.CancellationPending) return;

                    if (cropRegion.HasValue)
                    {
                        double w = rnd.NextDouble() * cropRegion.Value.Width + cropRegion.Value.X;
                        double h = rnd.NextDouble() * cropRegion.Value.Height + cropRegion.Value.Y;

                        s.x = w;
                        s.y = h;
                    }
                    else
                    {
                        double cellX = (double)rnd.Next(0, HashCells) / HashCells;
                        double cellY = (double)rnd.Next(0, HashCells) / HashCells;
                        double w = rnd.NextDouble() / HashCells;
                        double h = rnd.NextDouble() / HashCells;

                        s.x = cellX + w;
                        s.y = cellY + h;
                    }


                    int sx = (int)(s.x * OutputImageWidth);
                    int sy = (int)(s.y * OutputImageHeight);

                    s.c = PickSampleColor(sx, sy, rnd);
                    numAttempts++;
                } while (numAttempts < stipplingData.settings.Retries && !AcceptSample(s));
                if (numAttempts < stipplingData.settings.Retries)
                {
                    AddSample(s);
                } // otherwise the sample is considered lost

                if (worker != null && worker.WorkerReportsProgress &&
                    numSamples % stipplingData.settings.UpdateFrequency == 0)
                {
                    worker.ReportProgress((int)(100 * (float)numSamples / stipplingData.settings.TotalSamples), String.Format("Stippling... {0} samples placed", stipplingData.samples.Count));
                }

            } while (numSamples++ < stipplingData.settings.TotalSamples);
        }

        private bool AcceptSample(Sample_t s)
        {
            int sx = (int)(s.x * imageWidth);
            int sy = (int)(s.y * imageHeight);
            double radius = MinDistance(sx, sy, s.c, s.c);

            List<Sample_t> nearby = NearbySamples(s.x, s.y, radius);
            return nearby.Count == 0;
        }

        private Bitmap resizedSource = null;
        public override bool PrepareStippling(double spacing, System.ComponentModel.BackgroundWorker worker)
        {
            if (stipplingData.settings.SourceFile.Length == 0 ||
               !File.Exists(stipplingData.settings.SourceFile))
            {
                return false;
            }

            Bitmap source = new Bitmap(stipplingData.settings.SourceFile);

            numClasses = stipplingData.colorBasis.Length;

            double[] rDiag = new double[numClasses];
            resizedSource = new Bitmap((int)((float)Math.Max(1, source.Width * stipplingData.settings.ImageResizingFactor)),
                                       (int)((float)Math.Max(1, source.Height * stipplingData.settings.ImageResizingFactor)));
            {
                Graphics g = Graphics.FromImage(resizedSource);
                g.DrawImage(source, new Rectangle(0, 0, resizedSource.Width, resizedSource.Height));
            }
            imageWidth = resizedSource.Width;
            imageHeight = resizedSource.Height;
            r = new double[imageWidth, imageHeight];
            double pixelSize = 1.0 / imageWidth;

            for (int y = 0; y < imageHeight; ++y)
            {
                if (worker.WorkerSupportsCancellation && worker.CancellationPending) return false;
                if (worker.WorkerReportsProgress) worker.ReportProgress((int)((float)y / imageHeight * 100), "Building conflict check matrix");

                for (int x = 0; x < imageWidth; ++x)
                {
                    Color col = resizedSource.GetPixel(x, y);
                    double component;

                    if (stipplingData.additiveBlending)
                    {
                        double dR = (double)col.R / 255;
                        double dG = (double)col.G / 255;
                        double dB = (double)col.B / 255;
                        component = 1.0 / Math.Max(0.0001, Math.Sqrt(dR * dR + dG * dG + dB * dB));
                    }
                    else
                    {
                        double dR = (double)col.R / 255;
                        double dG = (double)col.G / 255;
                        double dB = (double)col.B / 255;
                        double dist = Math.Sqrt(dR * dR + dG * dG + dB * dB);
                        component = 1.0 / Math.Max(0.0001, Math.Sqrt(3) - dist);
                    }
                    r[x, y] = component * pixelSize * spacing;
                }
            }
            return true;
        }
        
        public override void Clear()
        {
            stipplingData.samples.Clear();
            r = null;
            imageWidth = 0;
            imageHeight = 0;
        }
    }
}
