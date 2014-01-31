using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParamincsSNMPcontrol
{
    class AllStages
    {
        List<List<int>> StageListUnsorted = new List<List<int>>();
        //List<List<int>> StageListSorted = new List<List<int>>();  


        List<double> Phases = new List<double>();
        double Phase1 = 50;
        double Phase2 = 50;
        double Phase3 = 150;
        double Phase4 = 40;
        double Phase5 = 123;
        double Phase6 = 15;
        double Phase7 = 55;
        double Phase8 = 50;
        double Phase9 = 50;
        double Phase10 = 110;
        double Phase11 = 120;
        double Phase12 = 130;

        public List<double> PopulatePhases()
        {
            Phases.Add(Phase1);
            Phases.Add(Phase2);
            Phases.Add(Phase3);
            Phases.Add(Phase4);
            Phases.Add(Phase5);
            Phases.Add(Phase6);
            Phases.Add(Phase7);
            Phases.Add(Phase8);
            Phases.Add(Phase9);
            Phases.Add(Phase10);
            Phases.Add(Phase11);
            Phases.Add(Phase12);
            return Phases;
        }


        private bool[,] PopulateCrossroadsList()
        {
            bool[,] Combinations = new bool[12, 12]; //(Rows,Columns)

            //One is row one, Two is row 2, etc
            bool[] One = new bool[12] { false, true, true, false, false, true, true, true, true, false, false, true };
            bool[] Two = new bool[12] { true, false, true, false, false, false, true, true, true, false, false, true };
            bool[] Three = new bool[12] { true, true, false, true, true, true, true, true, true, true, false, true };
            bool[] Four = new bool[12] { false, false, true, false, true, true, false, false, true, true, true, true };
            bool[] Five = new bool[12] { false, false, true, true, false, true, false, false, false, true, true, true };
            bool[] Six = new bool[12] { true, false, true, true, true, false, true, true, true, true, true, true };
            bool[] Seven = new bool[12] { true, true, true, false, false, true, false, true, true, false, false, true };
            bool[] Eight = new bool[12] { true, true, true, false, false, true, true, false, true, false, false, false };
            bool[] Nine = new bool[12] { true, true, true, true, false, true, true, true, false, true, true, true };
            bool[] Ten = new bool[12] { false, false, true, true, true, true, false, false, true, false, true, true };
            bool[] Eleven = new bool[12] { false, false, false, true, true, true, false, false, true, true, false, true };
            bool[] Twelve = new bool[12] { true, true, true, true, true, true, true, false, true, true, true, false };


            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    switch (i)
                    {
                        case (0):
                            Combinations[i, j] = One[j];
                            break;
                        case (1):
                            Combinations[i, j] = Two[j];
                            break;
                        case (2):
                            Combinations[i, j] = Three[j];
                            break;
                        case (3):
                            Combinations[i, j] = Four[j];
                            break;
                        case (4):
                            Combinations[i, j] = Five[j];
                            break;
                        case (5):
                            Combinations[i, j] = Six[j];
                            break;
                        case (6):
                            Combinations[i, j] = Seven[j];
                            break;
                        case (7):
                            Combinations[i, j] = Eight[j];
                            break;
                        case (8):
                            Combinations[i, j] = Nine[j];
                            break;
                        case (9):
                            Combinations[i, j] = Ten[j];
                            break;
                        case (10):
                            Combinations[i, j] = Eleven[j];
                            break;
                        case (11):
                            Combinations[i, j] = Twelve[j];
                            break;

                    }
                }
            }
            return Combinations;
        }

        public List<List<int>> ReturnAllCrossroadStages()
        {
            bool[,] Combinations = PopulateCrossroadsList();
            List<int> Stage = new List<int>();
            List<int> TempStage = new List<int>();

            int CountK = 0;
            int CountM = 0;
            int CountN = 0;
            int CountP = 0;
            int CountQ = 0;
            int CountR = 0;
            int CountS = 0;

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (Combinations[i, j] && j > i) //If level One is true, then try more levels
                    {
                        for (int k = 0; k < 12; k++)
                        {
                            CountK = 0;
                            if (Combinations[i, k] && Combinations[j, k] && k > j)  //If level Two is true, and the new phase number is higher than previous phase number
                            {
                                CountK++;
                                for (int m = 0; m < 12; m++) //Try another level, 'm' is used because 'l' looks like the number one.
                                {
                                    CountM = 0;
                                    if (Combinations[i, m] && Combinations[j, m] && Combinations[k, m] && m > k) // This checks for a four stage sequence
                                    {
                                        CountM++;
                                        for (int n = 0; n < 12; n++)
                                        {
                                            CountN = 0;
                                            if (Combinations[i, n] && Combinations[j, n] && Combinations[k, n] && Combinations[m, n] && n > m)
                                            {
                                                CountN++;
                                                for (int p = 0; p < 12; p++)
                                                {
                                                    CountP = 0;
                                                    if (Combinations[i, p] && Combinations[j, p] && Combinations[k, p] && Combinations[m, p] && Combinations[n, p] && p > n)
                                                    {
                                                        CountP++;
                                                        for (int q = 0; q < 12; q++)
                                                        {
                                                            CountQ = 0;
                                                            if (Combinations[i, q] && Combinations[j, q] && Combinations[k, q] && Combinations[m, q] && Combinations[n, q] && Combinations[p, q] && q > p)
                                                            {
                                                                CountQ++;
                                                                for (int r = 0; r < 12; r++)
                                                                {
                                                                    CountR = 0;
                                                                    if (Combinations[i, r] && Combinations[j, r] && Combinations[k, r] && Combinations[m, r] && Combinations[n, r] && Combinations[p, r] && Combinations[q, r] && r > q)
                                                                    {
                                                                        CountR++;
                                                                        for (int s = 0; s < 12; s++)
                                                                        {
                                                                            CountS = 0;
                                                                            if (Combinations[i, s] && Combinations[j, s] && Combinations[k, s] && Combinations[m, s] && Combinations[n, s] && Combinations[p, s] && Combinations[q, s] && Combinations[r, s] && s > r)
                                                                            {
                                                                                CountS++;
                                                                            }
                                                                        }
                                                                        if (CountS == 0)
                                                                        {
                                                                            Stage.Add(i);
                                                                            Stage.Add(j);
                                                                            Stage.Add(k);
                                                                            Stage.Add(m);
                                                                            Stage.Add(n);
                                                                            Stage.Add(p);
                                                                            Stage.Add(q);
                                                                            Stage.Add(r);
                                                                            TempStage = ObjectCopier.Clone(Stage);
                                                                            StageListUnsorted.Add(TempStage);
                                                                            Stage.Clear();
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (CountR == 0)
                                                                {
                                                                    Stage.Add(i);
                                                                    Stage.Add(j);
                                                                    Stage.Add(k);
                                                                    Stage.Add(m);
                                                                    Stage.Add(n);
                                                                    Stage.Add(p);
                                                                    Stage.Add(q);
                                                                    TempStage = ObjectCopier.Clone(Stage);
                                                                    StageListUnsorted.Add(TempStage);
                                                                    Stage.Clear();
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (CountQ == 0)
                                                        {
                                                            Stage.Add(i);
                                                            Stage.Add(j);
                                                            Stage.Add(k);
                                                            Stage.Add(m);
                                                            Stage.Add(n);
                                                            Stage.Add(p);
                                                            TempStage = ObjectCopier.Clone(Stage);
                                                            StageListUnsorted.Add(TempStage);
                                                            Stage.Clear();
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (CountP == 0)
                                                {
                                                    Stage.Add(i);
                                                    Stage.Add(j);
                                                    Stage.Add(k);
                                                    Stage.Add(m);
                                                    Stage.Add(n);
                                                    TempStage = ObjectCopier.Clone(Stage);
                                                    StageListUnsorted.Add(TempStage);
                                                    Stage.Clear();
                                                    break;
                                                }

                                            }
                                        }
                                        if (CountN == 0)
                                        {
                                            Stage.Add(i);
                                            Stage.Add(j);
                                            Stage.Add(k);
                                            Stage.Add(m);
                                            TempStage = ObjectCopier.Clone(Stage);
                                            StageListUnsorted.Add(TempStage);
                                            Stage.Clear();
                                            break;
                                        }

                                    }
                                }
                                if (CountM == 0)
                                {
                                    Stage.Add(i);
                                    Stage.Add(j);
                                    Stage.Add(k);
                                    TempStage = ObjectCopier.Clone(Stage);
                                    StageListUnsorted.Add(TempStage);
                                    Stage.Clear();
                                    break;
                                }
                            }
                        }
                        if (CountK == 0)
                        {
                            Stage.Add(i);
                            Stage.Add(j);
                            TempStage = ObjectCopier.Clone(Stage);
                            StageListUnsorted.Add(TempStage);
                            Stage.Clear();
                            break;
                        }
                    }
                }

            }
            //The following code is for testing
            /*int Count2PhaseStages = 0;
            int MaxNoPhases = 0;
            foreach (List<int> item in StageListUnsorted)
            {
                foreach (int phases in item)
                {
                    Console.Write(phases);
                }
                Console.WriteLine("");
                if (item.Count() == 2)
                {
                    Count2PhaseStages++;
                }
                if (item.Count() > MaxNoPhases)
                {
                    MaxNoPhases = item.Count();
                }
            }
            Console.WriteLine("The number of stages are: " + StageListUnsorted.Count());
            Console.WriteLine("The number of stages with at least 3 phases are: " + (StageListUnsorted.Count() - Count2PhaseStages));
            Console.WriteLine("The maximum number of phases are: " + MaxNoPhases);
            
            Console.Read();*/

            /*Console.Write("The number of possible stages are: " + StageListUnsorted.Count());
            Console.Read();*/

            /*int Count = 0;
            foreach (List<int> Stages in StageListUnsorted)
            {
                Count++;
                foreach (int Phase in Stages)
                {
                    Console.Write(Convert.ToString(Phase + 1) + ",");
                }
                Console.WriteLine("....." + Convert.ToString(Count));
            }
            Console.Read();*/

            /*StreamWriter sw;
            //sw = new StreamWriter(@"C:\Dropbox\LanchesterRouteChoiceExperiment - Andrew's\TestBed\ConsoleApplication1\ConsoleApplication1\bin\Debug\TestFile.txt");   //Laptop version
            sw = new StreamWriter(@"C:\Dropbox\Dropbox\LanchesterRouteChoiceExperiment - Andrew's\TestBed\ConsoleApplication1\ConsoleApplication1\bin\Debug\TestFile3.txt");   //Desktop version

            int Counter = 0;
            foreach (List<int> Stages in StageListUnsorted)
            {
                Counter++;
                foreach (int Phase in Stages)
                {
                    sw.Write("," + Convert.ToString(Phase + 1));
                }
                //sw.WriteLine("....." + Convert.ToString(Counter));
                sw.WriteLine();
            }
            sw.Close();*/

            return StageListUnsorted;
        }

        /// <summary>
        /// This code simply returns the stage number with the highest combined bid
        /// </summary>
        /// <param name="Stages"></param>
        /// <returns></returns>
        public int SimpleHighestStage(List<List<int>> Stages, List<double> PhaseList)
        {
            //List<int> PhaseList = PopulatePhases(); //NB. That this is where the delay values come from
            /*List<int> Results = new List<int>();
            List<List<int>> BestStages = new List<List<int>>();
            List<double> BestValues = new List<double>();*/

            List<int> BestStageNumbers = new List<int>();
            int CounterForHighestValue = -1;
            double MaxStageValue = 0;
            int StageNo = 0;
            int Counter = 0;

            foreach (List<int> Stage in Stages)
            {
                Counter++;
                List<int> HighestStage = new List<int>();
                double TempStageValue = 0;

                foreach (int phase in Stage)
                {
                    double TempPhaseValue = 0;
                    TempPhaseValue = PhaseList[phase];
                    TempStageValue = TempStageValue + TempPhaseValue;
                }
                //Console.WriteLine(TempStageValue);
                if (TempStageValue >= MaxStageValue)
                {
                    MaxStageValue = TempStageValue;
                    BestStageNumbers.Add(Counter);
                    //BestStages.Add(Stage);
                    //BestValues.Add(TempStageValue);
                    CounterForHighestValue++;
                }
            }
            StageNo = BestStageNumbers[CounterForHighestValue];
            
            /*Results = BestStages[CounterForHighestValue];
            Console.WriteLine("Highest Value is = " + BestValues[CounterForHighestValue]);

            foreach (int item in Results)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Number of Conflicts in this stage = " + HowManyConflicts(BestStages[0]));

            Console.Read();
            */
            //return Results;
            return StageNo;
        }
        /// <summary>
        /// This returns the highest stage unless the new stage is only X percent better than the previous one
        /// </summary>
        /// <param name="Stages"></param>
        /// <param name="PhaseList"></param>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public int WithinXPercentStage(List<List<int>> Stages, List<double> PhaseList, int PreviousStage, double Percent)
        {
            //List<int> PhaseList = PopulatePhases(); //NB. That this is where the delay values come from
            /*List<int> Results = new List<int>();
            List<List<int>> BestStages = new List<List<int>>();*/
            List<double> BestValues = new List<double>();
            double PreviousStageValue = 0;

            List<int> BestStageNumbers = new List<int>();
            int CounterForHighestValue = -1;
            double MaxStageValue = 0;
            int StageNo = 0;
            int Counter = 0;

            foreach (List<int> Stage in Stages)
            {
                Counter++;
                List<int> HighestStage = new List<int>();
                double TempStageValue = 0;

                foreach (int phase in Stage)
                {
                    double TempPhaseValue = 0;
                    TempPhaseValue = PhaseList[phase];
                    TempStageValue = TempStageValue + TempPhaseValue;
                }
                if (Counter == PreviousStage)
                {
                    PreviousStageValue = TempStageValue;
                }
                //Console.WriteLine(TempStageValue);
                if (TempStageValue >= MaxStageValue)
                {
                    MaxStageValue = TempStageValue;
                    BestStageNumbers.Add(Counter);
                    BestValues.Add(TempStageValue);
                    //BestStages.Add(Stage);
                    //BestValues.Add(TempStageValue);
                    CounterForHighestValue++;
                }
            }
            if (BestValues[CounterForHighestValue]/PreviousStageValue <= (1 + (Percent/100)))
            {
                StageNo = PreviousStage;
            }
            else
            {
                StageNo = BestStageNumbers[CounterForHighestValue];
            }
            

            /*Results = BestStages[CounterForHighestValue];
            Console.WriteLine("Highest Value is = " + BestValues[CounterForHighestValue]);

            foreach (int item in Results)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Number of Conflicts in this stage = " + HowManyConflicts(BestStages[0]));

            Console.Read();
            */
            //return Results;
            return StageNo;
        }

        public List<List<int>> StagesGenerator(string Filename)     //NB. the phase numbers will be 0-11
        {
            List<List<int>> Results = new List<List<int>>();
            string Line;

            // Read the file and display it line by line.
            System.IO.StreamReader File = new System.IO.StreamReader(Filename);

            while ((Line = File.ReadLine()) != null)
            {
                List<int> TempPhaseList = new List<int>();
                foreach (string phase in Line.Split(','))
                {
                    if (phase != "")
                    {
                        TempPhaseList.Add(Convert.ToInt16(phase) - 1);   //NB. the "-1" is to my phase numbers range from 0-11
                    }
                }
                Results.Add(TempPhaseList);
            }
            File.Close();
            /*foreach (List<int> stage in Results)
            {
                foreach (int phase in stage)
                {
                    Console.Write(Convert.ToString(phase) + ",");
                }
                Console.WriteLine();
            }
            Console.Read();*/
            return Results;
        }

        /// <summary>
        /// This code returns the highest delay value stage without any conflicts
        /// </summary>
        /// <param name="Stages"></param>
        /// <returns></returns>
        public List<int> AvoidingConflictsStage(List<List<int>> Stages)
        {
            List<double> PhaseList = PopulatePhases(); //NB. That this is where the delay values come from
            List<int> Results = new List<int>();
            List<List<int>> BestStages = new List<List<int>>();
            List<double> BestValues = new List<double>();
            int CounterForHighestValue = -1;
            double MaxStageValue = 0;

            foreach (List<int> Stage in Stages)
            {
                if (HowManyConflicts(Stage) == 0)
                {
                    List<int> HighestStage = new List<int>();
                    double TempStageValue = 0;

                    foreach (int phase in Stage)
                    {
                        double TempPhaseValue = 0;
                        TempPhaseValue = PhaseList[phase];
                        TempStageValue = TempStageValue + TempPhaseValue;
                    }
                    //Console.WriteLine(TempStageValue);

                    if (TempStageValue >= MaxStageValue)
                    {
                        MaxStageValue = TempStageValue;
                        BestStages.Add(Stage);
                        BestValues.Add(TempStageValue);
                        CounterForHighestValue++;
                    }
                }
            }
            Results = BestStages[CounterForHighestValue];
            Console.WriteLine("Highest Value is = " + BestValues[CounterForHighestValue]);
            Console.WriteLine("The following phases should be called in the next stage:");

            foreach (int item in Results)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("Number of Conflicts in this stage = " + HowManyConflicts(BestStages[0]));

            Console.Read();

            return Results;
        }



        public int HowManyConflicts(List<int> stage)
        {
            //ReturnAllCrossroadStages();
            int Counter = 0;
            //int Counter2 = 0;

            if ((stage.Contains(0) && stage.Contains(7)))
            {
                Counter++;
            }
            if ((stage.Contains(0) && stage.Contains(8)))
            {
                Counter++;
            }
            if ((stage.Contains(1) && stage.Contains(6)))
            {
                Counter++;
            }
            if ((stage.Contains(2) && stage.Contains(6)))
            {
                Counter++;
            }
            if ((stage.Contains(3) && stage.Contains(10)))
            {
                Counter++;
            }
            if ((stage.Contains(3) && stage.Contains(11)))
            {
                Counter++;
            }
            if ((stage.Contains(4) && stage.Contains(9)))
            {
                Counter++;
            }
            if ((stage.Contains(5) && stage.Contains(9)))
            {
                Counter++;
            }
            return Counter;
        }

        /*private bool[,] PopulateAllowableConflictsList()
        {
            bool[,] Combinations = new bool[12, 12]; //(Rows,Columns)

            //One is row one, Two is row 2, etc
            bool[] One = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Two = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Three = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Four = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Five = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Six = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Seven = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Eight = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Nine = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Ten = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Eleven = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };
            bool[] Twelve = new bool[12] { false, false, false, false, false, false, false, false, false, false, false, false };


            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    switch (i)
                    {
                        case (0):
                            Combinations[i, j] = One[j];
                            break;
                        case (1):
                            Combinations[i, j] = Two[j];
                            break;
                        case (2):
                            Combinations[i, j] = Three[j];
                            break;
                        case (3):
                            Combinations[i, j] = Four[j];
                            break;
                        case (4):
                            Combinations[i, j] = Five[j];
                            break;
                        case (5):
                            Combinations[i, j] = Six[j];
                            break;
                        case (6):
                            Combinations[i, j] = Seven[j];
                            break;
                        case (7):
                            Combinations[i, j] = Eight[j];
                            break;
                        case (8):
                            Combinations[i, j] = Nine[j];
                            break;
                        case (9):
                            Combinations[i, j] = Ten[j];
                            break;
                        case (10):
                            Combinations[i, j] = Eleven[j];
                            break;
                        case (11):
                            Combinations[i, j] = Twelve[j];
                            break;

                    }
                }
            }
            return Combinations;
        }*/

        private bool[,] PopulateTJunctionList()
        {
            bool[,] Combinations = new bool[6, 6]; //(Rows,Columns) for a T-junction
            //One is row one, Two is row 2, etc
            bool[] One = new bool[6] { false, true, true, false, true, true };
            bool[] Two = new bool[6] { true, false, true, false, true, true };
            bool[] Three = new bool[6] { true, true, false, true, true, false };
            bool[] Four = new bool[6] { false, false, true, false, true, false };
            bool[] Five = new bool[6] { true, true, true, true, false, true };
            bool[] Six = new bool[6] { true, true, false, false, true, false };
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    switch (i)
                    {
                        case (0):
                            Combinations[i, j] = One[j];
                            break;
                        case (1):
                            Combinations[i, j] = Two[j];
                            break;
                        case (2):
                            Combinations[i, j] = Three[j];
                            break;
                        case (3):
                            Combinations[i, j] = Four[j];
                            break;
                        case (4):
                            Combinations[i, j] = Five[j];
                            break;
                        case (5):
                            Combinations[i, j] = Six[j];
                            break;
                    }
                }
            }
            return Combinations;
        }


        public void ReturnAllTJunctionStages()
        {
            bool[,] Combinations = PopulateTJunctionList();
            List<int> Stage = new List<int>();
            List<int> TempStage = new List<int>();

            int CountK = 0;
            int CountM = 0;
            int CountN = 0;
            int CountP = 0;
            int CountQ = 0;
            int CountR = 0;
            int CountS = 0;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (Combinations[i, j] && j > i) //If level One is true, then try more levels
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            CountK = 0;
                            if (Combinations[i, k] && Combinations[j, k] && k > j)  //If level Two is true, and the new phase number is higher than previous phase number
                            {
                                CountK++;
                                for (int m = 0; m < 6; m++) //Try another level, 'm' is used because 'l' looks like the number one.
                                {
                                    CountM = 0;
                                    if (Combinations[i, m] && Combinations[j, m] && Combinations[k, m] && m > k) // This checks for a four stage sequence
                                    {
                                        CountM++;
                                        for (int n = 0; n < 6; n++)
                                        {
                                            CountN = 0;
                                            if (Combinations[i, n] && Combinations[j, n] && Combinations[k, n] && Combinations[m, n] && n > m)
                                            {
                                                CountN++;
                                                for (int p = 0; p < 6; p++)
                                                {
                                                    CountP = 0;
                                                    if (Combinations[i, p] && Combinations[j, p] && Combinations[k, p] && Combinations[m, p] && Combinations[n, p] && p > n)
                                                    {
                                                        CountP++;
                                                        for (int q = 0; q < 6; q++)
                                                        {
                                                            CountQ = 0;
                                                            if (Combinations[i, q] && Combinations[j, q] && Combinations[k, q] && Combinations[m, q] && Combinations[n, q] && Combinations[p, q] && q > p)
                                                            {
                                                                CountQ++;
                                                                for (int r = 0; r < 6; r++)
                                                                {
                                                                    CountR = 0;
                                                                    if (Combinations[i, r] && Combinations[j, r] && Combinations[k, r] && Combinations[m, r] && Combinations[n, r] && Combinations[p, r] && Combinations[q, r] && r > q)
                                                                    {
                                                                        CountR++;
                                                                        for (int s = 0; s < 6; s++)
                                                                        {
                                                                            CountS = 0;
                                                                            if (Combinations[i, s] && Combinations[j, s] && Combinations[k, s] && Combinations[m, s] && Combinations[n, s] && Combinations[p, s] && Combinations[q, s] && Combinations[r,s] && s > r)
                                                                            {
                                                                                CountS++;
                                                                            }
                                                                        }
                                                                        if (CountS == 0)
                                                                        {
                                                                            Stage.Add(i);
                                                                            Stage.Add(j);
                                                                            Stage.Add(k);
                                                                            Stage.Add(m);
                                                                            Stage.Add(n);
                                                                            Stage.Add(p);
                                                                            Stage.Add(q);
                                                                            Stage.Add(r);
                                                                            TempStage = ObjectCopier.Clone(Stage);
                                                                            StageListUnsorted.Add(TempStage);
                                                                            Stage.Clear();
                                                                            break; 
                                                                        }
                                                                    }
                                                                }
                                                                if (CountR == 0)
                                                                {
                                                                    Stage.Add(i);
                                                                    Stage.Add(j);
                                                                    Stage.Add(k);
                                                                    Stage.Add(m);
                                                                    Stage.Add(n);
                                                                    Stage.Add(p);
                                                                    Stage.Add(q);
                                                                    TempStage = ObjectCopier.Clone(Stage);
                                                                    StageListUnsorted.Add(TempStage);
                                                                    Stage.Clear();
                                                                    break;                                                                    
                                                                }
                                                            }
                                                        }
                                                        if (CountQ == 0)
                                                        {
                                                            Stage.Add(i);
                                                            Stage.Add(j);
                                                            Stage.Add(k);
                                                            Stage.Add(m);
                                                            Stage.Add(n);
                                                            Stage.Add(p);
                                                            TempStage = ObjectCopier.Clone(Stage);
                                                            StageListUnsorted.Add(TempStage);
                                                            Stage.Clear();
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (CountP == 0)
                                                {
                                                    Stage.Add(i);
                                                    Stage.Add(j);
                                                    Stage.Add(k);
                                                    Stage.Add(m);
                                                    Stage.Add(n);
                                                    TempStage = ObjectCopier.Clone(Stage);
                                                    StageListUnsorted.Add(TempStage);
                                                    Stage.Clear();
                                                    break;
                                                }

                                            }
                                        }
                                        if (CountN == 0)
                                        {
                                            Stage.Add(i);
                                            Stage.Add(j);
                                            Stage.Add(k);
                                            Stage.Add(m);
                                            TempStage = ObjectCopier.Clone(Stage);
                                            StageListUnsorted.Add(TempStage);
                                            Stage.Clear();
                                            break;
                                        }

                                    }
                                }
                                if (CountM == 0)
                                {
                                    Stage.Add(i);
                                    Stage.Add(j);
                                    Stage.Add(k);
                                    TempStage = ObjectCopier.Clone(Stage);
                                    StageListUnsorted.Add(TempStage);
                                    Stage.Clear();
                                    break;
                                }
                            }
                        }
                        if (CountK == 0)
                        {
                            Stage.Add(i);
                            Stage.Add(j);
                            TempStage = ObjectCopier.Clone(Stage);
                            StageListUnsorted.Add(TempStage);
                            Stage.Clear();
                            break;
                        }
                    }
                }

            }
            foreach (List<int> item in StageListUnsorted)
            {
                foreach (int phases in item)
                {
                    Console.Write(phases);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("The number of stages are: " + StageListUnsorted.Count());

            Console.Read();
        }

    }
}


/*foreach (List<int> stage in StageListUnsorted)
{//Note the '!' to make it a search for all stages which don't have any conflict movements.
    if (!(stage.Contains(0) && stage.Contains(7) || stage.Contains(0) && stage.Contains(8) || stage.Contains(1) && stage.Contains(6) || stage.Contains(2) && stage.Contains(6) || stage.Contains(3) && stage.Contains(10) || stage.Contains(3) && stage.Contains(11) || stage.Contains(4) && stage.Contains(9) || stage.Contains(5) && stage.Contains(9)))
    {
        //Counter++;
        //Console.WriteLine("True" + Counter);
        if (stage.Count() > 2)
        {
            Counter2++;
            foreach (int item in stage)
            {
                Console.Write(item);
            }
            Console.WriteLine("   " + Counter2);
        }
    }
}
Console.Read();*/