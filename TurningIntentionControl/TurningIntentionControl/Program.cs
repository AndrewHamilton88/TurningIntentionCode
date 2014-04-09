using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ParamicsSNMP2007;
using System.Timers;
using System.IO;
using ParamicsSNMPcontrol;

namespace ParamincsSNMPcontrol
{
    class Program
    {
        public bool Stopper = false;
        
        static void Main(string[] args)
        {
            
            string IP = "152.78.97.129";
            int port = 2525;
            
            HighBidTurn ST1 = new HighBidTurn();
            
            Coordinate TestC;
            //coordinateSIT TestC;

            if (args.Length == 0)
            {
                //string[] fnames = new string[] { "TrainedTriJ1H7.csv", "TrainedTriJ2H7.csv", "TrainedTriJ3H7.csv" };
                //string[] sigNs = new string[] { "1", "0", "2" };

                //TestC = new coordinateSIT("JunctionDesignTriC.XML", ST1, IP, port); // <--- Simon used this one 05/11/12
                //TestC = new Coordinate("JunctionDesignMillbrook.XML", ST1, IP, port); //<--- Simon used this one 20/11/12
                //TestC = new Coordinate("JunctionDesignSimpleCrossroads.XML", ST1, IP, port); //Andrew's attempt on Simple Crossroads 20/11/12
                TestC = new Coordinate("JunctionDesignSimpleCrossroads3Lane.XML", ST1, IP, port); //Andrew's Simple Crossroads - 3 lane 02/09/13
                //TestC = new coordinateSIT("JunctionDesignSimpleCrossroads3Lane.XML", ST1, IP, port); //Andrew's Simple Crossroads - 3 lane 04/12/13
                //TestC = new Coordinate("JunctionDesignSimpleCrossroads2LaneStraightBoth.XML", ST1, IP, port); //Andrew's Simple Crossroads - 2 lane - Both Straight Ahead 04/12/13
                //TestC = new Coordinate("JunctionDesignSimpleCrossroads2LaneStraightLeft.XML", ST1, IP, port); //Andrew's Simple Crossroads - 2 lane - Straight and Left Lane Together, Dedicated Right 04/12/13
                //TestC = new Coordinate("JunctionDesignSimpleCrossroads2LaneStraightRight.XML", ST1, IP, port); //Andrew's Simple Crossroads - 2 lane - Straight and Right Lane Together, Dedicated Left 04/12/13

                ParamicsPuppetMaster.EditConfig ECG = new ParamicsPuppetMaster.EditConfig(TestC.ParamicsPath);
                ECG.SetDemandRate(100);

                //ParamicsPuppetMaster.EditDemands EDM = new ParamicsPuppetMaster.EditDemands(TestC.ParamicsPath, A.Demands);
            }
            else
            {
                TestC = new Coordinate("JunctionDesignOrigV2flat.xml", ST1, IP, port);
            }

            try
            {
                ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
                //ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
                StartParamicsModel.LauncParamics();


                TestC.ConnectToParamics();
                Runner Run = new Runner(TestC);

                Run.SynchRun(1439);

            }
            catch (Exception e)
            {
                StreamWriter SW;
                SW = File.AppendText(@"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\C# Code\ParamicsSNMPcontrolV3\BigRunLog.txt");
                SW.WriteLine("G = {0:G}", DateTime.Now);
                foreach (string s in args)
                {
                    SW.WriteLine(s);
                }
                SW.WriteLine(e.Message);
                SW.WriteLine("*******************************");
                SW.WriteLine("");
                SW.WriteLine("");
                SW.Close();
            }

            
        }
        /*public static void runHighRdDemo()
        {
            string IP = "152.78.99.101";
            int port = 2525;

            MultiHighBid ST1 = new MultiHighBid();
            coordinateSIT TestC = new coordinateSIT("JunctionDesignHighRdV5wiHi.XML", ST1, IP, port);

            ParamicsPuppetMaster.EditConfig ECG = new ParamicsPuppetMaster.EditConfig(TestC.ParamicsPath);
            ECG.SetDemandRate(60);

            try
            {
                ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
                //ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
                StartParamicsModel.LauncParamics();


                TestC.ConnectToParamics();
                Runner Run = new Runner(TestC);

                Run.SynchRun(359);

            }
            catch (Exception e)
            {
                StreamWriter SW;
                SW = File.AppendText(@"C:\Documents and Settings\User\My Documents\Research\Code\Visual Studio 2005\Projects\ParamicsSNMPcontrolV3\BigRunLog.txt");
                SW.WriteLine("G = {0:G}", DateTime.Now);

                SW.WriteLine(e.Message);
                SW.WriteLine("*******************************");
                SW.WriteLine("");
                SW.WriteLine("");
                SW.Close();
            }


        }*/

        /*public static void runMillbrookDemo()
        {
            string IP = "152.78.99.101";
            int port = 2525;

            HighBid ST1 = new HighBid();

            coordinateSIT TestC = new coordinateSIT("JunctionDesignMillbrook.XML", ST1, IP, port);
            ParamicsPuppetMaster.EditConfig ECG = new ParamicsPuppetMaster.EditConfig(TestC.ParamicsPath);
            ECG.SetDemandRate(55);

            try
            {
                ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
                //ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
                StartParamicsModel.LauncParamics();


                TestC.ConnectToParamics();
                Runner Run = new Runner(TestC);

                Run.SynchRun(359);

            }
            catch (Exception e)
            {
                StreamWriter SW;
                SW = File.AppendText(@"C:\Documents and Settings\User\My Documents\Research\Code\Visual Studio 2005\Projects\ParamicsSNMPcontrolV3\BigRunLog.txt");
                SW.WriteLine("G = {0:G}", DateTime.Now);

                SW.WriteLine(e.Message);
                SW.WriteLine("*******************************");
                SW.WriteLine("");
                SW.WriteLine("");
                SW.Close();
            }
        } */  
    }
}
