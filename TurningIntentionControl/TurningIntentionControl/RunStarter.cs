using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ParamincsSNMPcontrol
{
    public abstract class RunStarter
    {
        public int RunIts;
        public int port;
        public string IP;
        public Runner Run;
        public Coordinate TestC;

        public RunStarter(string ipIn, int prtIn)
        {
            IP = ipIn; port = prtIn;
        }

        public void StartRunningESV()
        {
            ParaESVstarter StartParamicsModel = new ParaESVstarter(TestC.ParamicsPath);
            StartRunning(StartParamicsModel);

        }
        public void StartRunningBSM()
        {
            ParaBSMstarter StartParamicsModel = new ParaBSMstarter(TestC.ParamicsPath);
            StartRunning(StartParamicsModel);
        }
        private void StartRunning(ParaStarter PSV)
        {
            PSV.LauncParamics();
            TestC.ConnectToParamics();
            Run = new Runner(TestC);
            Run.SynchRun(RunIts);
        }

        public void SetDemand(int dPercent)
        {
            ParamicsPuppetMaster.EditConfig ECG = new ParamicsPuppetMaster.EditConfig(TestC.ParamicsPath);
            ECG.SetDemandRate(dPercent);
        }

        public void SetDMatrix(Mapack.Matrix Mat)
        {
            ParamicsPuppetMaster.EditDemands EDM = new ParamicsPuppetMaster.EditDemands(TestC.ParamicsPath, Mat);
        }

    }

    public class StartHighBid : RunStarter
    {
        public StartHighBid(string ipIn, int prtIn, FileInfo ConfigFile)
            : base(ipIn, prtIn)
        {
            HighBid St1 = new HighBid();
            TestC = new Coordinate(ConfigFile.FullName, St1, IP, port);
        }
    }

    public class StartNeural : RunStarter
    {
        public StartNeural(string ipIn, int prtIn, FileInfo[] ParamFiles, FileInfo ConfigFile, string[] Nodes)
            : base(ipIn, prtIn)
        {
            string[] fnames = new string[ParamFiles.Length];
            for (int i = 0; i < ParamFiles.Length; i++)
            {
                fnames[i] = ParamFiles[i].FullName;
            }

            NeuralNet Strat1 = new NeuralNet(fnames, Nodes);
            TestC = new Coordinate(ConfigFile.FullName, Strat1, IP, port);
        }
    }

    public class StartTD : RunStarter
    {
        public int StartLearnTrigg;
        public double alpha, gamma, lambda, eta;
        public bool Q;
        public string[] pFnames;
        public string[] snodes;
        public string confname;



        public StartTD(string ipIn, int prtIn, FileInfo[] ParamFiles, FileInfo ConfigFile, string[] Nodes)
            : base(ipIn, prtIn)
        {
            //Default values for coefficients
            alpha = 0.1;
            gamma = 1.0;
            lambda = 0.1;
            eta = 0.1;
            Q = true;
            StartLearnTrigg = 3000;

            string[] fnames = new string[ParamFiles.Length];
            for (int i = 0; i < ParamFiles.Length; i++)
            {
                fnames[i] = ParamFiles[i].FullName;
            }
            pFnames = fnames;
            confname = ConfigFile.FullName;
            snodes = Nodes;


            SetConfig();
        }

        public void SetConfig()
        {
            TempDiff ST1 = new TempDiff(pFnames, snodes, StartLearnTrigg, alpha, gamma, lambda, eta, Q);
            TestC = new Coordinate(confname, ST1, IP, port);
        }
        public void SetConfig(bool independent)
        {
            TempDiff ST1 = new TempDiff(pFnames, snodes, StartLearnTrigg, alpha, gamma, lambda, eta, Q);
            ST1.independent = independent;
            TestC = new Coordinate(confname, ST1, IP, port);
        }
    }

    public class StartHuman : RunStarter
    {
        public StartHuman(string ipIn, int prtIn, FileInfo ConfigFile)
            : base(ipIn, prtIn)
        {
            Trainer St1 = new Trainer();
            TestC = new Coordinate(ConfigFile.FullName, St1, IP, port);
        }

    }
}
