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
    class Program
    {
        static void Main(string[] args)
        {
            /*Process.Start("../../RunParamics.bat");
            Console.WriteLine("Press return to continue:");
            Console.Read();*/


            string NetName = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads";

            ParamicsPuppetMaster.EditNodes EN = new ParamicsPuppetMaster.EditNodes(NetName);
            ReadTurningIntention RTI = new ReadTurningIntention();

            double dist =  EN.NodeList[4].DistToNode(EN.NodeList[3]);
            //Console.WriteLine(dist);
            double X = EN.NodeList[1].X;
            double Y = EN.NodeList[1].Y;
            string ResultTurningIntention;
            ResultTurningIntention = RTI.NextLinkNumber("4", "0", "2");

            string ResultStageNumber;
            ResultStageNumber = RTI.StageRequired("3", "4");
            //string ResultNextTurningIntention = RTI.NextNextLinkNumber(ResultTurningIntention, "0");

            Console.WriteLine(X);
            Console.WriteLine(Y);
            Console.WriteLine(ResultTurningIntention);
            Console.WriteLine(ResultStageNumber);
            //Console.WriteLine(ResultNextTurningIntention);
            Console.Read();
        }

        


    }
}
