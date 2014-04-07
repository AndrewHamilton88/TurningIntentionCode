using System;
using System.Collections.Generic;
using System.Text;
using Mapack;


namespace ParamincsSNMPcontrol
{
    public class Agents
    {
        //*class members
        public NetworkData NetDat;
        public Strategies Strat;
        public string Name;
        
        //*class construnctor
        public Agents(NetworkData A, Strategies B) { NetDat = A; Strat = B; }


    }

    public class LaneAgent : Agents
    {
        //*class members
        public List<BitOfRoad> RoadSegments = new List<BitOfRoad>();
        public List<double> SpeedList = new List<double>();
        public List<double> DistList = new List<double>();
        public double AvSpeed;
        public double[] AvSpeedTurns = new double[3];
        public double AvDist;
        public double[] AvDistTurns = new double[3];
        public int Count;
        public int[] CountTurns = new int[3];
        public bool Duplicate;
        public string UpstreamAgents;
        public string feedPercentages;
        public int[] TurningMovements = new int[3]; //This contains the number of [0] - Right, [1] - Straight, [2] - Left turns

        public Strategies.Bid LaneBid;

        //*Constructor
        public LaneAgent(NetworkData A, Strategies B) : base(A, B) { }

        //*function to return the total lane length in an area
        public double AreaLength()
        {
            double Len = 0;
            foreach (BitOfRoad b in RoadSegments)
            {
                Len += b.TotalLength;
            }
            return (Len);
        }
        public double geometricLength()
        {
            double minOff;
            double maxOff;
            double maxLen;

            minOff = RoadSegments[0].Offset;
            maxOff = RoadSegments[0].Offset;
            maxLen = RoadSegments[0].TotalLength;

            for (int i = 1; i < RoadSegments.Count; i++)
            {

                if (RoadSegments[i].Offset < minOff)
                {
                    minOff = RoadSegments[i].Offset;
                }
                else if (RoadSegments[i].Offset > maxOff)
                {
                    maxOff = RoadSegments[i].Offset;
                    maxLen = RoadSegments[i].TotalLength;
                }
                else
                {

                }
            }
            return (maxLen + maxOff - minOff);
        }

        //Function to get vehicle data from the database - AH editted the function at bottom of class to include turning intention
        public void PopulateAgentData(int ToD)
        {
            SpeedList.Clear();
            DistList.Clear();
            AvSpeedTurns[0] = 0.0; AvSpeedTurns[1] = 0.0; AvSpeedTurns[2] = 0.0;        //[0] = Right, [1] = Straight, [2] = Left
            AvDistTurns[0] = 0.0; AvDistTurns[1] = 0.0; AvDistTurns[2] = 0.0;
            TurningMovements[0] = 0; TurningMovements[1] = 0; TurningMovements[2] = 0;
            CountTurns[0] = 0; CountTurns[1] = 0; CountTurns[2] = 0;

            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();
            

            foreach (BitOfRoad BoR in RoadSegments)
            {
                string ConditionLine = "AtTime = '" + TimeOfDay + "' AND";
                //ConditionLine += " OnLink = '" + BoR.StartNode + ":" + BoR.EndNode + "'";
                ConditionLine += " OnLink = '" + BoR.StartNode + ":" + BoR.EndNode + "' AND ";
                //ConditionLine += " AlongLink < '25' AND ";                                     //NB. AH added this line to consider vehicles within 'X' metres of the junction
                ConditionLine += "LaneNum = " + BoR.LaneNum + " AND " + "NextTurn = '";
                string[] Directions = new string[3];
                Directions[0] = "Right"; Directions[1] = "Straight"; Directions[2] = "Left";        //NB. AH - has changed the order [0] - Right, [1] - Straight, [2] - Left

                for (int i = 0; i < 3; i++)
                {
                    List<double[]> SpeedDist = NetDat.PDB.GetSpeedAndDistane(ConditionLine + Directions[i] + "'");

                    foreach (double[] SD in SpeedDist)
                    {
                        AvSpeedTurns[i] += SD[0];
                        AvDistTurns[i] += SD[1] + BoR.Offset;
                        CountTurns[i]++;
                        SpeedList.Add(SD[0]);
                        DistList.Add(SD[1] + BoR.Offset);
                    }
                    TurningMovements[i] =+ CountTurns[i];
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (CountTurns[i] != 0)
                {
                    AvSpeedTurns[i] = AvSpeedTurns[i] / CountTurns[i];
                    AvDistTurns[i] = AvDistTurns[i] / CountTurns[i];
                }
            }
        }

        //Function to generate a bid
        public void GenerateBid()
        {
            Strat.ProcessLane(this);
        }

        //Function to generate a bid
        public void GenerateBidTurns()
        {
            //Strat.ProcessLaneTurns(this);
            Strat.ProcessLane(this);
        }



        public class BitOfRoad
        {
            //*Class Members
            public string
                StartNode,
                EndNode;
            public int
                LaneNum,
                OfLanes;
            public double
                Offset,
                TotalLength;

            public BitOfRoad(string sn, string en, int ln, int ol, double os)
            {
                StartNode = sn; EndNode = en;
                LaneNum = ln; OfLanes = ol;
                Offset = os;
            }
        }

    }

