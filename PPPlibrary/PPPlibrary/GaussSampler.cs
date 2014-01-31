using System;
using System.Collections.Generic;
using System.Text;

namespace ParamicsPuppetMaster
{
    public class GaussSampler
    {
        //* class members
        double Mean;
        double Sigma;
        Random FlatRan;

        //*Class constructor
        public GaussSampler(double m, double s)
        {
            Mean = m; Sigma = s;
            FlatRan = new Random();
        }

        public double RandomSample()
        {
            double x = FlatRan.NextDouble();
            double n = InvCumNorm(x);

            return (Mean + Sigma*n);

        }
        private double InvCumNorm(double x)
        {
            double a = 0.140012;//approximation from wikipedia!!

            double z = 2 * x - 1;
            double Lt = Math.Log(1 - Math.Pow(z, 2));
            double At = Math.PI * a;
            double T3 = (2 / At + Lt / 2);
            double T2a = Math.Pow((2 / At + Lt / 2), 2);
            double T2b = Lt / a;
            double T2 = Math.Sqrt(T2a - T2b);
            double T1 = Math.Sqrt(T2 - T3);
            double erfz = (z / Math.Abs(z)) * T1;

            return (Math.Sqrt(2) * erfz);
        }

    }
}
