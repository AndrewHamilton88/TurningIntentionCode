using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParamincsSNMPcontrol
{
    class RunnerCyclePlan
    {
        FixedVariables FV = new FixedVariables();
        
        Initialising_Genome IG = new Initialising_Genome();
        Performance Perf = new Performance();
        Queue_Lengths Queue = new Queue_Lengths();
        FinalFunction FF = new FinalFunction();
        Mutate MU = new Mutate();

        int NumberOfStages = FixedVariables.NumberOfStages;
        int NumberOfTimeSteps = FixedVariables.MaxCycleTime;

        List<double[]> ListOfStagesRoadState = new List<double[]>();
        List<int[]> ListOfPhases = new List<int[]>();

        public List<double[]> PopulateStagesFourStageModel()        //Current Road State for four stage model
        {
            ListOfStagesRoadState.Add(FV.Stage1);
            ListOfStagesRoadState.Add(FV.Stage2);
            ListOfStagesRoadState.Add(FV.Stage3);
            ListOfStagesRoadState.Add(FV.Stage4);
            return ListOfStagesRoadState;
        }

        public List<int[]> PopulatePhasesFourStageModel()        //The phases which are active when the stage is called
        {
            ListOfPhases.Add(FV.Stage1Phases);
            ListOfPhases.Add(FV.Stage2Phases);
            ListOfPhases.Add(FV.Stage3Phases);
            ListOfPhases.Add(FV.Stage4Phases);
            return ListOfPhases;
        }

        public List<double[]> Populate12PhasesRoadState()        //Current Road State for all 12 phases
        {
            ListOfStagesRoadState.Add(FV.Phase1);
            ListOfStagesRoadState.Add(FV.Phase2);
            ListOfStagesRoadState.Add(FV.Phase3);
            ListOfStagesRoadState.Add(FV.Phase4);
            ListOfStagesRoadState.Add(FV.Phase5);
            ListOfStagesRoadState.Add(FV.Phase6);
            ListOfStagesRoadState.Add(FV.Phase7);
            ListOfStagesRoadState.Add(FV.Phase8);
            ListOfStagesRoadState.Add(FV.Phase9);
            ListOfStagesRoadState.Add(FV.Phase10);
            ListOfStagesRoadState.Add(FV.Phase11);
            ListOfStagesRoadState.Add(FV.Phase12);
            return ListOfStagesRoadState;
        }

        public List<int[]> PopulatePhasesEightStageModel()        //The phases which are active when the stage is called
        {
            ListOfPhases.Add(FV.Stage1Phases8Stage);
            ListOfPhases.Add(FV.Stage2Phases8Stage);
            ListOfPhases.Add(FV.Stage3Phases8Stage);
            ListOfPhases.Add(FV.Stage4Phases8Stage);
            ListOfPhases.Add(FV.Stage5Phases8Stage);
            ListOfPhases.Add(FV.Stage6Phases8Stage);
            ListOfPhases.Add(FV.Stage7Phases8Stage);
            ListOfPhases.Add(FV.Stage8Phases8Stage);
            return ListOfPhases;
        }

        public List<int[]> PopulatePhasesSeventeenStageModel()        //The phases which are active when the stage is called
        {
            ListOfPhases.Add(FV.Stage1Phases17Stage);
            ListOfPhases.Add(FV.Stage2Phases17Stage);
            ListOfPhases.Add(FV.Stage3Phases17Stage);
            ListOfPhases.Add(FV.Stage4Phases17Stage);
            ListOfPhases.Add(FV.Stage5Phases17Stage);
            ListOfPhases.Add(FV.Stage6Phases17Stage);
            ListOfPhases.Add(FV.Stage7Phases17Stage);
            ListOfPhases.Add(FV.Stage8Phases17Stage);
            ListOfPhases.Add(FV.Stage9Phases17Stage);
            ListOfPhases.Add(FV.Stage10Phases17Stage);
            ListOfPhases.Add(FV.Stage11Phases17Stage);
            ListOfPhases.Add(FV.Stage12Phases17Stage);
            ListOfPhases.Add(FV.Stage13Phases17Stage);
            ListOfPhases.Add(FV.Stage14Phases17Stage);
            ListOfPhases.Add(FV.Stage15Phases17Stage);
            ListOfPhases.Add(FV.Stage16Phases17Stage);
            ListOfPhases.Add(FV.Stage17Phases17Stage);
            return ListOfPhases;
        }

        public List<int[]> PopulatePhasesTwoStageModel()        //The phases which are active when the stage is called
        {
            ListOfPhases.Add(FV.Stage1Phases2Stage);
            ListOfPhases.Add(FV.Stage2Phases2Stage);
            return ListOfPhases;
        }


        private List<int[]> CopyCyclePlan(List<int[]> CyclePlan)
        {
            List<int[]> Returner = new List<int[]>();
            foreach (int[] item in CyclePlan)
            {
                Returner.Add(item);
            }
            return Returner;
        }

        private List<double[]> CopyRoadState(List<double[]> RoadState)
        {
            List<double[]> Returner = new List<double[]>();
            foreach (double[] item in RoadState)
            {
                Returner.Add(item);
            }
            return Returner;
        }

        private double CopyDouble(double Value)
        {
            double Result = Value;
            return Result;
        }

        public List<int[]> RunAlgorithm(int StartingSeeds, int StepsClimbed, int MutationsAroundAPoint, List<double[]> CurrentRoadState, List<int[]> PhaseList)
        {
            //List<double[]> CurrentRoadState = PopulateStages();
            List<int[]> BestCyclePlan = new List<int[]>();
            double LeastDelay = 9999999999;
            
            //StreamWriter sw = new StreamWriter("DetailedOutputV5For" + StartingSeeds + "seeds," + StepsClimbed + "steps," + MutationsAroundAPoint + "Mutations.csv");
            
            int WhileCounter = 0;

            while (WhileCounter < StartingSeeds)           //This is the number of starting Cycle Plans
            {
                List<int[]> CyclePlan = new List<int[]>();
                CyclePlan.Clear();
                //Initialising_Genome IG = new Initialising_Genome();
                bool AllowableInitialCyclePlan = false;
                while (AllowableInitialCyclePlan == false)
                {
                    CyclePlan = IG.GenerateCyclePlan();     //This is generates a new starting point - the initial seed
                    AllowableInitialCyclePlan = MU.AllowableCyclePlanPhases(CyclePlan, PhaseList);
                    //AllowableInitialCyclePlan = MU.AllowableCyclePlanStages(CyclePlan);
                }
                
                //System.Threading.Thread.Sleep(50);

                double InitialDelay = FF.RunnerFunction(CyclePlan, LeastDelay, CurrentRoadState, PhaseList);
                if (InitialDelay < LeastDelay)                                          //This just checks to see if the initial seed is the best cycle plan
                {
                    LeastDelay = InitialDelay;
                    BestCyclePlan = CyclePlan;
                }

                //sw.WriteLine(InitialDelay + ",Initial delay, 0, Steps");
                //sw.WriteLine();

                /*Console.WriteLine("The initial cycle plan is:");
                foreach (int[] StageAndTime in CyclePlan)
                {
                    foreach (int item in StageAndTime)
                    {
                        Console.Write(item + ",");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("And the delay is: " + InitialDelay);*/

                List<int[]> TempBestPlan = new List<int[]>();                           //This is for the lowest delay plan after each set of mutations (so after a cycle plan has been mutated, then the best plan out of the set of mutations will be stored here)
                TempBestPlan = CyclePlan;                           //This uses the initial seed as the current best plan
                double TempLeastDelay;                          
                TempLeastDelay = InitialDelay;

                int TempWhileCounter = 0;
                while (TempWhileCounter < StepsClimbed)                                          //This while loop will repeat the mutation process 'x' amount of times for the current best Cycle plan from the current seed (ie. the algorithm will try to climb the hill 'x' times) - this is the step generation
                {
                    List<int[]> TempCyclePlan = new List<int[]>();                      //This is so the mutated cycle plan has an identity
                    List<int[]> TempMultipleMutationBestPlan = new List<int[]>();       //This is to store the best mutated cycle plan
                    double TempMultipleMutationLeastDelay = 9999999999;                 //This is to store the best mutated cycle plan's delay

                    for (int i = 0; i < MutationsAroundAPoint; i++)                                         //This for-loop trials 'y' number of mutations of the current best cycle plan from the current seed (ie. the algorithm will search through the nearest location 'y' times) - it finds the lowest delay around the current plan
                    {
                        TempCyclePlan = MU.MutateCyclePlan(TempBestPlan);
                        bool IsCyclePlanAllowable = MU.AllowableCyclePlanPhases(TempCyclePlan, PhaseList);
                        //bool IsCyclePlanAllowable = MU.AllowableCyclePlanStages(TempCyclePlan);
                        if (IsCyclePlanAllowable == false)
                        {
                            continue;
                        }
                        double TempDelayTotal = 0;

                        TempDelayTotal = FF.RunnerFunction(TempCyclePlan, TempMultipleMutationLeastDelay, CurrentRoadState, PhaseList);

                        if (TempDelayTotal < TempMultipleMutationLeastDelay)            //Currently this ignores any stage with the same amount of delay...
                        {
                            TempMultipleMutationLeastDelay = TempDelayTotal;
                            TempMultipleMutationBestPlan = TempCyclePlan;
                        }
                    }
                    //Console.WriteLine(TempMultipleMutationLeastDelay);

                    if (TempMultipleMutationLeastDelay < TempLeastDelay)
                    {
                        TempLeastDelay = TempMultipleMutationLeastDelay;
                        TempBestPlan = TempMultipleMutationBestPlan;
                        //sw.Write(TempLeastDelay + ", After," + (TempWhileCounter + 1) + ",Steps,");
                        /*foreach (int[] stage in TempBestPlan)
                        {
                            foreach (int item in stage)
                            {
                                sw.Write(item + ",");
                            }
                        }
                        sw.WriteLine();*/
                    }
                    //Console.WriteLine(TempLeastDelay);
            
                    TempWhileCounter++;                                                 //This ensures that we cycle through the mutation loop (for
                }

                if (TempLeastDelay < LeastDelay)
                {
                    LeastDelay = TempLeastDelay;
                    BestCyclePlan = TempBestPlan;
                }


                
                //Console.WriteLine(TempLeastDelay);
                WhileCounter++;
                /*if (WhileCounter % 10 == 0)
                {
                    Console.WriteLine(WhileCounter);
                }*/
            }

            /*sw.Write("Lowest Delay = ," + LeastDelay + ", , ,");
            foreach (int[] stage in BestCyclePlan)
            {
                foreach (int item in stage)
                {
                    sw.Write(item + ",");
                }
            }
            sw.WriteLine();
            sw.Close();*/

            /*Console.Write("The best cycle plan is: ");
            foreach (int[] stage in BestCyclePlan)
            {
                foreach (int item in stage)
                {
                    Console.Write(item + ",");
                }
                Console.WriteLine();
            }
            Console.Write(" with a Total Delay = " + Convert.ToString(LeastDelay));
            Console.Read();*/
            //Console.WriteLine(LeastDelay);
            //return LeastDelay;
            return BestCyclePlan;
        }

    }
}