    public class StageAgent : Agents
    {
        //*Class Members
        public List<LaneAgent> Lanes = new List<LaneAgent>();
        public List<double> Weights = new List<double>();
        public List<double> LanePhases = new List<double>();
        public Strategies.Bid StageBid;

        //*Constructor
        public StageAgent(NetworkData A, Strategies B) : base(A, B) { }

        //*class function
        public void GenerateBid(int ToD)
        {
            foreach (LaneAgent LA in Lanes)
            {
                LA.PopulateAgentData(ToD);  //AH's function
                LA.GenerateBidTurns();  //AH's function
                //LA.PullDataAtTime(ToD);    //SB's original function
                //LA.GenerateBid();          //SB's original function
            }
            Strat.ProcessStage(this);
        }

        //*class function
        public void NeutralWeight()
        {
            for (int i = 0; i < Lanes.Count; i++)
            {
                Weights.Add(1);
            }
        }
    }

    public class JunctionAgent : Agents
    {
        //*Class Members
        public List<StageAgent> Stages = new List<StageAgent>();
        public string SignalNode;
        public List<double> AllPhases = new List<double>();
        public int NoOfStages;
        public int NextStage;

        //*Class Constructor
        public JunctionAgent(NetworkData A, Strategies B, string SN, int NoS)  //AH LOOK INTO
            : base(A, B)
        {

            SignalNode = SN;
            NoOfStages = NoS;
            NextStage = 1;
        }

        //*Function for mediating the auction
        public void MediateAuction(int ToD, int[] PreviousStage)
        {
            AllPhases.Clear();
            foreach (StageAgent SA in Stages)
            {
                SA.GenerateBid(ToD);
                foreach (double PhaseBid in SA.LanePhases)
                {
                    AllPhases.Add(PhaseBid);
                }
            }
            Strat.ProcessJunction(this, PreviousStage);

        }

        //*Function for building lifetime data string
        public string BuildLifeTimeString()
        {
            string Condition = "";
            foreach (StageAgent SA in this.Stages)
            {
                foreach (LaneAgent LA in SA.Lanes)
                {
                    if (LA.Duplicate == false)
                    {
                        foreach (LaneAgent.BitOfRoad BoR in LA.RoadSegments)
                        {
                            if (BoR.LaneNum == 0)
                            {
                                Condition += "OnLink = '" + BoR.StartNode + ":" + BoR.EndNode + "' OR ";
                            }
                        }

                    }
                }
            }
            return (Condition);

        }

        public string BuildLifetimeCondition(string ToD)
        {
            string Condition = "AtTime = '" + ToD + "' AND (";
            Condition += BuildLifeTimeString();
            Condition = Condition.Remove((Condition.Length - 3));
            Condition += ")";
            return (Condition);
        }

    }

    public class ZoneAgent : Agents
    {
        //**Class Members
        public List<JunctionAgent> Junctions = new List<JunctionAgent>();
        public int[] NextStages;


        //Constructor
        public ZoneAgent(NetworkData A, Strategies B)
            : base(A, B)
        {

        }

