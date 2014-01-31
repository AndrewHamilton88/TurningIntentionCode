using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ParamincsSNMPcontrol;

namespace ParamicsSNMPcontrol
{
    class RWmatrix
    {
        ForwardPropTD FPN;
        string W1space,
            W1comma,
            W2space,
            W2comma,
            FileName;
        //*Class Constuctor
        public RWmatrix(ForwardPropTD FP,string FN)
        {
            FileName = FN;
            FPN = FP;
            W1space = FPN.W1.ToString();
            W1comma = W1space.Replace(' ', ',');
            W1comma = W1comma.Replace(",\r", " \r");

            W2space = FPN.W2.ToString();
            W2comma = W2space.Replace(' ', ',');
            W2comma = W2comma.Replace(",\r", " \r");
        }

        public void WriteLog(double[] Probs, int NextStage, string Path)
        {
            StreamWriter SW;
            SW = File.AppendText(Path + @"\Log\WeightsLog.txt");
            SW.WriteLine("G = {0:G}", DateTime.Now);
            SW.WriteLine("*******************************");
            string probline = "Probabilities = ";
            foreach (double d in Probs)
            {
                probline += d.ToString();
                probline += ", ";
            }
            //SW.WriteLine("Probabilities = " + Probs[0].ToString() + ", " + Probs[1].ToString() + ", " + Probs[2].ToString());
            SW.WriteLine(probline);
            SW.WriteLine("Next Stage = " + NextStage.ToString());
            SW.WriteLine("Feedback = " + FPN.U.ToString());
            SW.WriteLine("*******************************");
            SW.WriteLine(W1comma);
            SW.WriteLine("*******************************");
            SW.WriteLine("");
            SW.WriteLine(W2comma);
            SW.WriteLine("*******************************");
            SW.WriteLine("");
            SW.WriteLine("");
            SW.Close();
        }
        public void ReWriteWfile()
        {
            StreamWriter SW;
            SW = File.CreateText(FileName);
            SW.WriteLine(W1comma);
            SW.WriteLine("nextmatrix");
            SW.WriteLine(W2comma);
            SW.Close();
        }
    }
}
