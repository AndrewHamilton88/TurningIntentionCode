using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class FixedVariables
    {
        public int StartingSeeds = 20;
        public int StepsClimbed = 200;
        public int MutationsAroundAPoint = 200;
        
        public static int NumberOfStages = 8;
        public static int NumberOfPhases = 12;
        public static int MinimumGreenTime = 7;
        public static int IntergreenTime = 5;
        public static int MaximumGreenTime = 25;
        public static int IntergreenStageNumber = 99;
        public static int MaxCycleTime = 120;
        public int IntergreenTimeVariable = 5;

        //Four Stage Model
        public double[] Stage1 = { 2, 2, 3};  //double [] will contain [0] = queue length, [1] = arrival rate, [2] = discharge rate
        public double[] Stage2 = { 3, 2, 3};
        public double[] Stage3 = { 5, 3, 4};
        public double[] Stage4 = { 8, 1, 3};

        public int[] Stage1Phases = { 1, 2, 3};     //These are the active phases when the corresponding stage is called
        public int[] Stage2Phases = { 4, 5, 6};
        public int[] Stage3Phases = { 7, 8, 9};
        public int[] Stage4Phases = { 10, 11, 12};

        //12 Phase Model
        public double[] Phase1 = { 0.66667, 0.66667, 1 };  //double [] will contain [0] = queue length, [1] = arrival rate, [2] = discharge rate
        public double[] Phase2 = { 0.66667, 0.66667, 1 };
        public double[] Phase3 = { 0.66667, 0.66667, 1 };
        public double[] Phase4 = { 1, 0.66667, 1 };
        public double[] Phase5 = { 1, 0.66667, 1 };
        public double[] Phase6 = { 1, 0.66667, 1 };
        public double[] Phase7 = { 1.66667, 1, 1.33333 };
        public double[] Phase8 = { 1.66667, 1, 1.33333 };
        public double[] Phase9 = { 1.66667, 1, 1.33333 };
        public double[] Phase10 = { 2.66667, 0.33333, 1 };
        public double[] Phase11 = { 2.66667, 0.33333, 1 };
        public double[] Phase12 = { 2.66667, 0.33333, 1 };


        //8 Stage Model
        public int[] Stage1Phases8Stage = { 1, 2, 3, 7, 8, 9 };     //These are the active phases when the corresponding stage is called
        public int[] Stage2Phases8Stage = { 1, 2, 3, 7, 9, 12 };
        public int[] Stage3Phases8Stage = { 1, 3, 6, 7, 8, 9 };
        public int[] Stage4Phases8Stage = { 1, 3, 6, 7, 9, 12 };
        public int[] Stage5Phases8Stage = { 3, 4, 5, 6, 10, 12 };
        public int[] Stage6Phases8Stage = { 3, 4, 6, 9, 10, 12 };
        public int[] Stage7Phases8Stage = { 4, 5, 6, 10, 11, 12 };
        public int[] Stage8Phases8Stage = { 4, 6, 9, 10, 11, 12 };


        //17 Stage Model
        public int[] Stage1Phases17Stage = { 1, 2, 3, 12 };     //These are the active phases when the corresponding stage is called
        public int[] Stage2Phases17Stage = { 3, 4, 5, 6 };
        public int[] Stage3Phases17Stage = { 6, 7, 8, 9 };
        public int[] Stage4Phases17Stage = { 9, 10, 11, 12 };
        public int[] Stage5Phases17Stage = { 1, 3, 6, 12 };
        public int[] Stage6Phases17Stage = { 3, 4, 6, 9 };
        public int[] Stage7Phases17Stage = { 3, 9, 10, 12 };
        public int[] Stage8Phases17Stage = { 6, 7, 9, 12 };
        public int[] Stage9Phases17Stage = { 1, 6, 7, 12 };
        public int[] Stage10Phases17Stage = { 3, 4, 9, 10 };
        public int[] Stage11Phases17Stage = { 2, 3, 8, 9 };
        public int[] Stage12Phases17Stage = { 5, 6, 11, 12 };
        public int[] Stage13Phases17Stage = { 2, 3, 9, 12 };
        public int[] Stage14Phases17Stage = { 3, 5, 6, 12 };
        public int[] Stage15Phases17Stage = { 3, 6, 8, 9 };
        public int[] Stage16Phases17Stage = { 6, 9, 11, 12 };
        public int[] Stage17Phases17Stage = { 3, 6, 9, 12 };
        
    }
}
