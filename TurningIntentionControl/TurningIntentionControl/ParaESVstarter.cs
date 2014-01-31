using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Reflection;
using System.Diagnostics;


namespace ParamincsSNMPcontrol
{
    public abstract class ParaStarter
    {
        //*Class members
        string ModelPath;
        protected string ControlCommand;

        //Class constructor
        public ParaStarter(string MP)
        {
            ModelPath = MP;
            ControlCommand = "";
        }

        public void LauncParamics()
        {
            ControlCommand += ModelPath;
            ControlCommand += "\"";
            FileInfo ABatchFile = new FileInfo("RunParaModel.bat");
            StreamWriter ABatchWrite = ABatchFile.CreateText();
            ABatchWrite.WriteLine(ControlCommand);
            ABatchWrite.Close();

            Thread.Sleep(1000);
            Process BatchP = new Process();
            BatchP.StartInfo.FileName = "RunParaModel.bat";
            BatchP.Start();
            Thread.Sleep(25000);
        }

    }
    class ParaESVstarter : ParaStarter
    {

        //Class constructor
        public ParaESVstarter(string MP)
            : base(MP)
        {
            ControlCommand = "C:\\\"Program Files\"\\\"Paramics 2000\"\\paramics2007.1\\bin\\ParaEsv.exe -network \"";
        }

    }
    class ParaBSMstarter : ParaStarter
    {

        //Class constructor
        public ParaBSMstarter(string MP)
            : base(MP)
        {
            ControlCommand = "C:\\\"Program Files\"\\\"Paramics 2000\"\\paramics2007.1\\bin\\ParaBSM.exe -paused -network \"";
        }

    }
}