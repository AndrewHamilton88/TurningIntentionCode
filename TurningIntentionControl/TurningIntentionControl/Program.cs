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
        public List<List<int>> AllPossibleStages;
        
        static void Main(string[] args)
        {
            /*AllStages AS = new AllStages();
            Console.WriteLine(AS.SimpleHighestStage(AS.StagesGenerator(), AS.PopulatePhases()));
            Console.Read();*/
           
            /*ReadTurningIntention RTI = new ReadTurningIntention(); -------->Andrew added on 03/12/12
            string test = RTI.NextLinkNumber("1", "0", "1");
            Console.WriteLine(test);
            Console.Read();*/
            

            //string NetName = @"C:\Documents and Settings\User\My Documents\Research\ParamicsModels\Ddrive\ParamicsFiles\EXAMPLE1 Validated_empty\EXAMPLE1 Validated_empty";

            //ParamicsPuppetMaster.EditNodes EN = new ParamicsPuppetMaster.EditNodes(NetName);

            //double dist =  EN.NodeList[4].DistToNode(EN.NodeList[3]);
            //double x = 0;
            //ParamicsPuppetMaster.ReadPointLoopFiles RPF = new ParamicsPuppetMaster.ReadPointLoopFiles(NetName, 6500, 10);

            //double[][] Input = new double[][] { new double[]{ 0, 2 }, new double[]{ 3, 0 } };
            //FFTTrunManager A = new FFTTrunManager(Input);
            
            //args = new string[] {"0", "0", "JunctionDesignOrigV2.xml", @"C:\Documents and Settings\User\My Documents\Research\Code\Visual Studio 2005\Projects\TestAutomatorV3\TestAutomator\bin\Debug\SimpleTTDtrainingParams\pFile502.csv", "1" };


            //args = new string[] { "TRAINLongWestH11A.csv", "TRAINLongEastH11A.csv" , "1", "0.05", "0.9", "0.2", "0.1", "20" };

            //RunTwinProgram RTP = new RunTwinProgram();

            /*///*****Standalone implementation for game******

            Strategies ST1;

            int StrategyIndex = Convert.ToInt32(args[0]);

            switch (StrategyIndex)
            {
                case 0:
                    ST1 = new Trainer();
                    break;
                case 1:
                    ST1 = new HighBid();
                    break;
                case 2:
                    ST1 = new MultiHighBid();
                    break;
                default:
                    ST1 = new HighBid();
                    break;
            }

            string IP = args[1];
            int port = Convert.ToInt32(args[2]);
            string Path = args[3];

            Coordinate TestC = new Coordinate(Path, ST1, IP, port);
            Runner Run = new Runner(TestC);
            Run.SynchRun();

            ///*********************************************/
            //string[] args1 ={ "0", "0", "JunctionDesignHighRdV3a.xml", "WsHrLongWestH9.csv;WsHrLongEastH9.csv", "193;202,206y,213y" };
            string IP = "152.78.97.129";
            //string IP = "169.254.25.129";
            int port = 2525;


            //StationaryVehicles ST1 = new StationaryVehicles();
            //Trainer ST1 = new Trainer();
            //MultiHighBid ST1 = new MultiHighBid();
            //MultiHighBidV2 ST1 = new MultiHighBidV2();

            //HighBid ST1 = new HighBid();
            
            HighBidTurn ST1 = new HighBidTurn();
            
            //Logit ST1;
            //NeuralNet ST1;
            Coordinate TestC;
            //coordinateSIT TestC;

            if (args.Length == 0)
            {
                //string[] fnames = new string[] { "WsHrLongWestH11.csv", "WsHrLongEastH11.csv" };
                //string[] fnames = new string[] { "JamesGameTrain.csv" };
                //string[] fnames = new string[] { "TRAINLongWestH11A150411.csv", "TRAINLongEastH11A150411.csv" };
                //string[] fnames = new string[] { "TRAINLongWestH11G01_ttest3.csv", "TRAINLongEastH11G01_ttest3.csv" };
                string[] fnames = new string[] { "TrainedTriJ1H7.csv", "TrainedTriJ2H7.csv", "TrainedTriJ3H7.csv" };
                string[] sigNs = new string[] { "1", "0", "2" };
                //string[] sigNs = new string[] { "193", "202,206y,213y" };
                //string[] fnames = new string[] { "WsStLongH7.csv" };
                //string[] fnames = new string[] { "TDinputAverageLifeHTrained080411.csv" };
                //string[] fnames = new string[] { "SandBoxTDinput.csv" };
                //string[] fnames = new string[] { "TDFullTrain240111.csv" };
                //string[] sigNs = new string[] { "1" };

                //ST1 = new TempDiff(fnames, sigNs, 3000, 0.1, 1, 0.1,0.1, true);
                //ST1 = new NeuralNet(fnames, sigNs);
                //ST1.independent = true;
                //TestC = new Coordinate("JunctionDesignOrigV2.xml", ST1, IP, port);
                //TestC = new Coordinate("JunctionDesignOrigV2flat.xml", ST1, IP, port);
                //TestC = new Coordinate("JunctionDesignTrib.XML", ST1, IP, port);
                //TestC = new coordinateSIT("JunctionDesignBabyModel.XML", ST1, IP, port);
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
                /*ArgumentsInterpreter ArgsI = new ArgumentsInterpreter(args);
                ST1 = ArgsI.BuildNeuralNet();*/   //AH blocked out 04/09/13
                //ST1 = ArgsI.BuildLogit();
                //TestC = ArgsI.BulidCoordinator(ST1, IP, port);
                //string[] fnames = new string[] { args[0],args[1] };
                //string[] sigNs = new string[] { "193", "202,206y,213y" };
                /*bool Q;
                if (Convert.ToInt32(args[2]) == 1)
                {
                    Q = true;
                }
                else
                {
                    Q = false;
                }*/
                //ST1 = new TempDiff(fnames, sigNs, 3000, Convert.ToDouble(args[3]), Convert.ToDouble(args[4]), Convert.ToDouble(args[5]), Convert.ToDouble(args[6]), Q);
                //ST1 = new NeuralNet(fnames, sigNs);
                //TestC = new coordinateSIT(ArgsI.ConfigFile, ST1, IP, port);
                TestC = new Coordinate("JunctionDesignOrigV2flat.xml", ST1, IP, port);
                //TestC = new coordinateSIT("JunctionDesignOrigV2flat.xml", ST1, IP, port);
                //TestC = new Coordinate("JunctionDesignHighRdV3aflat.XML", ST1, IP, port);
                //ParamicsPuppetMaster.EditConfig ECG = new ParamicsPuppetMaster.EditConfig(TestC.ParamicsPath);
                //ECG.SetDemandRate(Convert.ToInt32(args[7]));*/
            }

            try
            {
                //ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
                ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
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
        public static void runHighRdDemo()
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


        }

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
