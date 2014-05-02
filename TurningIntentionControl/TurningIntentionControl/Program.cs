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
            
            string IP = "152.78.97.184";
            int port = 2525;
            
            HighBidTurn ST1 = new HighBidTurn();
            
            Coordinate TestC;
            //coordinateSIT TestC;

            //This is to create a directory which records the MySQL results in a time and date folder
            String Todaysdate = DateTime.Now.ToString("dd-MM-yyyy-HH.mm");
            string ResultsDirectory = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads - 3 Lane Approach with Sensors\Results\" + Todaysdate;
            
            if(!Directory.Exists(ResultsDirectory))
            {
              Directory.CreateDirectory(ResultsDirectory); 
            }

            string ResultsDir = ResultsDirectory;
            ResultsDir = ResultsDirectory.Replace("'", "''");        //This is because MySQL uses an apostrophe (') as it's delimiter
            ResultsDir = ResultsDir.Replace("\\", "//");


            //How many times would you like the program to run
            int NumberOfRuns = 1;
            int Counter = 0;

            while (Counter < NumberOfRuns)
            {
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
                    //ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
                    ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
                    StartParamicsModel.LauncParamics();


                    TestC.ConnectToParamics();
                    Runner Run = new Runner(TestC);

                    Run.SynchRun(14400);

                    //AH added this function to save the biddata and linkturningmovements tables after a run

                    TestC.SaveTables(Counter, ResultsDir);
                    

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

                Counter++;
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
