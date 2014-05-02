using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class RunnerAllSingleStageOptions
    {
        FixedVariables FV = new FixedVariables();

        SingleStageInitialGenerator SSIG = new SingleStageInitialGenerator();
        Initialising_Genome IG = new Initialising_Genome();
        Performance Perf = new Performance();
        Queue_Lengths Queue = new Queue_Lengths();
        FinalFunction FF = new FinalFunction();
        Mutate MU = new Mutate();

        int NumberOfStages = FixedVariables.NumberOfStages;
        int NumberOfTimeSteps = FixedVariables.MaxCycleTime;
        int MaxCycleTime = FixedVariables.MaxCycleTime;
        int MinimumGreen = FixedVariables.MinimumGreenTime;
        int MaxGreenTime = FixedVariables.MaximumGreenTime;
        int IntergreenTime = FixedVariables.IntergreenTime;
        int IntergreenStageNumber = FixedVariables.IntergreenStageNumber;

        int[] IntergreenAndLength = new int[2]; //This is the Intergreen Number followed by the Stage Length

        List<double[]> ListOfStages = new List<double[]>();

        public int CyclePlanLength(List<int[]> CyclePlan)
        {
            int Returner = 0;
            foreach (int[] item in CyclePlan)
            {
                Returner += item[1];            //This combines the stage length and the intergrenn length together (ie. total cycle plan length
            }
            return Returner;
        }

        /// <summary>
        /// This function will return a single stage followed by an intergreen time. The performance measure is "Lowest Delay divided by entire cycle length".
        /// This is because if we return least delay for a variable length single stage then we are not making a fair comparison as one answer could be 20 seconds
        /// long and another answer would be 12 seconds long - so the least delay would not make sense. But "delay / cycle time" is a fair comparison.
        /// </summary>
        /// <param name="StartingSeeds"></param>
        /// <param name="StepsClimbed"></param>
        /// <param name="MutationsAroundAPoint"></param>
        /// <param name="CurrentRoadState"></param>
        /// <param name="PreviousStageNumber"></param>
        /// <returns></returns>
        public List<int[]> RunAlgorithm(List<double[]> CurrentRoadState, int PreviousStageNumber, List<int[]> PhaseList)
        {
            List<int[]> BestCyclePlan = new List<int[]>();
            double LeastDelay = 9999999999;
            double LeastDelayPerSecond = 9999999999;

            IntergreenAndLength[0] = IntergreenStageNumber;
            IntergreenAndLength[1] = IntergreenTime;
            
            for (int StageNumber = 1; StageNumber < NumberOfStages + 1; StageNumber++)
			{
                if (StageNumber != PreviousStageNumber)
	            {
                    for (int StageLength = MinimumGreen; StageLength < MaxGreenTime + 1; StageLength++)
			        {
                        List<int[]> CyclePlan = new List<int[]>();
                        CyclePlan.Clear();

                        int[] StageAndLength = new int[2];      //This is the Stage Number followed by the Stage Length
                        StageAndLength[0] = StageNumber;
                        StageAndLength[1] = StageLength;

                        CyclePlan.Add(StageAndLength);
                        CyclePlan.Add(IntergreenAndLength);

                        int InitialCyclePlanLength = 0;
                        InitialCyclePlanLength = CyclePlanLength(CyclePlan);

                        double InitialDelay = FF.RunnerFunction(CyclePlan, LeastDelay, CurrentRoadState, PhaseList);
                        double InitialDelayPerSecond = InitialDelay / InitialCyclePlanLength;

                        if (InitialDelayPerSecond < LeastDelayPerSecond)                                          //This just checks to see if the initial seed is the best cycle plan
                        {
                            LeastDelay = InitialDelay;
                            BestCyclePlan = CyclePlan;
                            LeastDelayPerSecond = InitialDelayPerSecond;
                        }

			        }
                }
			}
            return BestCyclePlan;
        }



    }
}
