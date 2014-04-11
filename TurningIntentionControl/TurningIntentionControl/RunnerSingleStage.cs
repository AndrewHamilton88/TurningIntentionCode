using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParamincsSNMPcontrol
{
    class RunnerSingleStage
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

        List<double[]> ListOfStages = new List<double[]>();

        public List<double[]> PopulateStages()
        {
            ListOfStages.Add(FV.Stage1);
            ListOfStages.Add(FV.Stage2);
            ListOfStages.Add(FV.Stage3);
            ListOfStages.Add(FV.Stage4);
            return ListOfStages;
        }

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
        public Answer RunAlgorithm(int StartingSeeds, int StepsClimbed, int MutationsAroundAPoint, List<double[]> CurrentRoadState, int PreviousStageNumber)
        {
            List<int[]> BestCyclePlan = new List<int[]>();
            double LeastDelay = 9999999999;
            double LeastDelayPerSecond = 9999999999;
            
            int WhileCounter = 0;

            while (WhileCounter < StartingSeeds)           //Number of starting seeds
            {
                List<int[]> CyclePlan = new List<int[]>();
                CyclePlan.Clear();
                CyclePlan = SSIG.GenerateCyclePlan(PreviousStageNumber);   //This is generates a random stage and stage length - TODO Fix Previous Stage Number
                int InitialCyclePlanLength = 0;
                InitialCyclePlanLength = CyclePlanLength(CyclePlan); 

                double InitialDelay = FF.RunnerFunction(CyclePlan, LeastDelay, CurrentRoadState);
                double InitialDelayPerSecond = InitialDelay / InitialCyclePlanLength;

                if (InitialDelayPerSecond < LeastDelayPerSecond)                                          //This just checks to see if the initial seed is the best cycle plan
                {
                    LeastDelay = InitialDelay;
                    BestCyclePlan = CyclePlan;
                    LeastDelayPerSecond = InitialDelayPerSecond;
                }
                Console.WriteLine(InitialDelay);
                Console.WriteLine(InitialDelayPerSecond);

                List<int[]> TempBestPlan = new List<int[]>();                           //This is for the lowest delay plan after each set of mutations (so after a cycle plan has been mutated, then the best plan out of the set of mutations will be stored here)
                TempBestPlan = CyclePlan;                           //This uses the initial seed as the current best plan
                double TempLeastDelay;
                TempLeastDelay = InitialDelay;
                double TempLeastDelayPerSecond;
                TempLeastDelayPerSecond = InitialDelayPerSecond;

                int TempWhileCounter = 0;
                while (TempWhileCounter < StepsClimbed)                                          //This while loop will repeat the mutation process 'x' amount of times for the current best Cycle plan from the current seed (ie. the algorithm will try to climb the hill 'x' times) - this is the step generation
                {
                    List<int[]> TempCyclePlan = new List<int[]>();                      //This is so the mutated cycle plan has an identity
                    List<int[]> TempMultipleMutationBestPlan = new List<int[]>();       //This is to store the best mutated cycle plan
                    double TempMultipleMutationLeastDelay = 9999999999;                 //This is to store the best mutated cycle plan's delay
                    double TempMultipleMutationLeastDelayPerSecond = 9999999999;

                    for (int i = 0; i < MutationsAroundAPoint; i++)                                         //This for-loop trials 'y' number of mutations of the current best cycle plan from the current seed (ie. the algorithm will search through the nearest location 'y' times) - it finds the lowest delay around the current plan
                    {
                        TempCyclePlan = MU.MutateCyclePlan(TempBestPlan);
                        double TempDelayTotal = 0;
                        int TempCycleLength = 0;
                        double TempDelayPerSecond = 0;

                        TempDelayTotal = FF.RunnerFunction(TempCyclePlan, TempMultipleMutationLeastDelay, CurrentRoadState);
                        TempCycleLength = CyclePlanLength(TempCyclePlan);
                        TempDelayPerSecond = TempDelayTotal / TempCycleLength;

                        if (TempDelayPerSecond < TempMultipleMutationLeastDelayPerSecond)            //Currently this ignores any stage with the same amount of delay...
                        {
                            TempMultipleMutationLeastDelay = TempDelayTotal;
                            TempMultipleMutationBestPlan = TempCyclePlan;
                            TempMultipleMutationLeastDelayPerSecond = TempDelayPerSecond;
                        }
                    }
                    //Console.WriteLine(TempMultipleMutationLeastDelay);

                    if (TempMultipleMutationLeastDelayPerSecond < TempLeastDelayPerSecond)
                    {
                        TempLeastDelay = TempMultipleMutationLeastDelay;
                        TempBestPlan = TempMultipleMutationBestPlan;
                        TempLeastDelayPerSecond = TempMultipleMutationLeastDelayPerSecond;
                        //sw.WriteLine(TempLeastDelay + ", After," + (TempWhileCounter + 1) + ",Steps");
                    }
                    //Console.WriteLine(TempLeastDelay);
            
                    TempWhileCounter++;                                                 //This ensures that we cycle through the mutation loop (for
                }

                if (TempLeastDelayPerSecond < LeastDelayPerSecond)
                {
                    LeastDelay = TempLeastDelay;
                    BestCyclePlan = TempBestPlan;
                    LeastDelayPerSecond = TempLeastDelayPerSecond;
                }


                
                //Console.WriteLine(TempLeastDelay);
                WhileCounter++;
                /*if (WhileCounter % 10 == 0)
                {
                    Console.WriteLine(WhileCounter);
                }*/
            }

            //sw.Write("Lowest Delay = ," + LeastDelay);
            //sw.WriteLine();
            //sw.Close();

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

            Answer Returner = new Answer();
            Returner.TotalDelay = LeastDelay;
            Returner.Cycleplan = BestCyclePlan;
            Returner.TotalDelayPerSecond = LeastDelayPerSecond;

            Console.WriteLine(Returner.TotalDelay);
            Console.WriteLine(Returner.TotalDelayPerSecond);
            foreach (int[] stage in Returner.Cycleplan)
            {
                foreach (int item in stage)
                {
                    Console.Write(item + ",");
                }
                Console.WriteLine();
            }

            return Returner;
        }

    }
}
