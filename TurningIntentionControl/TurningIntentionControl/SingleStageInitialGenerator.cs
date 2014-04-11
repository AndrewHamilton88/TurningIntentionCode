using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class SingleStageInitialGenerator
    {
        int NumberOfStages = FixedVariables.NumberOfStages;
        int MinimumGreenTime = FixedVariables.MinimumGreenTime;
        int IntergreenTime = FixedVariables.IntergreenTime;
        int MaximumGreenTime = FixedVariables.MaximumGreenTime;
        int IntergreenStageNumber = FixedVariables.IntergreenStageNumber;
        static int MaxCycleTime = FixedVariables.MaxCycleTime;
        
        int[] IntergreenAndLength = new int[2]; //This is the Intergreen Number followed by the Stage Length

        List<int[]> InitialCyclePlanList = new List<int[]>();   //Final list will look like [(1,10),(9,5)]

        Random RandomGenerator = new Random();

        private int RandomStageNumber()
        {
            return RandomGenerator.Next(1, NumberOfStages + 1);   //This returns a random stage number
        }

        private int RandomStageLength()
        {
            return RandomGenerator.Next(MinimumGreenTime, MaximumGreenTime + 1);   //This returns a random stage length between minimum green time and maximum green time
        }

        public List<int[]> GenerateCyclePlan(int PreviousStageNumber)     //This will populate "InitialCyclePlan" with a random stage number and random stage length
        {
            InitialCyclePlanList.Clear();

            int StageNumber = RandomStageNumber();   //This generates the stage number
            while (StageNumber == PreviousStageNumber)    //This makes sure the same stage is not called one after the other
            {
                StageNumber = RandomStageNumber();
            }
            int StageLength = RandomStageLength();      //This generates the length of the stage
                
            int[] StageAndLength = new int[2];      //This is the Stage Number followed by the Stage Length
            StageAndLength[0] = StageNumber;
            StageAndLength[1] = StageLength;

            IntergreenAndLength[0] = IntergreenStageNumber;
            IntergreenAndLength[1] = IntergreenTime;

            InitialCyclePlanList.Add(StageAndLength);       //Adds to the list the (StageNumber, StageLength) and then the (IntergreenNumber, IntergreenLength)
            InitialCyclePlanList.Add(IntergreenAndLength);  //Final list will look like [(1,10),(9,5)]
                
            PreviousStageNumber = StageNumber;


            foreach (int[] StageAndTime in InitialCyclePlanList)
            {
                foreach (int item in StageAndTime)
                {
                    Console.Write(item + ",");
                }
                Console.WriteLine();
            }
            //Console.Read();

            return InitialCyclePlanList;
        }





    }
}
