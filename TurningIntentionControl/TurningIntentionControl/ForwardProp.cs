using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mapack;

namespace ParamincsSNMPcontrol
{
    public abstract class ForwardProp
    {
        //*class members
        public Mapack.Matrix W1;
        public FileStream FS;

        //*class constructor
        public ForwardProp(string fp)
        {
            try{
                FS = new FileStream(fp,FileMode.Open);
            }
            catch(FileNotFoundException e){
                Console.WriteLine(e.Message);
            }
        }

        //*function 
       public  void CSVtoMAT(ref StreamReader SR, ref Mapack.Matrix W)
        {
            string line = " ";
            List<string[]> LineList = new List<string[]>();

            while (!SR.EndOfStream && !line.Contains("nextmatrix"))
            {
                line = SR.ReadLine();
                if (line.Contains(","))
                {
                    LineList.Add(line.Split(','));
                }
            }

            int Row = LineList.Count;
            int Col = LineList[0].Length;

            W = new Matrix(Row, Col);

            int R = 0;
            int C = 0;

            foreach (string[] ln in LineList)
            {
                foreach (string cell in ln)
                {
                    W[R,C] = Double.Parse(cell);
                    C++;
                }
                R++;
                C = 0;
            }
        }

        public abstract double[] propagate(double[] Bids);
       

    }

    public class ForwardPropLogit : ForwardProp
    {
        //*class constructor
        public ForwardPropLogit(string fp)
            : base(fp)
        {
            StreamReader ReadW = new StreamReader(FS);
            CSVtoMAT(ref ReadW, ref W1);
            FS.Close();
        }

