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

        List<double[]> ListOfStages = new List<double[]>();

        public List<double[]> PopulateStages()
        {
            ListOfStages.Add(FV.Stage1);
            ListOfStages.Add(FV.Stage2);
            ListOfStages.Add(FV.Stage3);
            ListOfStages.Add(FV.Stage4);
            return ListOfStages;
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

        public List<int[]> RunAlgorithm(int StartingSeeds, int StepsClimbed, int MutationsAroundAPoint, List<double[]> CurrentRoadState)
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
                    AllowableInitialCyclePlan = MU.AllowableCyclePlan(CyclePlan);
                }
                
                //System.Threading.Thread.Sleep(50);

                double InitialDelay = FF.RunnerFunction(CyclePlan, LeastDelay, CurrentRoadState);
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
                        bool IsCyclePlanAllowable = MU.AllowableCyclePlan(TempCyclePlan);
                        if (IsCyclePlanAllowable == false)
                        {
                            continue;
                        }
                        double TempDelayTotal = 0;

                        TempDelayTotal = FF.RunnerFunction(TempCyclePlan, TempMultipleMutationLeastDelay, CurrentRoadState);

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
