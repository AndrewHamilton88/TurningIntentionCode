using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ParamicsSNMP2007;
using System.Timers;
using System.IO;
using ParamicsSNMPcontrol;
using ParamincsSNMPcontrol;

namespace ParamicsSNMPcontrol
{
    public class RunTwinProgram
    {
        string IP;
        int Port;
        string ConfigFile;
        double
            Distance, A1, B1, F1, F2, G1, H1, G2, H2;
        Strategies Strat;

        public RunTwinProgram() 
        {
            //****Editable Section of the File************
            IP = "152.78.97.176";
            Port = 2525;
            ConfigFile = "JunctionDesignTwinNew.xml";
            Distance = 55;
            A1 = 0.95;
            B1 = 0.52;
            F1 = 0.52;
            F2 = 0.48;
            G1 = 1;
            G2 = 0.83;
            
            //Comment the relevant line below:
            Strat = new MultiHighBid(Distance, A1,B1,F1,F2,G1,G2);//Coordinated
            //Strat = new HighBid();//Uncoordinated
            //**********************************************

            Run();
        }

        public void Run()
        {         
            Coordinate TestC = new Coordinate(ConfigFile, Strat, IP, Port);
            TestC.ConnectToParamics();
            Runner Run = new Runner(TestC);
            Run.SynchRun(89);
        }

    }
}
