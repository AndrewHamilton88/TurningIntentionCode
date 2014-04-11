using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mapack;
using ParamicsSNMPcontrol;

namespace ParamincsSNMPcontrol
{
    //*Abstract base class for signal control strategies
    public abstract class Strategies
    {
        public abstract void ProcessLane(LaneAgent LA);
        //public abstract void ProcessLaneTurns(LaneAgent LA, int[] Turns);       //AH added
        public abstract void ProcessStage(StageAgent SA);
        public abstract void ProcessJunction(JunctionAgent JA, int[] PreviousStage);
        public abstract void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage);

        public class Bid
        {
            public int RoundNum;
            public double Scalar;
            public double[] TurningBids = new double[3];       //AH added
        }

    }


    //*Basic Strategy
    public class BasicStrategy : Strategies
    {
        public override void ProcessLane(LaneAgent LA)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        /*public override void ProcessLaneTurns(LaneAgent LA, int[] Turns)
        {
            throw new Exception("The method or operation is not implemented2.");
        }*/
        public override void ProcessStage(StageAgent SA)
        {
            SA.LanePhases.Clear();
            //test that number of lanes and weights is the same
            if (SA.Lanes.Count != SA.Weights.Count)
            {
                throw (new Exception("The number of weights does not match the number of lanes"));
            }

            Bid B = new Bid();
            //double pBid = 0;
            int Lcount = SA.Lanes.Count;

            for (int i = 0; i < Lcount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    SA.LanePhases.Add(SA.Lanes[i].LaneBid.TurningBids[j]);         //AH - Not considering Weights
                    for (int k = 0; k < 3; k++)
                    {
                        SA.RoadStates[j, k] = SA.Lanes[i].RoadState[j, k];          //This makes the assumption that there is only one lane!
                    }
                }
                //pBid += SA.Lanes[i].LaneBid.Scalar * SA.Weights[i];    
            }



            //B.Scalar = pBid;
            //Console.WriteLine(pBid);
            //SA.StageBid = B;
        }
        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            //test that number of stages and agents match
            JA.AllRoadStates.Clear();
            if (JA.Stages.Count != JA.NoOfStages)
            {
                throw (new Exception("The number of stages does not match the number of agents"));
            }

            for (int i = 0; i < JA.Stages.Count; i++)
            {
                JA.AllRoadStates.Add(JA.Stages[i].RoadStates);
            }

            int WinningStage = 1;

            //This is where AH adds his stage selector...
            AllStages AS = new AllStages();
            List<List<int>> AllPossibleStages = new List<List<int>>();

            //Number of Stages - Reads a file to generate stage list
            //AllPossibleStages = AS.ReturnAllCrossroadStages();
            //AllPossibleStages = AS.StagesGenerator(@"OptimalCrossroad17Stages.txt");   //This is for the 17 stage option
            //AllPossibleStages = AS.StagesGenerator(@"OptimalCrossroad16Stages.txt");   //This is for the 16 stage option
            //AllPossibleStages = AS.StagesGenerator(@"OptimalCrossroad12Stages.txt");   //This is for the 12 stage option
            //AllPossibleStages = AS.StagesGenerator(@"OptimalCrossroad8Stages.txt");   //This is for the 8 stage option
            //Console.WriteLine(PreviousStage);

            //Four stage road state model [Stage1RoadState, State2RoadState...]
            List<double[]> FourStageRoadStates = new List<double[]>();
            FourStageRoadStates = ReturnFourStageSolution(JA.AllRoadStates);

            //12 Phase road state model [Phase1RoadState, Phase2RoadState...]
            //List<double[]> TwelvePhaseRoadStates = new List<double[]>();
            //TwelvePhaseRoadStates = Return12PhaseRoadState(JA.AllRoadStates);

            List<int[]> CyclePlan = new List<int[]>();
            CyclePlan.Clear();

            FixedVariables FV = new FixedVariables();
            RunnerCyclePlan RunCyclePlan = new RunnerCyclePlan();

            CyclePlan = RunCyclePlan.RunAlgorithm(FV.StartingSeeds, FV.StepsClimbed, FV.MutationsAroundAPoint, FourStageRoadStates);

            List<int[]> FinalAnswer = new List<int[]>();        //This is to remove the (99,5) Intergreen Phase
            FinalAnswer.Clear();

            FinalAnswer = ReturnCyclePlan(CyclePlan);

            JA.NextStage = FinalAnswer;
            

            //This all works for the highbid model...
            /*int LastStage = 2;
            LastStage = PreviousStage[0];

            //Stage Choice - Select the 'logic' for choosing the best stage
            //Console.WriteLine(LastStage);
            //WinningStage = AS.WithinXPercentStage(AllPossibleStages, JA.AllPhases, LastStage, 15.0);   //This is for the within "X" percent bid

            WinningStage = AS.SimpleHighestStage(AllPossibleStages, JA.AllPhases);                   //This is for the basic highest bid
            
            List<int[]> Answer = new List<int[]>();
            int[] SingleStage = new int[2];
            int StageLength = 10;
            SingleStage[0] = WinningStage;
            SingleStage[1] = StageLength;
            Answer.Add(SingleStage);

            JA.NextStage = Answer;

            /*int Stage = 1;
            double BidHolder = 0;
            foreach (StageAgent SA in JA.Stages)
            {
                Bid B = SA.StageBid;
                if (B.Scalar > BidHolder)               //This is Simon's original stage selection process
                {
                    WinningStage = Stage;
                    BidHolder = B.Scalar;
                }
                Stage++;
            }*/

            //JA.NextStage = WinningStage;*/
        }
        public override void ProcessZone(ZoneAgent ZA,int ToD, int[] PreviousStage)
        {
            GreedyNextStages(ZA);
        }

        private void GreedyNextStages(ZoneAgent ZA)
        {
            //int[] NextStages = new int[ZA.Junctions.Count];
            List<int[]> NextStages = new List<int[]>();     //Note that I have removed ZA.Junctions.Count!!!
            
            for (int i = 0; i < ZA.Junctions.Count; i++)
            {
                NextStages = ZA.Junctions[i].NextStage;
            }
            ZA.NextStages = NextStages;
        }

        private List<double[]> ReturnFourStageSolution(List<double[,]> AllRoadStates)
        {
            List<double[]> Answer = new List<double[]>();

            foreach (double[,] Stage in AllRoadStates)
            {
                double[] TempAnswer = new double[3];
                TempAnswer[0] = Stage[0, 0] + Stage[1, 0] + Stage[2, 0];          //Queue length is the sum of all queues
                TempAnswer[1] = Stage[0, 1] + Stage[1, 1] + Stage[2, 1];          //Arrival rate is the sum of all arrivals
                TempAnswer[2] = Stage[0, 2] + Stage[1, 2] + Stage[2, 2];        //Discharge rate is the sum of all discharges if there are 3 lanes
                Answer.Add(TempAnswer);
            }
            return Answer;
        }

        private List<double[]> Return12PhaseRoadState(List<double[,]> AllRoadStates)
        {
            List<double[]> Answer = new List<double[]>();

            foreach (double[,] Stage in AllRoadStates)
            {
                double[] TempAnswer1 = new double[3];
                double[] TempAnswer2 = new double[3];
                double[] TempAnswer3 = new double[3];
                TempAnswer1[0] = Stage[0, 0];          //Queue length is the sum of all queues
                TempAnswer1[1] = Stage[0, 1];          //Arrival rate is the sum of all arrivals
                TempAnswer1[2] = Stage[0, 2];        //Discharge rate is the sum of all discharges if there are 3 lanes
                TempAnswer2[0] = Stage[1, 0];          //Queue length is the sum of all queues
                TempAnswer2[1] = Stage[1, 1];          //Arrival rate is the sum of all arrivals
                TempAnswer2[2] = Stage[1, 2];        //Discharge rate is the sum of all discharges if there are 3 lanes
                TempAnswer3[0] = Stage[2, 0];          //Queue length is the sum of all queues
                TempAnswer3[1] = Stage[2, 1];          //Arrival rate is the sum of all arrivals
                TempAnswer3[2] = Stage[2, 2];        //Discharge rate is the sum of all discharges if there are 3 lanes
                Answer.Add(TempAnswer1);
                Answer.Add(TempAnswer2);
                Answer.Add(TempAnswer3);
            }
            return Answer;
        }

        private List<int[]> ReturnCyclePlan(List<int[]> CyclePlan)
        {
            List<int[]> Answer = new List<int[]>();
            for (int i = 0; i < CyclePlan.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Answer.Add(CyclePlan[i]);
                }
            }
            return Answer;
        }
    }


    //*Strategy 3 High Bid with Turning Intention
    public class HighBidTurn : BasicStrategy  // AH's turning intention High Bid approach
    {

        public override void ProcessLane(LaneAgent SA)
        {

            Bid B = new Bid();
            for (int i = 0; i < 3; i++)
            {
                if (SA.CountTurns[i] != 0)
                {
                    B.TurningBids[i] = SA.TurningMovements[i] * (1 - 0.01 * SA.AvSpeedTurns[i] - 0.001 * SA.AvDistTurns[i]);
                    //B.TurningBids[i] = SA.TurningMovements[i] * (3 - 0.05 * SA.AvSpeedTurns[i] - 0.001 * SA.AvDistTurns[i]);   //AH Testing different numbers
                }
                else
                {
                    B.TurningBids[i] = 0.0;
                }
            }

            SA.LaneBid = B;

        }

        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA, ToD, PreviousStage);

        }
    }


    //*Strategy 1 Count Stationary vehicles
    /*public class StationaryVehicles : BasicStrategy
    {

        public override void ProcessLane(LaneAgent SA)
        {
            Bid B = new Bid();
            int SVcount = 0;
            foreach (double Speed in SA.SpeedList)
            {
                if (Speed < 2)
                {
                    SVcount++;
                }
            }
            B.Scalar = Convert.ToDouble(SVcount);
            SA.LaneBid = B;
            
        }

        public override void ProcessZone(ZoneAgent ZA,int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA,ToD,PreviousStage);
        } 
    }*/

    //*Strategy 2 High Bid
    /*public class HighBid : BasicStrategy
    {

        public override void ProcessLane(LaneAgent SA)
        {
            
            Bid B = new Bid();

            if (SA.Count != 0)
            {
                B.Scalar = SA.Count*(1 - 0.01 * SA.AvSpeed - 0.001 * SA.AvDist);
            }
            else
            {
                B.Scalar = 0.0;
            }
            SA.LaneBid = B;

        }
        
        public override void ProcessZone(ZoneAgent ZA,int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA,ToD,PreviousStage);
            
        } 
    }*/


    //*Strategy 4 HumanControl
    /*public class Trainer : HighBid
    {
        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
        }

        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            HumanControl Hcont = new HumanControl();

            List<int> NextStages = Hcont.LaunchInterface(ZA.Junctions);

            for (int i = 0; i < NextStages.Count; i++)
            {
                ZA.Junctions[i].NextStage = NextStages[i];
            }
            base.ProcessZone(ZA,ToD, PreviousStage);
        }

    }

    //base strategy for Logit and Neural

    public class MachineLearning : HighBid
    {
        //*class members
        protected double[] Bids;
        protected bool BidsSet;
        protected string[] SigNstrings;
        protected string[] FileNames;
        protected List<ForwardProp> FPL;
        public bool independent;

        public MachineLearning(string[] filenames, string[] SignalNodes)
        {
            independent = false;
            BidsSet = false;
            SigNstrings = SignalNodes;
            FileNames = filenames;
            FPL = new List<ForwardProp>();
            
        }

        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void ProcessStage(StageAgent SA)
        {
            
        }

        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            try
            {
                if (!BidsSet && !independent)
                {
                    throw new Exception("Nothing is wrong");
                }
                int JuncIndex = 0;
                foreach (string SigN in SigNstrings)
                {
                    if (SigN.Equals(JA.SignalNode))
                    {
                        break;
                    }
                    JuncIndex++;
                }
                ForwardProp FPN = FPL[JuncIndex];

                if (independent)
                {
                    List<double> BidList = new List<double>();

                  
                    foreach (StageAgent SA in JA.Stages)
                    {
                        foreach (LaneAgent LA in SA.Lanes)
                        {
                            if (!LA.Duplicate)
                            {
                                BidList.Add(LA.LaneBid.Scalar);
                            }
                        }
                    }
                    
                    Bids = new double[BidList.Count + 1];
                    for (int i = 0; i < BidList.Count; i++)
                    {
                        Bids[i] = BidList[i];
                    }
                    Bids[BidList.Count] = 1;
                }


                double[] Probs = FPN.propagate(Bids);

                //check that the number of probabilities and stages match
                if (Probs.Length != JA.NoOfStages)
                {
                    throw new Exception("The number of probabilities does not match the number of stages");
                }

                int Stage = 1;
                int WinningStage = 1;
                double Probability = 0;
                foreach (double P in Probs)
                {
                    if (P > Probability)
                    {
                        Probability = P;
                        WinningStage = Stage;
                    }
                    Stage++;
                }

                JA.NextStage = WinningStage;
                
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Nothing is wrong"))
                {
                    throw e;
                }

            }
        }

        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            if (!independent)
            {
                List<double> BidList = new List<double>();

                foreach (JunctionAgent JA in ZA.Junctions)
                {
                    foreach (StageAgent SA in JA.Stages)
                    {
                        foreach (LaneAgent LA in SA.Lanes)
                        {
                            if (!LA.Duplicate)
                            {
                                BidList.Add(LA.LaneBid.Scalar);
                            }
                        }
                    }
                }
                Bids = new double[BidList.Count + 1];
                for (int i = 0; i < BidList.Count; i++)
                {
                    Bids[i] = BidList[i];
                }
                Bids[BidList.Count] = 1;
                BidsSet = true;

                foreach (JunctionAgent JA in ZA.Junctions)
                {
                    ProcessJunction(JA, PreviousStage);
                }
                BidsSet = false;
            }

            base.ProcessZone(ZA,ToD, PreviousStage);
        }

    }

    //*Strategy 3 logistc regression
    public class Logit : MachineLearning
    {       
        public Logit(string[] filenames, string[] SignalNodes) : base(filenames,SignalNodes)
        {           
            foreach (string fn in FileNames)
            {
                ForwardProp Temp = new ForwardPropLogit(fn);
                FPL.Add(Temp);
            }
            if (FPL.Count != SigNstrings.Length)
            {
                throw new Exception("The number of Logit weight files provided does not match the number of junction identifiers");
            }
        }


        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void ProcessStage(StageAgent SA)
        {
            base.ProcessStage(SA);
        }
        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            base.ProcessJunction(JA, PreviousStage);
        }
        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA,ToD, PreviousStage);
        }
       
    }

    //Strategy 5 NeuralNetControl
    public class NeuralNet : MachineLearning
    {

        public NeuralNet(string[] filenames, string[] signalnodes)
            : base(filenames, signalnodes)
        {
            foreach (string fn in FileNames)
            {
                ForwardProp Temp = new ForwardPropNeural(fn);
                FPL.Add(Temp);
            }
            if (FPL.Count != SigNstrings.Length)
            {
                throw new Exception("The number of Logit weight files provided does not match the number of junction identifiers");
            }
        }

        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void  ProcessStage(StageAgent SA)
        {
 	         base.ProcessStage(SA);
        }
        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            base.ProcessJunction(JA, PreviousStage);
        }
        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA,ToD, PreviousStage);
        }

        
    }
    //Strategy 6 Temporal difference
    public class TempDiff : NeuralNet
    {
        //class members
        Random FlatRan = new Random();
        int StartLearning;
        double eta;

        //Constructor
        public TempDiff(string[] filenames, string[] signalnodes, int SL, double alpha, double gamma, double lambda, double etaIn, bool Q )
            : base(filenames, signalnodes)
        {
            StartLearning = SL;
            eta = etaIn;
            FPL.Clear();//TODO not very efficient should create common parent class of Neural and TD
            foreach (string fn in FileNames)
            {
                ForwardPropTD Temp = new ForwardPropTD(fn);
                Temp.alpha = alpha;
                Temp.gamma = gamma;
                Temp.lambda = lambda;
                Temp.Qlearn = Q;
                ForwardProp Temp2 = Temp;
                FPL.Add(Temp2);
            }
            if (FPL.Count != SigNstrings.Length)
            {
                throw new Exception("The number of Logit weight files provided does not match the number of junction identifiers");
            }
        }

        //Functions
        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }

        public override void ProcessStage(StageAgent SA)
        {
            base.ProcessStage(SA);
        }

        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            try
            {
                if (!BidsSet && !independent)
                {
                    throw new Exception("Nothing is wrong");
                }
                int JuncIndex = 0;
                foreach (string SigN in SigNstrings)
                {
                    if (SigN.Equals(JA.SignalNode))
                    {
                        break;
                    }
                    JuncIndex++;
                }
                ForwardPropTD FPN = (ForwardPropTD)FPL[JuncIndex];

                if (independent)
                {
                    List<double> BidList = new List<double>();


                    foreach (StageAgent SA in JA.Stages)
                    {
                        foreach (LaneAgent LA in SA.Lanes)
                        {
                            if (!LA.Duplicate)
                            {
                                BidList.Add(LA.LaneBid.Scalar);
                            }
                        }
                    }

                    Bids = new double[BidList.Count + 1];
                    for (int i = 0; i < BidList.Count; i++)
                    {
                        Bids[i] = BidList[i];
                    }
                    Bids[BidList.Count] = 1;
                }

                double[] Probs = FPN.propagate(Bids);

                //check that the number of probabilities and stages match
                if (Probs.Length != JA.NoOfStages)
                {
                    throw new Exception("The number of probabilities does not match the number of stages");
                }             

                //JA.NextStage = eStochasticStrategy(Probs,0.05);
                //JA.NextStage = GreedyStrategy(Probs);
                JA.NextStage = eGreedyStrategy(Probs, eta);
                //JA.NextStage = StochasticGreedy(Probs, 0.1);
                

                LogWeights(FPN,Probs,JA.NextStage,JA);
                
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Nothing is wrong"))
                {
                    throw e;
                }

            }
        }

        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            base.ProcessZone(ZA,ToD, PreviousStage);

            if (ToD >= StartLearning)
            {
                TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
                string TimeOfDay = TS.ToString();    

                for (int i = 0; i < ZA.Junctions.Count; i++)
                {
                    double LifeTime = 0;
                    int Vcount = 0;
                    string Condition = ZA.Junctions[i].BuildLifetimeCondition(TimeOfDay);
                    ZA.NetDat.PDB.GetVLifeTime(Condition, ref LifeTime, ref Vcount);
                    ForwardPropTD FPT = (ForwardPropTD)FPL[i];
                    int WS = ZA.Junctions[i].NextStage;
                    FPT.UpdateWeights(LifeTime, Vcount,WS);
                }
            }

        }

        private void LogWeights(ForwardPropTD FPN, double[] Probs, int NextStage, JunctionAgent JA)
        {
            int i = 0;
            string Fname = "";
            foreach (string s in SigNstrings)
            {
                if (s.Equals(JA.SignalNode))
                {
                    Fname = this.FileNames[i];
                }
                i++;
            }
            RWmatrix Wlog = new RWmatrix(FPN,Fname);
            Wlog.WriteLog(Probs, NextStage,JA.NetDat.Path);
            Wlog.ReWriteWfile();
        }
        private int GreedyStrategy(double[] Probs)
        {
            int Stage = 1;
            int WinningStage = 1;
            double Probability = 0;
            foreach (double P in Probs)
            {
                if (P > Probability)
                {
                    Probability = P;
                    WinningStage = Stage;
                }
                Stage++;
            }
            return (WinningStage);
        }
        private int StochasticStrategy(double[] Probs)
        {

            double r = FlatRan.NextDouble();
            int Stage = 1;
            double Probablility = 0;
            foreach (double P in Probs)
            {
                Probablility += P;
                if (Probablility > r)
                {
                    break;
                }
                Stage++;
            }
            return (Stage);

        }
        private int eGreedyStrategy(double[] Probs, double e)
        {
            double r = FlatRan.NextDouble();
            int stage;
            if (r > e)
            {
                stage = GreedyStrategy(Probs);
            }
            else
            {
                double r2 = FlatRan.NextDouble();
                stage = Convert.ToInt32(Math.Ceiling(Probs.Length * r2));
            }
            return (stage);
        }
        private int eStochasticStrategy(double[] Probs, double e)
        {
            double r = FlatRan.NextDouble();
            int stage;
            if (r > e)
            {
                stage = StochasticStrategy(Probs);
            }
            else
            {
                double r2 = FlatRan.NextDouble();
                stage = Convert.ToInt32(Math.Ceiling(Probs.Length * r2));
            }
            return (stage);

        }

        private int StochasticGreedy(double[] Probs, double e)
        {
            double r = FlatRan.NextDouble();
            int stage;
            if (r > e)
            {
                stage = GreedyStrategy(Probs);
            }
            else
            {
                stage = StochasticStrategy(Probs);
            }
            return (stage);

        }
        private string BuildLifetimeCondition(ZoneAgent ZA, string ToD)
        {
            string Condition = "AtTime = '" + ToD + "' AND (";
            foreach (JunctionAgent JA in ZA.Junctions)
            {
                foreach (StageAgent SA in JA.Stages)
                {
                    foreach (LaneAgent LA in SA.Lanes)
                    {
                        if (LA.Duplicate == false)
                        {
                            foreach(LaneAgent.BitOfRoad BoR in LA.RoadSegments)
                            {
                                if (BoR.LaneNum == 0)
                                {
                                    Condition += "OnLink = '" + BoR.StartNode + ":" + BoR.EndNode + "' OR ";
                                }
                            }

                        }
                    }
                }
            } 
            Condition = Condition.Remove((Condition.Length - 3));
            Condition += ")";
            return (Condition);
        }

    }

    /*public class MultiHighBid : HighBid
    {
        //class main functions
        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void ProcessJunction(JunctionAgent JA)
        {
            base.ProcessJunction(JA);
        }
        public override void ProcessZone(ZoneAgent ZA,int ToD)
        {
            // ad hoc implementation for twin T
            double EastBid = ZA.Junctions[0].Stages[ZA.Junctions[0].NextStage - 1].LaneBid.Scalar;
            double WestBid = ZA.Junctions[1].Stages[ZA.Junctions[1].NextStage - 1].LaneBid.Scalar;

            if (EastBid > WestBid)
            {
                double Weight = 0;

                switch(ZA.Junctions[0].NextStage)
                {
                    case 1:
                        LaneAgent TempA = ZA.Junctions[0].Stages[0];
                        for(int i = 0; i<TempA.DistList.Count; i++)
                        {
                            TempA.DistList[i] += 55;//TODO Totally unnecessary! just add 55 to the AverageDistance!
                        }
                        TempA.GenerateBid();
                        Weight = 0.9*TempA.LaneBid.Scalar;
                        break;
                    case 2:
                        Weight = 0;
                        break;
                    case 3:
                        LaneAgent TempB = ZA.Junctions[0].Stages[2];
                        for (int i = 0; i < TempB.DistList.Count; i++)
                        {
                            TempB.DistList[i] += 55;
                        }
                        TempB.GenerateBid();
                        Weight = 0.515 * TempB.LaneBid.Scalar;
                        break;
                }
                ZA.Junctions[1].Stages[0].LaneBid.Scalar += Weight;
                ProcessJunction(ZA.Junctions[1]);

            }
            else if (WestBid > EastBid)
            {
                double Weight = 0;

                switch (ZA.Junctions[1].NextStage)
                {
                    case 1:
                        LaneAgent TempA = ZA.Junctions[1].Stages[1];
                        for (int i = 0; i < TempA.DistList.Count; i++)
                        {
                            TempA.DistList[i] += 55;
                        }
                        TempA.GenerateBid();
                        Weight = 0.826 * TempA.LaneBid.Scalar;
                        break;
                    case 2:
                        LaneAgent TempB = ZA.Junctions[1].Stages[1];
                        for (int i = 0; i < TempB.DistList.Count; i++)
                        {
                            TempB.DistList[i] += 55;
                        }
                        TempB.GenerateBid();
                        Weight = 0.826 * TempB.LaneBid.Scalar;
                        break;
                    case 3:
                        LaneAgent TempC = ZA.Junctions[1].Stages[2];
                        for (int i = 0; i < TempC.DistList.Count; i++)
                        {
                            TempC.DistList[i] += 55;
                        }
                        TempC.GenerateBid();
                        Weight = 0.515 * TempC.LaneBid.Scalar;
                        break;
                }
                ZA.Junctions[0].Stages[1].LaneBid.Scalar += Weight;
                ProcessJunction(ZA.Junctions[0]);
            }
        }

        //class additional functions
    }

    public class MultiHighBidV2 : HighBid
    {
        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void ProcessJunction(JunctionAgent JA)
        {
            int Stage = 1;
            int WinningStage = 1;
            double BidHolder = 0;
            for (int i = 0; i < JA.NoOfStages; i++)
            {
                Bid B = JA.Stages[i].LaneBid;
                if (B.Scalar > BidHolder)
                {
                    WinningStage = Stage;
                    BidHolder = B.Scalar;
                }
                Stage++;
            }
            JA.NextStage = WinningStage;
        }

        public override void ProcessZone(ZoneAgent ZA,int ToD)
        {
            // ad hoc implementation for twin T
            double EastBid = ZA.Junctions[0].Stages[ZA.Junctions[0].NextStage - 1].LaneBid.Scalar;
            double WestBid = ZA.Junctions[1].Stages[ZA.Junctions[1].NextStage - 1].LaneBid.Scalar;

            if (EastBid > WestBid)
            {
                switch (ZA.Junctions[0].NextStage)
                {
                    case 1:
                        ZA.Junctions[1].Stages[0].LaneBid.Scalar += ZA.Junctions[1].Stages[3].LaneBid.Scalar;
                        break;
                    case 2:
                        break;
                    case 3:
                        ZA.Junctions[1].Stages[0].LaneBid.Scalar += 0.515 * ZA.Junctions[1].Stages[4].LaneBid.Scalar;
                        break;
                }
                ProcessJunction(ZA.Junctions[1]);
            }

            else if (WestBid > EastBid)
            {
                switch (ZA.Junctions[1].NextStage)
                {
                    case 1:
                        
                        ZA.Junctions[0].Stages[1].LaneBid.Scalar += 0.826 * ZA.Junctions[0].Stages[4].LaneBid.Scalar;
                        break;
                    case 2:
                        
                        ZA.Junctions[0].Stages[1].LaneBid.Scalar += 0.826 * ZA.Junctions[0].Stages[4].LaneBid.Scalar;
                        break;
                    case 3:
                        ZA.Junctions[0].Stages[0].LaneBid.Scalar += 0.48 * ZA.Junctions[0].Stages[5].LaneBid.Scalar;
                        ZA.Junctions[0].Stages[1].LaneBid.Scalar += 0.515 * ZA.Junctions[0].Stages[5].LaneBid.Scalar;
                        break;
                }
                ProcessJunction(ZA.Junctions[0]);
                
            }
            
        }
    }*/

    /*public class MultiHighBid : HighBid
    {
        double
            Distance,
            G1,
            G2,
            F1,
            F2,
            A1,
            B1;

        public MultiHighBid() { }
        public MultiHighBid(double d1, double d2, double d3, double d4, double d5, double d6, double d7)
        {
            Distance = d1;
            A1 = d2;
            B1 = d3;
            F1 = d4;
            F2 = d5;
            G1 = d6;
            G2 = d7;
        }
        //class main functions
        public override void ProcessLane(LaneAgent SA)
        {
            base.ProcessLane(SA);
        }
        public override void ProcessStage(StageAgent SA)
        {
            base.ProcessStage(SA);
        }
        public override void ProcessJunction(JunctionAgent JA, int[] PreviousStage)
        {
            base.ProcessJunction(JA, PreviousStage);
        }
        public override void ProcessZone(ZoneAgent ZA, int ToD, int[] PreviousStage)
        {
            //ad hoc implementation for Twin T
            double EastBid = ZA.Junctions[0].Stages[ZA.Junctions[0].NextStage - 1].StageBid.Scalar;
            double WestBid = ZA.Junctions[1].Stages[ZA.Junctions[1].NextStage - 1].StageBid.Scalar;

            if (WestBid > EastBid)
            {

                switch (ZA.Junctions[1].NextStage)
                {
                    case 1:
                        ReWeightBids(ZA, 1, 1, 0, 1, G1, Distance);
                        ReWeightBids(ZA, 1, 1, 0, 0, G2, Distance);
                        break;
                    case 2:
                        ReWeightBids(ZA, 1, 1, 0, 1, G1, Distance);
                        ReWeightBids(ZA, 1, 1, 0, 0, G2, Distance);
                        break;
                    case 3:
                        ReWeightBids(ZA, 1, 2, 0, 1, F1, Distance);
                        ReWeightBids(ZA, 1, 2, 0, 0, F2, Distance);
                        break;
                }
                ProcessJunction(ZA.Junctions[0], PreviousStage);

            }
            else if (EastBid > WestBid)
            {

                switch (ZA.Junctions[0].NextStage)
                {
                    case 1:
                        ReWeightBids(ZA, 0, 0, 0, 0, A1, Distance);
                        break;
                    case 2:
                        break;
                    case 3:
                        ReWeightBids(ZA, 0, 2, 0, 0, B1, Distance);
                        break;
                }
                ProcessJunction(ZA.Junctions[1], PreviousStage);
            }*/
            /*// ad hoc implementation for High Rd Pair
            double WestBid = ZA.Junctions[0].Stages[ZA.Junctions[0].NextStage - 1].StageBid.Scalar;
            double EastBid = ZA.Junctions[1].Stages[ZA.Junctions[1].NextStage - 1].StageBid.Scalar;

            if (EastBid > WestBid)
            {

                switch(ZA.Junctions[1].NextStage)
                {
                    case 1:
                        ReWeightBids(ZA,1,0,0,2,0.2);
                        ReWeightBids(ZA, 1, 0, 0, 3, 0.2);
                        break;
                    case 2:
                        ReWeightBids(ZA, 1, 1, 1, 2,0.42);
                        ReWeightBids(ZA, 1, 1, 1, 3, 0.046);
                        break;
                    case 3:
                        break;
                }
                ProcessJunction(ZA.Junctions[0]);

            }
            else if (WestBid > EastBid)
            {

                switch (ZA.Junctions[0].NextStage)
                {
                    case 1:
                        ReWeightBids(ZA, 0, 0, 0, 1,0.2);
                        ReWeightBids(ZA, 0, 0, 0, 2, 0.2);
                        break;
                    case 2:
                        ReWeightBids(ZA, 0, 1, 0, 1, 0.34);
                        ReWeightBids(ZA, 0, 1, 0, 2, 0.11);
                        break;
                    case 3:
                        ReWeightBids(ZA, 0, 2, 0, 1,0.44);
                        ReWeightBids(ZA, 0, 2, 0, 2, 0.24);
                        break;
                    case 4:
                        break;
                }
                ProcessJunction(ZA.Junctions[1]);
            }
            base.ProcessZone(ZA,ToD, PreviousStage);
        }

        //class additional functions
        private void ReWeightBids(ZoneAgent ZA, int Jind, int Sind, int Lind, int wSind, double W, double Sep)
        {
            int wJind = Math.Abs(Jind - 1);
            LaneAgent Temp = ZA.Junctions[Jind].Stages[Sind].Lanes[Lind];
            Temp.AvDist += Sep;
            Temp.GenerateBid();
            ZA.Junctions[wJind].Stages[wSind].Lanes.Add(Temp);
            ZA.Junctions[wJind].Stages[wSind].Weights.Add(W);
            ProcessStage(ZA.Junctions[wJind].Stages[wSind]);
            ZA.Junctions[wJind].Stages[wSind].Lanes.RemoveAt(ZA.Junctions[wJind].Stages[wSind].Lanes.Count - 1);
            ZA.Junctions[wJind].Stages[wSind].Weights.RemoveAt(ZA.Junctions[wJind].Stages[wSind].Weights.Count - 1);
        }
    }

    */
    
}
