using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Initialising_Genome
    {
        int NumberOfStages = FixedVariables.NumberOfStages;
        int MinimumGreenTime = FixedVariables.MinimumGreenTime;
        int IntergreenTime = FixedVariables.IntergreenTime;
        int MaximumGreenTime = FixedVariables.MaximumGreenTime;
        int IntergreenStageNumber = FixedVariables.IntergreenStageNumber;
        static int MaxCycleTime = FixedVariables.MaxCycleTime;
        
        int[] IntergreenAndLength = new int[2]; //This is the Intergreen Number followed by the Stage Length

        //int[] InitialCyclePlan = new int[MaxCycleTime];         //Final list will look like [1,1,1,1,1,1,1,9,9,9,9,9...]   so every second has a value
        List<int[]> InitialCyclePlanList = new List<int[]>();   //Final list will look like [(1,10),(9,5),(2,15),(9,5),...]

        Random RandomGenerator = new Random();

        private int RandomStageNumber()
        {
            return RandomGenerator.Next(1, NumberOfStages + 1);   //This returns a random stage number
        }

        private int RandomStageLength()
        {
            return RandomGenerator.Next(MinimumGreenTime, MaximumGreenTime + 1);   //This returns a random stage length between minimum green time and maximum green time
        }

        public List<int[]> GenerateCyclePlan()     //This will populate "InitialCyclePlan" with a random stage order and random stage lengths
        {
            InitialCyclePlanList.Clear();
            int TimeUsed = 0;
            //List<int> TempCyclePlan = new List<int>();
            int PreviousStage = 0;

            while (TimeUsed < MaxCycleTime)
            {
                int StageNumber = RandomStageNumber();   //This generates the stage number
                while (StageNumber == PreviousStage)    //This makes sure the same stage is not called one after the other
                {
                    StageNumber = RandomStageNumber();
                }

                int StageLength = RandomStageLength();      //This generates the length of the stage

                if ((TimeUsed >= MaxCycleTime - MaximumGreenTime - MinimumGreenTime - (2 * IntergreenTime)) && (TimeUsed < MaxCycleTime - MaximumGreenTime - IntergreenTime))  //This is the point where we must forceably make at least two more stages (we don't want to take too much time so that the minimum green no longer works for the next stage)
                {
                    StageLength = RandomGenerator.Next(MinimumGreenTime, MaxCycleTime - TimeUsed - MinimumGreenTime - (2 * IntergreenTime));   //This ensures that the next stage will still have enough time for minimum green
                }

                if (TimeUsed >= MaxCycleTime - MaximumGreenTime - IntergreenTime)   // ie. There is enough time left for up to a maximum green and intergreen
                {
                    int UsableTimeLeft = MaxCycleTime - TimeUsed - IntergreenTime;        //This is the maximum green time allowable for this stage
                    StageLength = RandomGenerator.Next(MinimumGreenTime, UsableTimeLeft);
                    if (StageLength > UsableTimeLeft - MinimumGreenTime - IntergreenTime)  //If the random Stage Length would not leave enough time for one more stage then it will use all of the remaining time
                    {
                        StageLength = UsableTimeLeft;
                    }
                }
                
                /*for (int i = 0; i < StageLength; i++)       //This is for the genome looking like [1,1,1,1,1,1,1,9,9,9,9,9...]
                {
                    TempCyclePlan.Add(StageNumber);
                }
                for (int i = 0; i < IntergreenTime; i++)
                {
                    TempCyclePlan.Add(IntergreenStageNumber);           //Note that if there are more than 8 stages then this will be an issue
                }*/

                int[] StageAndLength = new int[2];      //This is the Stage Number followed by the Stage Length
                StageAndLength[0] = StageNumber;
                StageAndLength[1] = StageLength;

                IntergreenAndLength[0] = IntergreenStageNumber;
                IntergreenAndLength[1] = IntergreenTime;

                InitialCyclePlanList.Add(StageAndLength);       //Adds to the list the (StageNumber, StageLength) and then the (IntergreenNumber, IntergreenLength)
                InitialCyclePlanList.Add(IntergreenAndLength);  //Final list will look like [(1,10),(9,5),(2,15),(9,5),...]
                
                TimeUsed += StageLength + IntergreenTime;
                PreviousStage = StageNumber;
            }

            //Everything between these points is just to write the output to the console...
            /*Console.WriteLine(TempCyclePlan.Count());
            for (int i = 0; i < MaxCycleTime; i++)
            {
                InitialCyclePlan[i] = TempCyclePlan[i];
                Console.Write(TempCyclePlan[i] + ",");
            }
            Console.WriteLine();
            foreach (int[] StageAndTime in InitialCyclePlanList)
            {
                foreach (int item in StageAndTime)
                {
                    Console.Write(item + ",");
                }
                Console.WriteLine();
            }*/
            //Console.Read();
            //...end of Console output.

            return InitialCyclePlanList;
        }





    }
}
