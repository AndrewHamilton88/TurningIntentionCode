using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ParamincsSNMPcontrol
{
    class FinalFunction
    {
        private List<double[]> CopyRoadState(List<double[]> RoadState)
        {
            List<double[]> Returner = new List<double[]>();
            foreach (double[] item in RoadState)
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

        public double RunnerFunction(List<int[]> CyclePlan, double LeastDelay, List<double[]> CurrentRoadState, List<int[]> PhaseList)
        {
            Performance Perf = new Performance();
            Queue_Lengths Queue = new Queue_Lengths();

            double TempDelayTotal = 0;
            List<double[]> TempRoadState = new List<double[]>();
            //TempRoadState = CopyRoadState(CurrentRoadState);          //This does not seem to deep clone the object!!
            //TempRoadState = ObjectCopier.Clone(CurrentRoadState);
            TempRoadState = Clone(CurrentRoadState);
            //TempRoadState = CurrentRoadState;                         //This does not deep clone the object!!
            

            foreach (int[] Stage in CyclePlan)
            {
                //if (TempDelayTotal <= LeastDelay)
                if (true)
                {
                    /*TempDelayTotal += Perf.DelayFunctionOtherStages(Stage[0], Stage[1], TempRoadState, LeastDelay, PhaseList);   //Determines 'off' stages's delay  
                    TempRoadState = Queue.UpdateQueueLength(Stage[0], Stage[1], TempRoadState, PhaseList);     //Updates current queues
                    TempDelayTotal += Perf.DelayFunctionCurrentStage(Stage[0], Stage[1], TempRoadState, PhaseList);   //Calculates the delay to the remaining queued vehicles on current stage*/

                    TempDelayTotal += Perf.DelayFunctionOtherStagesVer2(Stage[0], Stage[1], TempRoadState, LeastDelay, PhaseList);   //Determines 'off' stages's delay  
                    TempDelayTotal += Perf.DelayFunctionCurrentStageVer2(Stage[0], Stage[1], TempRoadState, PhaseList);   //Calculates the delay to the remaining queued vehicles on current stage
                    TempRoadState = Queue.UpdateQueueLength(Stage[0], Stage[1], TempRoadState, PhaseList);     //Updates current queues
                }
                else
                {
                    return 9999999999;
                }
            }
            return TempDelayTotal;
        }


        public List<int[]> TestFunction()
        {
            List<int[]> Answer = new List<int[]>();
            int[] Stage1 = new int[2];
            int[] Interstage = new int[2];

            Stage1[0] = 1;
            Stage1[1] = 25;
            Interstage[0] = 99;
            Interstage[1] = 5;

            Answer.Add(Stage1);
            Answer.Add(Interstage);
            Answer.Add(Stage1);
            Answer.Add(Interstage);
            Answer.Add(Stage1);
            Answer.Add(Interstage);
            Answer.Add(Stage1);
            Answer.Add(Interstage);
            return Answer;
        }
    }
}
