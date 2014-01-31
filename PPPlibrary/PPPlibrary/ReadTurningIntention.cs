using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamicsPuppetMaster
{
    public class ReadTurningIntention
    {
        //This is the simple crossroads data files...
        /*public string LinkTurningFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads\VIVAtest"; //This is the clockwise link order
        public string LinkFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads\links";
        public string PrioritiesFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads\priorities";
        public string PrioritiesStages = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads\priorities_file"; //This is the required stage*/

        //This is the simple crossroads 3 lanes data files...
        public string LinkTurningFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads - 3 Lane Approach with Sensors\3LaneApproachMovements"; //This is the clockwise link order
        public string LinkFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads - 3 Lane Approach with Sensors\links";
        public string PrioritiesFile = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads - 3 Lane Approach with Sensors\priorities";
        public string PrioritiesStages = @"C:\Documents and Settings\Siemens\Desktop\Andrew's Work\Paramics Models\Simple Crossroads\Simple Crossroads\priorities_file"; //This is the required stage


        /// <summary>
        /// This  returns the link number being turned onto. (i.e. moving from link 1 0 to link 0 3
        /// would return '3'.
        /// </summary>
        /// <returns></returns>
        public string NextLinkNumber(string ApproachNode, string JunctionNode, string TurningNumber)
        {
            string Returner = "";
            string WithoutCommaReturner = "";
            using (StreamReader ReadFile = new StreamReader(LinkTurningFile))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("Link " + ApproachNode + " " + JunctionNode + ": "))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        Returner = SplitString[3 + Convert.ToInt16(TurningNumber)];
                        WithoutCommaReturner = Returner.Substring(0, Returner.Length - 1);
                        return WithoutCommaReturner;
                        //break;
                    }
                }
            }
            /*using (StreamReader ReadFile = new StreamReader(LinkFile))
            {
                string FileLine2;
                while ((FileLine2 = ReadFile.ReadLine()) != null)
                {
                    if (FileLine2.Contains("link " + JunctionNode + " " + WithoutCommaReturner))
                    {
                        return WithoutCommaReturner;
                    }
                }
            }*/
            return "null";
        }

        public string StageRequired(string ApproachNode, string NextLinkNumber)
        {
            string Returner = "";
            using (StreamReader ReadFile = new StreamReader(PrioritiesStages))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("link " + ApproachNode + " to " + NextLinkNumber + ": "))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        Returner = SplitString[4];
                        return Returner;
                    }
                }
            }
            return "null";
        }

        /// <summary>
        /// This  returns the direction they are turning. (i.e. 'Left'.)
        /// </summary>
        /// <returns></returns>
        public string NextTurnDirection(string ApproachNode, string JunctionNode, string TurningNumber)
        {
            string Returner = "";
            string WithoutCommaReturner = "";
            using (StreamReader ReadFile = new StreamReader(LinkTurningFile))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("Link " + ApproachNode + " " + JunctionNode + ": "))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        if (SplitString[4] == "None,")
                        {
                            return "None";
                        }
                        if (SplitString[5] == "Left," || SplitString[5] == "Straight," || SplitString[5] == "Right,")
                        {
                            Returner = SplitString[5 + Convert.ToInt16(TurningNumber)];
                            WithoutCommaReturner = Returner.Substring(0, Returner.Length - 1);
                            return WithoutCommaReturner;
                        }
                        else
                        {
                            Returner = SplitString[6 + Convert.ToInt16(TurningNumber)];
                            WithoutCommaReturner = Returner.Substring(0, Returner.Length - 1);
                            return WithoutCommaReturner;
                        }
                    }
                }
            }
            return "null";
        }



        /*public string NextNextJunctionNode(string ApproachNode, string TurningNumber)
        {
            string JunctionNode = "";
            using (StreamReader ReadFile = new StreamReader(NetName))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("Link " + ApproachNode))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        JunctionNode = SplitString[2];
                        return JunctionNode;
                    }
                }
            }
            return JunctionNode;
        }

        public string NextNextLinkNumber(string ApproachNode, string TurningNumber)
        {
            string Returner = "";
            using (StreamReader ReadFile = new StreamReader(NetName))
            {
                string FileLine;
                while ((FileLine = ReadFile.ReadLine()) != null)
                {
                    if (FileLine.Contains("Link " + ApproachNode))
                    {
                        string[] SplitString = FileLine.Split(' ');
                        Returner = SplitString[3 + Convert.ToInt16(TurningNumber)];
                        string WithoutCommaReturner = Returner.Substring(0, Returner.Length - 1);
                        return WithoutCommaReturner;
                    }
                }
            }
            return Returner;
        }*/
    }
}