        //function
        public override double[] propagate(double[] Bids)
        {
            Mapack.Matrix BidM = new Matrix(new double[][] { Bids });

            //sanity check
            try{
                if (Bids.Length!=W1.Rows)
                {
                    throw new Exception("The number of bids and weights does not match"); 
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


            Mapack.Matrix ProtoProbs = BidM * W1;
            double SigmaSum = 0;
            for (int i = 0; i < ProtoProbs.Columns; i++)
            {
                double d = Math.Exp(ProtoProbs[0,i]);
                ProtoProbs[0,i]=d;
                SigmaSum+=d;
            }
            double[] Probabilities = new double[ProtoProbs.Columns];
            for (int i = 0; i < ProtoProbs.Columns; i++)
            {
                Probabilities[i] = ProtoProbs[0,i] / (1 + SigmaSum);
            }
            return (Probabilities);
        }
    }

    public class ForwardPropNeural : ForwardProp
    {
        //class members
        public Mapack.Matrix W2;
        

        //class Constructor
        public ForwardPropNeural(string fp)
            : base(fp)
        {
            StreamReader ReadW = new StreamReader(FS);
            CSVtoMAT(ref ReadW, ref W1);
            CSVtoMAT(ref ReadW, ref W2);
            FS.Close();
        }

        //function
        public override double[] propagate(double[] Bids)
        {
            Mapack.Matrix BidM = new Matrix(new double[][] { Bids });

            //sanity check
            try
            {
                if (Bids.Length != W1.Rows)
                {
                    throw new Exception("The number of bids and weights does not match");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            int H = W1.Columns;
            int M = W1.Rows;
            int K = W2.Columns;

            Matrix a1 = W1.Transpose() * BidM.Transpose();
            Matrix Z = new Matrix(H, 1);

            for (int h = 0; h < H; h++)
            {
                double d = a1[h, 0];
                Z[h, 0] = Math.Tanh(d);
            }
            Matrix a2 = W2.Transpose() * Z;

            double Denominator = 0;
            for (int k = 0; k < K; k++)
            {
                double d = a2[k, 0];
                d = Math.Exp(d);
                Denominator += d;
            }

            double[] Probabilities = new double[K];
            for (int k = 0; k < K; k++)
            {
                double d = a2[k, 0];
                d = Math.Exp(d);
                Probabilities[k] = d / Denominator;
            }
            

            return (Probabilities);

        }
    }

    public class ForwardPropTD : ForwardPropNeural
    {
        //class members
        Matrix ET1t; 
        Matrix ET2t;
        Matrix Zt, Bt; 
        Matrix Ztp, Btp;
        public Matrix NablaY1t, NablaY2t;
        double[] Yt,Ytp;
        int Wint, Wintp;
        double MeanR;
        public double U;
        int Yums, H, M, K;
        public double alpha, gamma, lambda;
        public bool Qlearn;

        //class constructor
        public ForwardPropTD(string fp)
            : base(fp)
        {
            H = W1.Columns;
            M = W1.Rows;
            K = W2.Columns;
            Yums = 0;
            MeanR = 1;
            Wint = 1;
            Wintp = 1;


            ET1t = new Matrix(M, H, 0);
            ET2t = new Matrix(K, H, 0);
            ET2t = ET2t.Transpose();

            //default values for alpha gamma lambda
            alpha = 0.1;
            gamma = 1.0;
            lambda = 0.1;

            Qlearn = false;

            
        }

        //Functions
        public override double[] propagate(double[] Bids)
        {
            Mapack.Matrix BidM = new Matrix(new double[][] { Bids });

            //sanity check
            try
            {
                if (Bids.Length != W1.Rows)
                {
                    throw new Exception("The number of bids and weights does not match");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            int H = W1.Columns;
            int M = W1.Rows;
            int K = W2.Columns;

            Matrix a1 = W1.Transpose() * BidM.Transpose();
            Matrix Z = new Matrix(H, 1);

            for (int h = 0; h < H; h++)
            {
                double d = a1[h, 0];
                Z[h, 0] = Math.Tanh(d);
            }
            Matrix a2 = W2.Transpose() * Z;

            double Denominator = 0;
            for (int k = 0; k < K; k++)
            {
                double d = a2[k, 0];
                d = Math.Exp(d);
                Denominator += d;
            }

            double[] Probabilities = new double[K];
            for (int k = 0; k < K; k++)
            {
                double d = a2[k, 0];
                d = Math.Exp(d);
                Probabilities[k] = d / Denominator;
            }
            ///////Back up Data////////////////////////
            Zt = Ztp;
            Bt = Btp;
            Yt = Ytp;
            Ztp = Z;
            Btp = BidM;
            Ytp = Probabilities;


            return (Probabilities);
        }

        private void BackProp()
        {
            //Backpropagate to find gradients
            Matrix Y = new Matrix(K, 1);
            Matrix I = new Matrix(K, 1);

            for (int k=0; k < K; k++)
            {
                Y[k, 0] = Yt[k];
                if(k==(Wint-1))
                {
                    I[k,0] = 1;
                }
                else{
                    I[k,0] = 0;
                }
            }
            Matrix Yc = new Matrix(1, 1, Yt[Wint-1]);
            Matrix delta2 =  (I - Y)*Yc;

            Matrix delta1 = new Matrix(H, 1);
            Matrix d12 = W2*delta2;

            for (int h = 0; h < H; h++)
            {
                double z = Zt[h,0];
                delta1[h, 0] = (1 - Math.Pow(z, 2)) * d12[h, 0];
            }

            NablaY1t = delta1 * Bt;
            NablaY2t = delta2 * Zt.Transpose();
        }

        public void UpdateWeights(double R, int Vcount, int WS)
        {
            Wint = Wintp;
            Wintp = WS;
            BackProp();
            
            /*double alpha = 0.1;
            double gamma = 1;
            double lambda = 0.1;*/

            //code to select between pure Q learning and my skewed version //TODO make this less ad hoc or make a permanent choice.
            if (Qlearn)
            {
                double maxYtp = 0;
                foreach(double y in Ytp)
                {
                    if(y>maxYtp)
                    {
                        maxYtp = y;
                    }
                }
                //U = CalculateRewardMeanOverV(R,Vcount) + gamma * maxYtp - Yt[Wint - 1];
                U = CalculateRewardDerivative(R) + gamma * maxYtp - Yt[Wint - 1];
            }
            else
            {
                //U = CalculateRewardMeanOverV(R, Vcount) + gamma * Ytp[Wintp - 1] - Yt[Wint - 1];
                //U = CalculateRewardStraight(R) + 0.5 * CalculateRewardDerivative(R) + gamma * Ytp[Wintp - 1] - Yt[Wint - 1];

            }
            
            //double U = CalculateRewardMeanSutton(R);

            ET1t = ET1t * gamma * lambda + NablaY1t.Transpose();
            ET2t = ET2t * gamma * lambda + NablaY2t.Transpose();

            W1 = W1 + ET1t * alpha * U;
            W2 = W2 + ET2t * alpha * U;
  
        }
        private double CalculateRewardMean(double R)
        {     
            double Re = MeanR - R;
            if (Re > 1) { Re = 1; }
            else if (Re < -1) { Re = -1; }

            MeanR = (Yums * MeanR + R) / (Yums + 1);
            Yums++;

            return (Re);
        }

        private double CalculateRewardScaledMean(double R)
        {
            double Re = 1 - R / MeanR;

            MeanR = (Yums * MeanR + R) / (Yums + 1);
            Yums++;

            return (Re);
        }
        private double CalculateRewardMeanSutton(double R)
        {
            double rs = R / 59;
            double Re = MeanR - rs;

            double U = Re + Ytp[Wintp - 1] - Yt[Wint - 1];
            MeanR = MeanR - 0.1 * U;
            return (U);

        }
        private double CalculateRewardDerivative(double R)
        {
            double Re = (1 - (R / MeanR));
            if (R != 0)
            {
                MeanR = R;
            }
            else
            {
                MeanR = 1;
            }
            if (Re < -1) { Re = -1; }
            return (2*Re);//TODO factor of 2 is an experiment
        }

        private double CalculateRewardDeriv02(double R, int vc)
        {
            double Alt = R / vc /10;
            double dAlt = MeanR-Alt;
            if (dAlt > 2) { dAlt = 2; }
            else if (dAlt < -2) { dAlt = -2; }
            MeanR = Alt;
            return (dAlt);
        }

        private double CalculateRewardStraight(double R)
        {
            double Re = -R / 120;
            if (Re > 1)
            {
                Re = 1;
            }
            else if (Re < -1)
            {
                Re = -1;
            }
            return (Re);
        }
        private double CalculateRewardMeanOverV(double R,int Vcount)
        {
            double AvR;
            if (R == 0 || Vcount == 0)
            {
                AvR = 0;
            }
            else
            {
                AvR = -R / Vcount / 60;
            }
            if (AvR < -1) { AvR = -1; }

            return (AvR);
        }

    }
}
