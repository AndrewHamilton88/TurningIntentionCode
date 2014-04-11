using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Performance
    {
        int NumberOfStages = FixedVariables.NumberOfStages;

        public double DelayFunctionOtherStages(int CurrentStage, double TimeActive, List<double[]> RoadState, double CurrentLowestDelay)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnOtherStages = 0; //This function does not include any delay caused to the current stage

            for (int Stage = 1; Stage < NumberOfStages + 1; Stage++)
            {
                if (DelayOnOtherStages <= CurrentLowestDelay)
                {
                    if (Stage != CurrentStage)
                    {
                        DelayOnOtherStages += TimeActive * (RoadState[Stage - 1][0] + TimeActive * (RoadState[Stage - 1][1]));
                    }
                }
                else
                {
                    return 99999999999.00;
                }

            }
            return DelayOnOtherStages;
        }

        public double DelayFunctionCurrentStage(int CurrentStage, double TimeActive, List<double[]> RoadState)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnActiveStage = 0;
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                DelayOnActiveStage += RoadState[CurrentStage - 1][0] * TimeActive;                
            }
            return DelayOnActiveStage;
        }
    }
}
