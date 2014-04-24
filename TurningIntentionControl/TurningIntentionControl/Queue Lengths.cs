using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Queue_Lengths
    {
        int NumberOfStages = FixedVariables.NumberOfStages;
        int NumberOfPhases = FixedVariables.NumberOfPhases;

        public List<double[]> UpdateQueueLength(int CurrentStage, double time, List<double[]> CurrentRoadState, List<int[]> PhaseList)
        {
            List<double[]> NewRoadState = CurrentRoadState;

            
            //This will only work if the current road state is built up of all 12 phases - Road state looks like [Phase1RoadState, Phase2RoadState...]
            List<int> ActivePhases = new List<int>();
            ActivePhases.Clear();

            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                foreach (int Phase in PhaseList[CurrentStage - 1])
                {
                    ActivePhases.Add(Phase);

                    //This is updating the current phase which is on to reflect the current arrival and discharge rates for the specified time
                    NewRoadState[Phase - 1][0] += ((NewRoadState[Phase - 1][1] - NewRoadState[Phase - 1][2]) * time);
                    if (NewRoadState[Phase - 1][0] < 0)
                    {
                        NewRoadState[Phase - 1][0] = 0;
                    }
                }
            }

            for (int Phase = 1; Phase < NumberOfPhases + 1; Phase++)  //This updates the stopped stages using the arrival rate (nothing will be discharged)
            {
                if (!ActivePhases.Contains(Phase))
                {
                    NewRoadState[Phase - 1][0] += (NewRoadState[Phase - 1][1] * time);
                }
            }
            return NewRoadState;
            

            /*
            //The following code only works for a 4 stage model with the Road State as the four stages [Stage1RoadState,Stage2RoadState....]
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                NewRoadState[CurrentStage - 1][0] += ((NewRoadState[CurrentStage - 1][1] - NewRoadState[CurrentStage - 1][2]) * time);   //This is updating the current stage which is on to reflect the current arrival and discharge rates for the specified time
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
            return NewRoadState;*/
        }
    }
}
