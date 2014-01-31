using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace ParamicsPuppetMaster
{
    class RunParamicsNetwork 
    {
        //**Class objects

        //Network Path
        public string Path;
        static string ExecutePath=@"C:\Program Files\Paramics 2000\paramics2007.1\bin";

        //**Class constructor
        public RunParamicsNetwork(string pn)
        {
            Path = pn;
        }

        //**Class Functions

        //Run a NEW simulation of the network
        public void RunNetwork()
        {
            string RunCommand=String.Concat("cd ",'"',ExecutePath,'"',"\n","ParaBsm.exe -network ",'"',Path,'"');

            FileInfo BatchFile = new FileInfo("RunParamics.bat");
            StreamWriter BatchWrite = BatchFile.CreateText();
            BatchWrite.WriteLine(RunCommand);
            BatchWrite.Close();

            Process BatchP = new Process();
            BatchP.StartInfo.FileName = "RunParamics.bat";
            BatchP.Start();
            BatchP.WaitForExit();
            
         
        }

        //Run a simulation of the network from the saved snapshot
        public void RunNetworkSnap()
        {
            string RunCommand = String.Concat("cd ", '"', ExecutePath, '"', "\n", "ParaBsm.exe -Snapshot -network ", '"', Path, '"');

            FileInfo BatchFile = new FileInfo("RunParamics.bat");
            StreamWriter BatchWrite = BatchFile.CreateText();
            BatchWrite.WriteLine(RunCommand);
            BatchWrite.Close();

            Process BatchP = new Process();
            BatchP.StartInfo.FileName = "RunParamics.bat";
            BatchP.Start();
            BatchP.WaitForExit();
        }
    }
}
