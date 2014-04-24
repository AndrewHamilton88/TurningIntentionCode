using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ParamincsSNMPcontrol
{
    class Mutate
    {
        int NumberOfStages = FixedVariables.NumberOfStages;
        int MinimumGreenTime = FixedVariables.MinimumGreenTime;
        int IntergreenTime = FixedVariables.IntergreenTime;
        int MaximumGreenTime = FixedVariables.MaximumGreenTime;
        int IntergreenStageNumber = FixedVariables.IntergreenStageNumber;
        static int MaxCycleTime = FixedVariables.MaxCycleTime;
        int NumberOfPhases = FixedVariables.NumberOfPhases;

        List<int[]> InitialCyclePlanList = new List<int[]>();
        Random RandomGenerator = new Random();

        private List<int[]> Copy(List<int[]> CyclePlan)
        {
            List<int[]> Returner = new List<int[]>();
            foreach (int[] item in CyclePlan)
            {
                Returner.Add(item);
            }
            return Returner;
        }

        private static T Clone<T>(T source)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        class Utility<T> where T : ICloneable
        {
            static public IEnumerable<T> CloneList(List<T> tl)
            {
                foreach (T t in tl)
                {
                    yield return (T)t.Clone();
                }
            }
        }

        private int RandomStageNumber()
        {
            return RandomGenerator.Next(1, NumberOfStages + 1);   //This returns a random stage number
        }

        public bool AllowableCyclePlanStages(List<int[]> CyclePlan)
        {
            List<int> ListOfStages = new List<int>();
            foreach (int[] Stage in CyclePlan)
            {
                ListOfStages.Add(Stage[0]);
            }
            int StageCounter = 0;
            for (int i = 1; i < NumberOfStages + 1; i++)
            {
                if (ListOfStages.Contains(i))
                {
                    StageCounter++;
                }
                else
                {
                    return false;
                }
            }
            if (StageCounter == NumberOfStages)
            {
                return true;
            }
            return false;
        }

        public bool AllowableCyclePlanPhases(List<int[]> CyclePlan, List<int[]> PhaseList)        //This ensures that every phase has been called
        {
            List<int> ListOfStages = new List<int>();           //This is a list of stages called during the given cycle plan
            List<int> ListOfPhases = new List<int>();           //This is a list of phases called during the given cycle plan
            foreach (int[] Stage in CyclePlan)
            {
                if (Stage[0] != IntergreenStageNumber)
                {
                    if (!ListOfStages.Contains(Stage[0]))
                    {
                        ListOfStages.Add(Stage[0]);
                    }
                }
            }

            foreach (int Stage in ListOfStages)
            {
                foreach (int Phase in PhaseList[Stage - 1])      //This looks through each stage in the ListOfStages and adds the corresponding phases
                {
                    if (!ListOfPhases.Contains(Phase))
                    {
                        ListOfPhases.Add(Phase);
                    }
                }
            }

            if (ListOfPhases.Count() == NumberOfPhases)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int[]> MutateCyclePlan(List<int[]> ReceivedCyclePlan)
        {
            List<int[]> InitialCyclePlan = Clone(ReceivedCyclePlan);
            //List<int[]> InitialCyclePlan = ReceivedCyclePlan;
            int NumberOfStages = InitialCyclePlan.Count() / 2;              //This is divided by two otherwise it would count the intergreen stage as well
            int MutateNumber = RandomGenerator.Next(1, NumberOfStages + 1);     //This is the position number of the stage which will be mutated
            //System.Threading.Thread.Sleep(15);
            int CurrentStageLength = InitialCyclePlan[2 * MutateNumber - 2][1];     // [(StageNumber, StageLength)...] but want the altnerate numbers to avoid the intergreen period
            int CurrentStageNumber = InitialCyclePlan[2 * MutateNumber - 2][0];
            int NewStageLength;
            int Difference = 0;

            int MutateOption = RandomGenerator.Next(0, 2);              //This generates a random number of 0 or 1 so we can choose whether to alter the times or the stage number
            //System.Threading.Thread.Sleep(15);
            //Console.Write(MutateOption);
            //int MutateOption = 0;

            if (MutateOption == 0)
            {

                if (CurrentStageLength == MinimumGreenTime)
                {
                    NewStageLength = MinimumGreenTime + 1;
                    Difference = 1;
                }
                if (CurrentStageLength == MaximumGreenTime)
                {
                    NewStageLength = MaximumGreenTime - 1;
                    Difference = -1;
                }
                else
                {
                    Difference += RandomGenerator.Next(0, 2) * 2 - 1;           //This generates a random number of either -1 and +1
                    NewStageLength = CurrentStageLength + Difference;
                }

                if (NumberOfStages > 1)             //Mutation for an entire cycle plan [(1,10),(99,5),(2,15),(99,5)...]
                {
                    int OtherStageLength;
                    int TempCounter = 0;

                    while (TempCounter < 10)        //The function trials up to 10 other stages to include the 'difference' into the cycle plan
                    {
                        int RandomOtherNumber = RandomGenerator.Next(1, NumberOfStages + 1);
                        if (RandomOtherNumber != MutateNumber)                  //To ensure that you do not randomly select the stage you have already selected
                        {
                            OtherStageLength = InitialCyclePlan[2 * RandomOtherNumber - 2][1];
                            OtherStageLength -= Difference;                     //The difference must be subtracted to ensure the cycle plan has the correct time.

                            if (!(OtherStageLength < MinimumGreenTime || OtherStageLength > MaximumGreenTime))
                            {
                                //List<int[]> FinalCyclePlanList = Copy(InitialCyclePlan);
                                InitialCyclePlan[2 * MutateNumber - 2][1] = NewStageLength;
                                InitialCyclePlan[2 * RandomOtherNumber - 2][1] = OtherStageLength;
                                return InitialCyclePlan;
                            }
                        }
                        TempCounter++;
                    }
                    return InitialCyclePlan;      //If no adjustment can be made then the Initial Cycle Plan is returned
                }
                else     //If there is just one stage in the initial cycle plan [(1,15),(99,5)] then the mutated stage length will be returned
                {
                    //List<int[]> FinalCyclePlanList = Copy(InitialCyclePlan);
                    InitialCyclePlan[2 * MutateNumber - 2][1] = NewStageLength;
                    return InitialCyclePlan;
                }
            }
            else                                                       //This is for altering the stage number order!
            {
                int TempCounter = 0;
                while (TempCounter < 10)
                {
                    int NewStageNumber = RandomStageNumber();               //This is to add a new stage number instead of the current one (it is allowed to be the same number as the previous or future stage)
                    if (NewStageNumber != CurrentStageNumber)
                    {
                        //List<int[]> FinalCyclePlanList = Copy(InitialCyclePlan);
                        InitialCyclePlan[2 * MutateNumber - 2][0] = NewStageNumber;
                        return InitialCyclePlan;
                    }
                    TempCounter++;
                }

                return InitialCyclePlan;
            }
            
        }



    }
}
