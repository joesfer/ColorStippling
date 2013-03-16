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
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace MultiClassBlueNoise
{
    public class HardDiskSampling : Stippling
    {
       
        private void BuildRMatrix(double[] rDiag, int x, int y)
        {
            int c = rDiag.Length; // c = num classes
            for (int i = 0; i < c; ++i)
            {
                r[x, y, i, i] = rDiag[i];
            }
            // Sort the c classes into priority groups P with decreasing Ri
            List<Tuple<double, List<int>>> priorityGroups = new List<Tuple<double, List<int>>>();
            for (int i = 0; i < c; ++i)
            {
                bool found = false;
                List<Tuple<double, List<int>>>.Enumerator e = priorityGroups.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Item1 == rDiag[i])
                    {
                        e.Current.Item2.Add(i);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    List<int> classList = new List<int>();
                    classList.Add(i);
                    Tuple<double, List<int>> pi = new Tuple<double, List<int>>(rDiag[i], classList);
                    priorityGroups.Add(pi);
                }
            }
            SortedDictionary<double, List<int>> P = new SortedDictionary<double, List<int>>();
            {
                List<Tuple<double, List<int>>>.Enumerator e = priorityGroups.GetEnumerator();
                while (e.MoveNext())
                {
                    P.Add(-e.Current.Item1, e.Current.Item2); // negate the r key to reverse the sorting
                }
            }

            HashSet<int> C = new HashSet<int>();
            double D = 0;
            for (int k = 0; k < P.Count; ++k)
            {
                List<int> Pk = P.ElementAt(k).Value;
                foreach (int c_i in Pk)
                {
                    C.Add(c_i);
                    D += 1.0 / (rDiag[c_i] * rDiag[c_i]);
                }

                foreach (int c_i in Pk)
                {
                    foreach (int c_j in C)
                    {
                        if (c_i != c_j)
                        {
                            r[x, y, c_i, c_j] = r[x, y, c_j, c_i] = 1.0 / Math.Sqrt(D);
                        }
                    }
                }
            }
        }

        double[, , ,] r = null;
        private double[, , ,] RadiusMatrix { get { return r; } }
        public override double MinDistance(int x, int y, int classi, int classj) { return RadiusMatrix[x, y, classi, classj]; }

        int numClasses;
        public override int NumClasses { get { return this.stipplingData.colorBasis.Length; } }

        private int imageWidth, imageHeight;
        public override int OutputImageWidth { get { return imageWidth; } }
        public override int OutputImageHeight { get { return imageHeight; } }      

        public override void StartStippling(BackgroundWorker worker)
        {          
            InitSampleHash();

            Random rnd = new Random();

            if (RadiusMatrix == null || imageWidth == 0 || imageHeight == 0) PrepareStippling(stipplingData.settings.Spacing, worker);

            int[] Ni = new int[NumClasses]; // target num samples per class
            {
                for (int i = 0; i < NumClasses; ++i) Ni[i] = (int)((float)stipplingData.settings.TotalSamples / NumClasses);
            }

            //
            int[] ni = new int[NumClasses];
            for (int i = 0; i < NumClasses; ++i) ni[i] = 0;

            float[] fillRate = new float[NumClasses];

            double[] diag = new double[NumClasses];

            do
            {
                //  pick next sample class

                for (int i = 0; i < NumClasses; ++i)
                {
                    fillRate[i] = (float)ni[i] / Ni[i];
                }

                int c = 0;
                float m = fillRate.Min();
                for (; c < NumClasses; ++c) if (fillRate[c] == m) break;


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

                    s.c = c;
                    numAttempts++;
                } while (numAttempts < stipplingData.settings.Retries && !AcceptSample(s));
                if (numAttempts < stipplingData.settings.Retries)
                {
                    AddSample(s);
                    ni[s.c]++;
                }
                else
                {
                    int sx = (int)(s.x * OutputImageWidth);
                    int sy = (int)(s.y * OutputImageHeight);
                    if (RadiusMatrix[sx, sy, s.c, s.c] > 0.1) continue; // the conflicting radius affects too many samples

                    List<Sample_t> conflict = GetConflictSamples(s);
                    if (Removable(conflict, s, fillRate))
                    {
                        foreach (Sample_t s_ in conflict)
                        {
                            RemoveSample(s_);
                            ni[s_.c]--;
                        }
                        AddSample(s);
                        ni[s.c]++;
                    }
                }

                if (worker != null && worker.WorkerReportsProgress &&
                    stipplingData.samples.Count % stipplingData.settings.UpdateFrequency == 0)
                {
                    worker.ReportProgress((int)(100 * (float)stipplingData.samples.Count / stipplingData.settings.TotalSamples), String.Format("Stippling... {0} samples placed", stipplingData.samples.Count));
                }

            } while (stipplingData.samples.Count < stipplingData.settings.TotalSamples);

        }

        public override bool PrepareStippling(double spacing, BackgroundWorker worker)
        {
            if (stipplingData.settings.SourceFile.Length == 0 ||
                !File.Exists(stipplingData.settings.SourceFile))
            {
                return false;
            }

            Bitmap source = new Bitmap(stipplingData.settings.SourceFile);

            numClasses = stipplingData.colorBasis.Length;

            double[] rDiag = new double[numClasses];
            Bitmap resizedSource = new Bitmap((int)((float)Math.Max(1, source.Width * stipplingData.settings.ImageResizingFactor)),
                                       (int)((float)Math.Max(1, source.Height * stipplingData.settings.ImageResizingFactor)));
            {
                Graphics g = Graphics.FromImage(resizedSource);
                g.DrawImage(source, new Rectangle(0, 0, resizedSource.Width, resizedSource.Height));
            }
            imageWidth = resizedSource.Width;
            imageHeight = resizedSource.Height;
            r = new double[imageWidth, imageHeight, numClasses, numClasses];
            double pixelSize = 1.0 / imageWidth;

            for (int y = 0; y < imageHeight; ++y)
            {
                if (worker.WorkerSupportsCancellation && worker.CancellationPending) return false;
                if (worker.WorkerReportsProgress) worker.ReportProgress((int)((float)y / imageHeight * 100), "Building conflict check matrix");

                for (int x = 0; x < imageWidth; ++x)
                {
                    Color col = resizedSource.GetPixel(x, y);
                    for (int c = 0; c < stipplingData.colorBasis.Length; ++c)
                    {
                        double component;

                        if (stipplingData.additiveBlending)
                        {
                            double dR = (((double)stipplingData.colorBasis[c].R / 255) * ((double)col.R / 255));
                            double dG = (((double)stipplingData.colorBasis[c].G / 255) * ((double)col.G / 255));
                            double dB = (((double)stipplingData.colorBasis[c].B / 255) * ((double)col.B / 255));
                            component = 1.0 / Math.Max(double.Epsilon, Math.Sqrt(dR * dR + dG * dG + dB * dB));
                        }
                        else
                        {
                            double dR = ((1.0 - (double)stipplingData.colorBasis[c].R / 255) * ((double)col.R / 255));
                            double dG = ((1.0 - (double)stipplingData.colorBasis[c].G / 255) * ((double)col.G / 255));
                            double dB = ((1.0 - (double)stipplingData.colorBasis[c].B / 255) * ((double)col.B / 255));
                            double dist = Math.Sqrt(dR * dR + dG * dG + dB * dB);
                            component = 1.0 / Math.Max(double.Epsilon, 1.0 - dist);
                        }

                        rDiag[c] = component * pixelSize * spacing;
                    }

                    BuildRMatrix(rDiag, x, y);
                }
            }
            return true;
        }


        private bool AcceptSample(Sample_t s)
        {
            double radius = 0;
            for (int c = 0; c < numClasses; ++c) radius = Math.Max(radius, r[(int)(s.x * imageWidth), (int)(s.y * imageHeight), s.c, c]);

            int sx = (int)(s.x * imageWidth);
            int sy = (int)(s.y * imageHeight);

            List<Sample_t> nearby = NearbySamples(s.x, s.y, radius);
            foreach (Sample_t s_ in nearby)
            {
                double dx = s_.x - s.x;
                double dy = s_.y - s.y;
                double d = Math.Sqrt(dx * dx + dy * dy);
                if (d <= r[sx, sy, s.c, s_.c]) return false;
            }
            return true;
        }

       

        private bool Removable(List<Sample_t> conflict, Sample_t s, float[] fillRate)
        {
            int sx = (int)(s.x * imageWidth);
            int sy = (int)(s.y * imageHeight);
            foreach (Sample_t s_ in conflict)
            {
                int sx_ = (int)(s_.x * imageWidth);
                int sy_ = (int)(s_.y * imageHeight);
                if (r[sx_, sy_, s_.c, s_.c] >= r[sx, sy, s.c, s.c] ||
                     fillRate[s_.c] < fillRate[s.c]) return false;
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