        //function for coordinating junction
        public void CoordinateJunctions(int ToD, int[] PreviousStage)
        {
            foreach (JunctionAgent JA in Junctions)
            {
                JA.MediateAuction(ToD, PreviousStage);
            }
            Strat.ProcessZone(this, ToD, PreviousStage);
            //WriteBidsDataBase(ToD);
            //WriteSITDataBase(ToD);
        }

        //*Function to get vehicle data from the database.
        /*public void PullDataAtTime(int ToD)
        {
            SpeedList.Clear();
            DistList.Clear();
            AvSpeed = 0;
            AvDist = 0;
            Count = 0;
            Left = 0;
            Straight = 0;
            Right = 0; //AH TODO

            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();

            foreach (BitOfRoad BoR in RoadSegments)
            {
                string ConditionLine = "AtTime = '" + TimeOfDay + "' AND";
                ConditionLine += " OnLink = '" + BoR.StartNode + ":" + BoR.EndNode + "' AND ";
                ConditionLine += "LaneNum = " + BoR.LaneNum;
                List<double[]> SpeedDist = NetDat.PDB.GetSpeedAndDistane(ConditionLine);

                foreach (double[] SD in SpeedDist)
                {
                    AvSpeed += SD[0];
                    AvDist += SD[1] + BoR.Offset;
                    Count++;
                    SpeedList.Add(SD[0]);
                    DistList.Add(SD[1] + BoR.Offset);
                }
                List<string[]> NextAndNextNextTurns = NetDat.PDB.GetTurningDirections(ConditionLine); //AH editted
                foreach (string[] Turns in NextAndNextNextTurns) //string[0] is the NextTurn, and string[1] is the NextNextTurn
                {
                    if (Turns[0] == "Left")
                    {
                        Left++;
                    }
                    if (Turns[0] == "Straight")
                    {
                        Straight++;
                    }
                    if (Turns[0] == "Right")
                    {
                        Right++;
                    }
                    //Console.WriteLine("Next: " + Turns[0] + " NextNext: " + Turns[1]);
                    
                }
            }
            Console.WriteLine("Left " + Convert.ToString(Left));
            Console.WriteLine("Straight " + Convert.ToString(Straight));
            Console.WriteLine("Right " + Convert.ToString(Right));
            Console.Read();        //AH NextTurn and NextNextTurn
            
            if (Count != 0)
            {
                AvSpeed = AvSpeed / Count;
                AvDist = AvDist / Count;
            }
        }*/

        /*public string BuildLifetimeCondition(string ToD)
{
string Condition = "AtTime = '" + ToD + "' AND (";
foreach (JunctionAgent JA in Junctions)
{
Condition += JA.BuildLifeTimeString();
}
Condition = Condition.Remove((Condition.Length - 3));
Condition += ")";
return (Condition);
}

private void WriteBidsDataBase(int ToD)
{
List<double> BidsList = new List<double>();
foreach (JunctionAgent JA in Junctions)
{
foreach (StageAgent SA in JA.Stages)
{
foreach (LaneAgent LA in SA.Lanes)
{
BidsList.Add(LA.LaneBid.Scalar);//TODO this only works for bids of type 'double'
}
}
}
double[] Bids = new double[BidsList.Count];
for (int i = 0; i < Bids.Length; i++)
{
Bids[i] = BidsList[i];
}

double LifeTime = 0;
int Vcount = 0;

TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
string TimeOfDay = TS.ToString();
string Condition = BuildLifetimeCondition(TimeOfDay);
NetDat.PDB.GetVLifeTime(Condition, ref LifeTime, ref Vcount);


NetDat.RecordDecision(TimeOfDay, Bids, NextStages, Vcount, LifeTime);
}

private void WriteSITDataBase(int ToD)
{
TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
string TimeOfDay = TS.ToString();
int Znum = 0;
foreach (JunctionAgent JA in Junctions)
{
foreach (StageAgent SA in JA.Stages)
{
foreach (LaneAgent LA in SA.Lanes)
{
//NetDat.SIT.AddSITLine(TimeOfDay, Znum, LA.Count, LA.AvSpeed, LA.AvDist);
Znum++;
}
}
}
}*/


    }

}