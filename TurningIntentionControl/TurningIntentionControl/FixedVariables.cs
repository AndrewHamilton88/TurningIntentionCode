using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class FixedVariables
    {
        public int StartingSeeds = 20;
        public int StepsClimbed = 50;
        public int MutationsAroundAPoint = 100;
        public int IntergreenTimeVariable = 5;
        
        public static int NumberOfStages = 4;
        public static int MinimumGreenTime = 7;
        public static int IntergreenTime = 5;
        public static int MaximumGreenTime = 25;
        public static int IntergreenStageNumber = 99;
        public static int MaxCycleTime = 120;

        public double[] Stage1 = { 2, 2, 3, 1 };
        public double[] Stage2 = { 3, 2, 3, 1 };
        public double[] Stage3 = { 5, 3, 4, 3 };
        public double[] Stage4 = { 8, 1, 3, 1 };
        //double [] will contain [0] = queue length, [1] = arrival rate, [2] = discharge rate, [3] = minimum green time
    }
}
