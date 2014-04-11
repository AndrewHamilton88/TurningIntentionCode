using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Queue_Lengths
    {
        int NumberOfStages = FixedVariables.NumberOfStages;

        public List<double[]> UpdateQueueLength(int CurrentStage, double time, List<double[]> CurrentRoadState)
        {
            List<double[]> NewRoadState = CurrentRoadState;
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                NewRoadState[CurrentStage - 1][0] += (NewRoadState[CurrentStage - 1][1] - NewRoadState[CurrentStage - 1][2] * time);   //This is updating the current stage which is on to reflect the current arrival and discharge rates for the specified time
                if (NewRoadState[CurrentStage - 1][0] < 0)
                {
                    NewRoadState[CurrentStage - 1][0] = 0;
                }

            }

            for (int Stage = 1; Stage < NumberOfStages + 1; Stage++)  //This updates the stopped stages using the arrival rate (nothing will be discharged)
            {
                if (Stage != CurrentStage)
                {
                    NewRoadState[Stage - 1][0] += (NewRoadState[Stage - 1][1] * time);
                }
            }
            return NewRoadState;
        }
    }
}
