using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Performance
    {
        int NumberOfStages = FixedVariables.NumberOfStages;
        int NumberOfPhases = FixedVariables.NumberOfPhases;

        public double DelayFunctionOtherStages(int CurrentStage, double TimeActive, List<double[]> RoadState, double CurrentLowestDelay, List<int[]> PhaseList)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnOtherStages = 0; //This function does not include any delay caused to the current stage


            
            //This will only work if the current road state is built up of all 12 phases - Road state looks like [Phase1RoadState, Phase2RoadState...]
            List<int> ActivePhases = new List<int>();
            ActivePhases.Clear();

            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                foreach (int Phase in PhaseList[CurrentStage - 1])
                {
                    ActivePhases.Add(Phase);
                }
            }

            for (int Phase = 1; Phase < NumberOfPhases + 1; Phase++)
            {
                if (DelayOnOtherStages <= CurrentLowestDelay)
                {
                    if (!ActivePhases.Contains(Phase))
                    {
                        DelayOnOtherStages += TimeActive * (RoadState[Phase - 1][0] + TimeActive * (RoadState[Phase - 1][1]));
                    }
                }
                else
                {
                    return 99999999999.00;
                }
            }
            return DelayOnOtherStages;
            

            /*
            //The following code only works for a 4 stage model with the Road State as the four stages [Stage1RoadState,Stage2RoadState....]
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
            return DelayOnOtherStages;*/
        }

        public double DelayFunctionCurrentStage(int CurrentStage, double TimeActive, List<double[]> RoadState, List<int[]> PhaseList)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnActiveStage = 0;

            
            //This will only work if the current road state is built up of all 12 phases - Road state looks like [Phase1RoadState, Phase2RoadState...]
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                foreach (int Phase in PhaseList[CurrentStage - 1])
                {
                    //This calculates the delay on the active phase
                    DelayOnActiveStage += RoadState[Phase - 1][0] * TimeActive;  
                }
            }
            return DelayOnActiveStage;


            /*
            //The following code only works for a 4 stage model with the Road State as the four stages [Stage1RoadState,Stage2RoadState....]
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                DelayOnActiveStage += RoadState[CurrentStage - 1][0] * TimeActive;                
            }
            return DelayOnActiveStage;*/
        }



        //The following functions are for my version of calculating delay - the more complicated approach

        public double DelayFunctionOtherStagesVer2(int CurrentStage, double TimeActive, List<double[]> RoadState, double CurrentLowestDelay, List<int[]> PhaseList)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnOtherStages = 0; //This function does not include any delay caused to the current stage

            //This will only work if the current road state is built up of all 12 phases - Road state looks like [Phase1RoadState, Phase2RoadState...]
            List<int> ActivePhases = new List<int>();
            ActivePhases.Clear();

            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                foreach (int Phase in PhaseList[CurrentStage - 1])
                {
                    ActivePhases.Add(Phase);
                }
            }

            for (int Phase = 1; Phase < NumberOfPhases + 1; Phase++)
            {
                if (DelayOnOtherStages <= CurrentLowestDelay)
                {
                    if (!ActivePhases.Contains(Phase))
                    {
                        DelayOnOtherStages += TimeActive * (RoadState[Phase - 1][0] + ((TimeActive + 1) * (RoadState[Phase - 1][1]) / 2));
                    }
                }
                else
                {
                    return 99999999999.00;
                }
            }
            return DelayOnOtherStages;
        }

        /// <summary>
        /// This uses the second set of delay calculations that I carried out in my notebook! This defines delay as the queue length at the end of each second.
        /// </summary>
        /// <param name="CurrentStage"></param>
        /// <param name="TimeActive"></param>
        /// <param name="RoadState"></param>
        /// <param name="PhaseList"></param>
        /// <returns></returns>
        public double DelayFunctionCurrentStageVer2(int CurrentStage, double TimeActive, List<double[]> RoadState, List<int[]> PhaseList)  //RoadState[0] = Stage1, RoadState[1] = Stage2, RoadState[2] = Stage3, Roadstate[3] = Stage4
        {
            double DelayOnActiveStage = 0;

            //This will only work if the current road state is built up of all 12 phases - Road state looks like [Phase1RoadState, Phase2RoadState...]
            if (CurrentStage != FixedVariables.IntergreenStageNumber)
            {
                foreach (int Phase in PhaseList[CurrentStage - 1])
                {
                    //This calculates the delay on the active phase
                    if ((RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) <= TimeActive)        // (queue/discharge) <= timeactive
                    {
                        DelayOnActiveStage += (RoadState[Phase - 1][0] / (2 * RoadState[Phase - 1][2])) * (RoadState[Phase - 1][0] - RoadState[Phase - 1][2]);  //Equation 1 in my notebook; (n/(2*D)) * (n - D)
                        DelayOnActiveStage += 0.5 * (RoadState[Phase - 1][1] * RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) * ((RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) + 1);  //Equation 2 in my notebook; (An/2D[n/D + 1])
                        if (RoadState[Phase - 1][1] > RoadState[Phase - 1][2])  //If the arrival rate is greater than the discharge rate
                        {
                            DelayOnActiveStage += (TimeActive * RoadState[Phase - 1][2] - RoadState[Phase - 1][0]) / (2 * RoadState[Phase - 1][2]) * ((2 * RoadState[Phase - 1][1] * RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) + (RoadState[Phase - 1][1] - RoadState[Phase - 1][2]) * (1 + TimeActive - (RoadState[Phase - 1][0] / RoadState[Phase - 1][2])));    //Equation 4; (tD - n)/(2D) * (2An/D + (A-D)(1+t-n/D))
                        }
                        if (RoadState[Phase - 1][1] == RoadState[Phase - 1][2])     //If the arrival rate is equal to the discharge rate
                        {
                            DelayOnActiveStage += (RoadState[Phase - 1][1] * RoadState[Phase - 1][0]) / (RoadState[Phase - 1][2] * RoadState[Phase - 1][2]) * (TimeActive * RoadState[Phase - 1][2] - RoadState[Phase - 1][0]);   //Equation 5; An / D^2 * (tD - n)
                        }
                        else            //If the arrival rate is less than the discharge rate
                        {
                            if (((RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) - ((RoadState[Phase - 1][1] * RoadState[Phase - 1][0]) / (RoadState[Phase - 1][2] * (RoadState[Phase - 1][1] - RoadState[Phase - 1][2])))) <= TimeActive)     // n/D - (An/(D(A-D))) <= t    This represents if the 'arrival queue' can be dissipated within the timeframe (the arrival queue is the queue built up while the initial queue is dissipated)
                            {
                                DelayOnActiveStage += (-1 * RoadState[Phase - 1][1] * RoadState[Phase - 1][0]) / (2 * RoadState[Phase - 1][2] * (RoadState[Phase - 1][1] - RoadState[Phase - 1][2])) * ((RoadState[Phase - 1][1] * RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) + (RoadState[Phase - 1][1] - RoadState[Phase - 1][2]));    // Equation 3;   -An/(2D(A-D) * (An/D + (A-D))
                            }
                            else
                            {
                                DelayOnActiveStage += (TimeActive * RoadState[Phase - 1][2] - RoadState[Phase - 1][0]) / (2 * RoadState[Phase - 1][2]) * ((2 * RoadState[Phase - 1][1] * RoadState[Phase - 1][0] / RoadState[Phase - 1][2]) + (RoadState[Phase - 1][1] - RoadState[Phase - 1][2]) * (1 + TimeActive - (RoadState[Phase - 1][0] / RoadState[Phase - 1][2])));    //Equation 4; (tD - n)/(2D) * (2An/D + (A-D)(1+t-n/D))
                            }
                        }
                    }
                    else
                    {
                        DelayOnActiveStage += (TimeActive / 2) * (2 * RoadState[Phase - 1][0] - (RoadState[Phase - 1][2] * (TimeActive + 1)));    //Equation 6;    t/2 * ( 2n - D*(t+1))
                        DelayOnActiveStage += (RoadState[Phase - 1][1] * TimeActive / 2) * (1 + TimeActive);        //Equation 7;    At/2 * (1 + t)
                    }
                }
            }
            return DelayOnActiveStage;
        }
    }
}
