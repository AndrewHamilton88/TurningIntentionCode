using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParamicsPuppetMaster;

namespace ParamincsSNMPcontrol
{
    class ReadTurningIntention
    {
        public string NetName = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\TurningDirections - Simple Crossroad";

        /// <summary>
        /// This  returns the link number being turned onto. (i.e. moving from link 1 0 to link 0 3
        /// would return '3'.
        /// </summary>
        /// <returns></returns>
        public string NextLinkNumber(string ApproachNode, string JunctionNode, string TurningNumber)
        {
            string Returner = "";
            using (StreamReader ReadFile = new StreamReader(NetName))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("Link " + ApproachNode + " " + JunctionNode + ": "))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        Returner = SplitString[3 + Convert.ToInt16(TurningNumber)];
                    }
                }
            }
            return Returner;
        }
    }
}
