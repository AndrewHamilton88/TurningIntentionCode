using System;
using System.Collections.Generic;
using System.Text;
using ParamicsSNMP2007;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using NeuralLib;


namespace ParamincsSNMPcontrol
{
    public class Runner
    {
        //*Class mambers
        public SnmpConnect Connector;
        public bool SynchDone;
        public bool ExitSim;
        public int ModelTimeofDay;
        Coordinate Processor;

        //*Class Constructor
        public Runner(Coordinate B)
        {
            Processor = B;
            Connector = Processor.NetDat;
        }

        //*Functions for running and pusing the simulation

        public void Run()
        {
            Connector.PCont.SimRun();
        }

        public void Pause()
        {
            Connector.PCont.SimPause();
        }
        public void SynchRun(int Duration)
        {
            int SynchDelay = 1000;      //This is in centi-seconds - ie. 10 second delay is 1000.
            //Connector.PCont.SetModelSyncTrapRate(ref SynchDelay);     //AH moved this to within the 'do' loop to ensure we can vary the time

            Connector.PCont.OnSynch += new __PController_OnSynchEventHandler(PCont_OnSynch);

            ExitSim = false;

            /*int[] LastStageList = new int[Processor.MainZone.Junctions.Count];
            for (int i = 0; i < LastStageList.Length; i++)
            {
                LastStageList[i] = 1;
            }*/
            
            
            //int iterations = 0;
            int iterations = 10;
            int[] PreviousStage = new int[1];
            PreviousStage[0] = 1;

            //AH copied this bit of code so that the first timestep would still work!
            Connector.PCont.SetModelSyncTrapRate(ref SynchDelay);       
            SynchDone = false;
            Connector.PCont.SimRunAndLogAndPause();
            while (SynchDone == false) ;
            Thread.Sleep(1000);//TODO This is to give paramics time to save the snapfile should make this explicit in the future

            FixedVariables FV = new FixedVariables();
            int IntergreenTime = FV.IntergreenTimeVariable;

            do
            {
                List<int[]> Jstages = Processor.EvaluationProcess(ModelTimeofDay, PreviousStage);  //AH this is where the next stage is determined for each junction

                foreach (int[] StageAndLength in Jstages)
                {
                    Processor.SigSet.SetStages(StageAndLength[0], 0);       //Junction 0 is the junction number for the single junction
                    PreviousStage[0] = StageAndLength[0];                      //This is so that when the calculations are made, the current stage is known
                    SynchDelay = (StageAndLength[1] + IntergreenTime) * 100;                         //This is how long the stage will run for multiplied by 100 to express it in centi-seconds

                    Connector.PCont.SetModelSyncTrapRate(ref SynchDelay);
                    SynchDone = false;
                    Connector.PCont.SimRunAndLogAndPause();
                    while (SynchDone == false) ;
                    //Thread.Sleep(1000);//TODO This is to give paramics time to save the snapfile should make this explicit in the future

                    //Trial to make any simulation time work...
                    iterations += (SynchDelay / 100);       //NB. SynchDelay is recorded in Centi-seconds
                    //NB. That I have commented out the iterations++ below for this to work
                }
                
                //This function needs to be editted to be able to use a cycle plan rather than a single stage number...
                //int[] Jstages = Processor.EvaluationProcess(ModelTimeofDay, PreviousStage);  //AH this is where the next stage is determined for each junction

                /*for (int i = 0; i < Jstages.Length; i++)
                {
                    /*if (LastStageList[i] != Jstages[i])
                    {
                        LastStageList[i] = Jstages[i];*/
                        /*Processor.SigSet.SetStages(Jstages[i], i);
                        PreviousStage = Jstages;
                    //}
                }*/
                if (iterations >= Duration)
                {
                    ExitSim = true;
                }
                //iterations++;
                

            } while (ExitSim == false);
            //Connector.PCont.Disconnect();
            Connector.PCont.Kill();
        }

       
        public void RunAlarm()
        {
            string ToD = "modeltimeofday";
            int arg = 0;
            string TriggerID = Connector.PCont.AddIndex(ref ToD, ref arg, ref arg, ref arg, ref arg);
            
        }
        void PCont_OnSynch(int modeldayofweek, int modeltimeofday, ref string ModelDescription)
        {
            ModelTimeofDay = modeltimeofday;
            SynchDone = true;
            
        }
        void ExitTrigger()
        {
            Console.WriteLine("Simulation looping, press return at any time to exit");
            Console.Read();
            ExitSim = true;
        }


    }
}
