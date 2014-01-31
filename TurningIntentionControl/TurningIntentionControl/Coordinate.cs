using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace ParamincsSNMPcontrol
{
    public class Coordinate
    {
        //*Class Members
        public NetworkData NetDat;
        protected Strategies Strat;
        public ZoneAgent MainZone;
        protected XmlDocument NetworkStruct;
        public SignalsSet SigSet;

        //public bool Stochastic;
        //public double Xsig;
        //public double dt;
        //public double SensorPC;
        public string
            ParamicsPath,
            IP;
        public int Port;

        //*Class Constructor
        public Coordinate(string FileName, Strategies St, string ip, int prt)
        {
            Strat = St;
            IP = ip;
            Port = prt;

            NetworkStruct = new XmlDocument();
            NetworkStruct.Load(FileName);

            XmlNode PModel = NetworkStruct.GetElementsByTagName("ParamicsModel")[0];
            ParamicsPath = PModel.SelectSingleNode("path").FirstChild.Value;

        }

        public virtual void ConnectToParamics()
        {
            //Find out the number of agents and junctions
            int AgentNum = NetworkStruct.GetElementsByTagName("Agent").Count * 3;  //AH added times 3 to include all 12 phase bids
            int JunctionNum = NetworkStruct.GetElementsByTagName("Junction").Count;

            NetDat = new NetworkData(ParamicsPath, IP, Port, AgentNum, JunctionNum);
            NetDat.PDB.ClearTableContents();

            MainZone = new ZoneAgent(NetDat, Strat);

            FindAgents(NetDat, ref MainZone);


            List<string> NodeNames = new List<string>();

            foreach (JunctionAgent JA in MainZone.Junctions)
            {
                NodeNames.Add(JA.SignalNode);
            }

            SigSet = new SignalsSet(NetDat, NodeNames);

        }

        //function
        public virtual int[] EvaluationProcess(int ModelTimeofDay, int[] PreviousStage)
        {
            TimeSpan TS = new TimeSpan(0, 0, ModelTimeofDay / 100);
            string TimeOfDay = TS.ToString();

            bool Stochastic = false;
            string[] Source = { "default" };
            double[] Xsig = { 0 };
            double[] Ysig = { 0 };
            double[] SenPC = { 0 };

            FindStoch(ref Stochastic, ref Source, ref Xsig, ref Ysig, ref SenPC);

            if (Stochastic == false)
            {
                NetDat.IVPextract();
            }
            else
            {
                NetDat.IVPextractWiggle(Source, Xsig, Ysig, SenPC);
            }
            


            MainZone.CoordinateJunctions(ModelTimeofDay,PreviousStage);
            WriteBidsDatabase(ModelTimeofDay);

            /*foreach (int item in MainZone.NextStages)
            {
                Console.WriteLine(item);
                Console.Read();
            }*/


            return (MainZone.NextStages);           //AH - this is where the next stage is finalised
        }






        //Function to read get the agent data from the XML file
        protected void FindAgents(NetworkData NDin, ref ZoneAgent ZAin)
        {
            XmlNodeList JunctionsData = NetworkStruct.GetElementsByTagName("Junction");
            foreach (XmlNode Jcn in JunctionsData)
            {
                string SignalNode = Jcn.SelectSingleNode("SignalNode").FirstChild.Value;
                int NumStages = Convert.ToInt32(Jcn.SelectSingleNode("Stages").FirstChild.Value);

                JunctionAgent TempJA = new JunctionAgent(NDin, Strat, SignalNode, NumStages);

                XmlNodeList StageData = Jcn.SelectNodes("Stage");
                foreach (XmlNode Stg in StageData)
                {
                    StageAgent TempSA = new StageAgent(NDin, Strat);

                    XmlNodeList AgentData = Stg.SelectNodes("Agent");
                    foreach (XmlNode Agt in AgentData)
                    {
                        LaneAgent TempLA = new LaneAgent(NDin, Strat);
                        TempLA.Duplicate = Convert.ToBoolean(Agt.SelectSingleNode("Duplicate").FirstChild.Value);
                        TempLA.Name = Agt.SelectSingleNode("name").FirstChild.Value;
                        //TempLA.UpstreamAgents = Agt.SelectSingleNode("UpstreamAgents").FirstChild.Value;
                        XmlNodeList RoadData = Agt.SelectNodes("RoadSec");
                        foreach (XmlNode Rsec in RoadData)
                        {
                            string Snode, Enode;
                            int Lane, OLanes;
                            double Oset;

                            Snode = Rsec.SelectSingleNode("StartNode").FirstChild.Value;
                            Enode = Rsec.SelectSingleNode("EndNode").FirstChild.Value;
                            Lane = Convert.ToInt32(Rsec.SelectSingleNode("Lane").FirstChild.Value);
                            OLanes = Convert.ToInt32(Rsec.SelectSingleNode("OfLanes").FirstChild.Value);
                            Oset = Convert.ToDouble(Rsec.SelectSingleNode("Offset").FirstChild.Value);

                            TempLA.RoadSegments.Add(new LaneAgent.BitOfRoad(Snode, Enode, Lane, OLanes, Oset));

                        }
                        TempSA.Lanes.Add(TempLA);
                    }
                    TempSA.NeutralWeight();
                    TempJA.Stages.Add(TempSA);
                }
                ZAin.Junctions.Add(TempJA);
            }

        }

        //Function to read the stochastic parameters from the XML file
        protected void FindStoch(ref bool isStoch, ref string[] source, ref double[] Xsig, ref double[] Vsig, ref double[] SenPC)
        {
            XmlNode StochNode = NetworkStruct.GetElementsByTagName("Stochastic")[0];

            isStoch = Convert.ToBoolean(StochNode.SelectSingleNode("isStochastic").FirstChild.Value);
            string Sources = StochNode.SelectSingleNode("Source").FirstChild.Value;
            source = Sources.Split(',');
            Xsig = ReadVectorFromXML(StochNode, "Xsig");
            Vsig = ReadVectorFromXML(StochNode, "Vsig");
            SenPC = ReadVectorFromXML(StochNode, "SenPc");
            //TODO probably also need to add dt here to prevent speed from going to infinity....(this has happenned before)

        }
        private double[] ReadVectorFromXML(XmlNode Node, string name)
        {
            string List = Convert.ToString(Node.SelectSingleNode(name).FirstChild.Value);
            string[] splitList = List.Split(',');

            double[] ExtractedA = new double[splitList.Length];

            for (int i = 0; i < splitList.Length; i++)
            {
                ExtractedA[i] = Convert.ToDouble(splitList[i]);
            }
            return (ExtractedA);
        }

        protected void WriteBidsDatabase(int ToD)
        {
            List<double> BidsList = new List<double>();
            foreach (JunctionAgent JA in MainZone.Junctions)
            {
                foreach (StageAgent SA in JA.Stages)
                {
                    foreach (LaneAgent LA in SA.Lanes)
                    {
                        for (int i = 0; i < 3; i++)             //AH added this 'for loop'
                        {
                            BidsList.Add(LA.LaneBid.TurningBids[i]);
                        }
                        //BidsList.Add(LA.LaneBid.Scalar);//TODO this only works for bids of type 'double'

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
            

            NetDat.RecordDecision(TimeOfDay, Bids, MainZone.NextStages, Vcount, LifeTime);

        }

        private string BuildLifetimeCondition(string ToD)
        {
            string Condition = "AtTime = '" + ToD + "' AND (";
            foreach (JunctionAgent JA in MainZone.Junctions)
            {
                Condition += JA.BuildLifeTimeString();
            }
            Condition = Condition.Remove((Condition.Length - 3));
            Condition += ")";
            return (Condition);
        }

    }

    public class coordinateSIT : Coordinate
    {
        //class members
        //public override NetworkDataSIT NetDat;
        public NetworkDataSIT NetDat2;
        public ZoneAgent WobblyZone;
        public SITbridge SitB;


        //*Class Constructor
        public coordinateSIT(string FileName, Strategies St, string ip, int prt)
            : base(FileName, St, ip, prt) { }

        public override void ConnectToParamics()
        {
            //Find out the number of agents and junctions
            int AgentNum = NetworkStruct.GetElementsByTagName("Agent").Count;
            int JunctionNum = NetworkStruct.GetElementsByTagName("Junction").Count;

            //NetDat = new NetworkDataSIT(ParamicsPath, IP, Port, AgentNum, JunctionNum, "Vehicledatasimple", "SITtrue");
            NetDat = new NetworkDataSIT(ParamicsPath, IP, Port, AgentNum, JunctionNum, "LinkTurningMovements", "SITtrue");
            NetDat2 = new NetworkDataSIT(ParamicsPath, IP, Port, AgentNum, JunctionNum, "VehicledataWobbly", "SITwobbly");
            NetDat.PDB.ClearTableContents();
            NetDat2.PDB.ClearTableContents();
            NetDat2.LTB.ClearTableContents();

            MainZone = new ZoneAgent(NetDat, Strat);
            WobblyZone = new ZoneAgent(NetDat2, Strat);
            FindAgents(NetDat, ref MainZone);
            FindAgents(NetDat2, ref WobblyZone);

            SitB = new SITbridge(NetDat2);
            FindSITAgents(NetDat2, ref SitB);
            SitB.AreaMash();
            FindSensors(NetDat2, ref SitB);//TODO it is important that this is called after AreaMash() should probably put in acheck for this....

            List<string> NodeNames = new List<string>();

            foreach (JunctionAgent JA in MainZone.Junctions)
            {
                NodeNames.Add(JA.SignalNode);
            }

            SigSet = new SignalsSet(NetDat, NodeNames);

        }

        //function
        public override int[] EvaluationProcess(int ModelTimeofDay, int[] PreviousStage)
        {
            TimeSpan TS = new TimeSpan(0, 0, ModelTimeofDay / 100);
            string TimeOfDay = TS.ToString();

            bool Stochastic = false;
            string[] Source = { "default" };
            double[] Xsig = { 0 };
            double[] Ysig = { 0 };
            double[] SenPC = { 0 };

            FindStoch(ref Stochastic, ref Source, ref Xsig, ref Ysig, ref SenPC);

            NetDat.IVPextract();
            NetDat2.IVPextractWiggle(Source, Xsig, Ysig, SenPC);//TODO implement a check that Stochastic is "true".
            NetDat2.LoopExtract(ModelTimeofDay, 10);//TODO unhardcode 10 second calc time (call in function)

            int[] CurrentStages = MainZone.NextStages;//TODO ad hoc code for setting the red light state for areas in the triangle model

            MainZone.CoordinateJunctions(ModelTimeofDay, PreviousStage);

            WriteBidsDatabase(ModelTimeofDay);
            WriteSITDataBase(ModelTimeofDay, ref MainZone, (NetworkDataSIT)NetDat);

            //TODO ad hoc code for setting the red light state for areas in the triangle model
            /*SitB.ClearRedSignals();
            if (CurrentStages != null)
            {
                for (int si = 0; si < 3; si++)
                {
                    switch (si)
                    {
                        case 0:
                            switch (CurrentStages[si])
                            {
                                case 1:
                                    SitB.SetAreaRed("WS3A1");
                                    SitB.SetAreaRed("WS2A2");
                                    break;
                                case 2:
                                    SitB.SetAreaRed("WS1A1");
                                    SitB.SetAreaRed("WS1A2");
                                    break;
                                case 3:
                                    SitB.SetAreaRed("WS3A1");
                                    SitB.SetAreaRed("WS2A2");
                                    SitB.SetAreaRed("WS2A2");
                                    break;
                            }
                            break;
                        case 1:
                            switch (CurrentStages[si])
                            {
                                case 1:
                                    SitB.SetAreaRed("NS2A1");
                                    SitB.SetAreaRed("NS2A2");
                                    break;
                                case 2:
                                    SitB.SetAreaRed("NS1A1");
                                    SitB.SetAreaRed("NS1A2");
                                    break;
                            }
                            break;
                        case 2:
                            switch (CurrentStages[si])
                            {
                                case 1:
                                    SitB.SetAreaRed("ES3A1");
                                    SitB.SetAreaRed("ES4A1");
                                    break;
                                case 2:
                                    SitB.SetAreaRed("ES3A1");
                                    SitB.SetAreaRed("ES4A1");
                                    SitB.SetAreaRed("ES1A2");
                                    break;
                                case 3:
                                    SitB.SetAreaRed("ES2A1");
                                    SitB.SetAreaRed("ES4A1");
                                    SitB.SetAreaRed("ES1A2");
                                    break;
                                case 4:
                                    SitB.SetAreaRed("ES3A1");
                                    SitB.SetAreaRed("ES2A1");
                                    SitB.SetAreaRed("ES1A2");
                                    break;
                            }
                            break;
                    }

                }
            }*/
            //********************************************************************************/
            //TODO hardcoding for HighRd Signal pattern
            /*SitB.ClearRedSignals();
            if (CurrentStages != null)
            {
                for (int si = 0; si < 2; si++)
                {
                    switch (si)
                    {
                        case 0:
                            switch (CurrentStages[si])
                            {
                                case 1:
                                    SitB.SetAreaRed("WS3_A1");
                                    SitB.SetAreaRed("WS2_A1");
                                    SitB.SetAreaRed("WS3_A2");
                                    SitB.SetAreaRed("WS4_A1");
                                    break;
                                case 2:
                                    SitB.SetAreaRed("WS1_A1");
                                    SitB.SetAreaRed("WS3_A1");
                                    SitB.SetAreaRed("WS3_A2");
                                    SitB.SetAreaRed("WS4_A1");
                                    break;
                                case 3:
                                    SitB.SetAreaRed("WS1_A1");
                                    SitB.SetAreaRed("WS2_A1");
                                    break;
                                case 4:
                                    SitB.SetAreaRed("WS1_A1");
                                    SitB.SetAreaRed("WS2_A1");
                                    SitB.SetAreaRed("WS3_A1");
                                    break;
                            }
                            break;
                        case 1:
                            switch (CurrentStages[si])
                            {
                                case 1:
                                    SitB.SetAreaRed("ES2_A1");
                                    SitB.SetAreaRed("ES3_A1");
                                    SitB.SetAreaRed("ES2_A2");
                                    break;
                                case 2:
                                    SitB.SetAreaRed("ES3_A1");
                                    SitB.SetAreaRed("ES1_A1");
                                    break;
                                case 3:
                                    SitB.SetAreaRed("ES1_A1");
                                    SitB.SetAreaRed("ES2_A2");
                                    break;
                            }
                            break;
                    }
                }
            }*/
            //*********************************************************************************/
            SitB.PullSensorData(ModelTimeofDay);
            SitB.StateUpdate(ModelTimeofDay, NetDat2);
            //writeSITdiffFiles(MainZone,SitB,@"Z:\visualizationBuffer");



            return (MainZone.NextStages);
        }

        private void WriteSITDataBase(int ToD, ref ZoneAgent ZoneIn, NetworkDataSIT NDin)
        {
            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();
            int Znum = 0;
            foreach (JunctionAgent JA in ZoneIn.Junctions)
            {
                foreach (StageAgent SA in JA.Stages)
                {
                    foreach (LaneAgent LA in SA.Lanes)
                    {
                        if (!LA.Duplicate)
                        {
                            if (LA.Count == 0)
                            {
                                NDin.SIT.AddSITLine(TimeOfDay, Znum, LA.Name, Convert.ToDouble(LA.Count), 13.41, LA.AvDist, "unknown");//TODO super hacky hardcode! get rid of this!
                            }
                            else
                            {
                                NDin.SIT.AddSITLine(TimeOfDay, Znum, LA.Name, Convert.ToDouble(LA.Count), LA.AvSpeed, LA.AvDist, "unknown");
                            }
                            Znum++;
                        }
                    }
                }
            }
        }

        private void writeSITdiffFiles(ZoneAgent ZoneIn, SITbridge SITin, String Path)
        {
            string[] countDiff = new string[SITin.getAreas().Count];
            string[] speedDiff = new string[SITin.getAreas().Count];
            int Znum = 0;
            foreach (JunctionAgent JA in ZoneIn.Junctions)
            {
                foreach (StageAgent SA in JA.Stages)
                {
                    foreach (LaneAgent LA in SA.Lanes)
                    {
                        if (!LA.Duplicate)
                        {
                            foreach (SITarea theArea in SITin.getAreas())
                            {
                                if (theArea.Area.name.Equals(LA.Name))
                                {
                                    if (LA.Count == 0)
                                    {
                                        countDiff[Znum] = theArea.Area.name + "," + Math.Round(Math.Abs(theArea.Area.stateNow.getNumberOfVehicles()) * 2).ToString();
                                        speedDiff[Znum] = theArea.Area.name + "," + Math.Round((Math.Abs(13.41 - theArea.Area.stateNow.getAverageSpeedOfVehicles()))).ToString();
                                    }
                                    else
                                    {
                                        countDiff[Znum] = theArea.Area.name + "," + Math.Round((Math.Abs(LA.Count - theArea.Area.stateNow.getNumberOfVehicles())) * 2).ToString();
                                        speedDiff[Znum] = theArea.Area.name + "," + Math.Round((Math.Abs(LA.AvSpeed - theArea.Area.stateNow.getAverageSpeedOfVehicles()))).ToString();
                                    }
                                    break;
                                }

                            }
                            Znum++;
                        }
                    }
                }
            }

            File.WriteAllLines(Path + @"\diffCounts", countDiff);
            File.WriteAllLines(Path + @"\diffSpeeds", speedDiff);
        }

        //Function like FindAgents above but specifically for pulling out the area data for SIT calculation and the sensor data...

        public void FindSITAgents(NetworkData NDin, ref SITbridge SITin)
        {
            XmlNodeList JunctionsData = NetworkStruct.GetElementsByTagName("Junction");
            foreach (XmlNode Jcn in JunctionsData)
            {
                string SignalNode = Jcn.SelectSingleNode("SignalNode").FirstChild.Value;
                int NumStages = Convert.ToInt32(Jcn.SelectSingleNode("Stages").FirstChild.Value);

                //JunctionAgent TempJA = new JunctionAgent(NDin, Strat, SignalNode, NumStages);

                XmlNodeList StageData = Jcn.SelectNodes("Stage");
                foreach (XmlNode Stg in StageData)
                {
                    //StageAgent TempSA = new StageAgent(NDin, Strat);

                    XmlNodeList AgentData = Stg.SelectNodes("Agent");
                    foreach (XmlNode Agt in AgentData)
                    {
                        LaneAgent TempLA = new LaneAgent(NDin, Strat);
                        TempLA.Duplicate = Convert.ToBoolean(Agt.SelectSingleNode("Duplicate").FirstChild.Value);
                        if (!TempLA.Duplicate)
                        {
                            TempLA.Name = Agt.SelectSingleNode("name").FirstChild.Value;
                            if (Agt.SelectSingleNode("UpstreamAgents").HasChildNodes)
                            {
                                TempLA.UpstreamAgents = Agt.SelectSingleNode("UpstreamAgents").FirstChild.Value;
                            }
                            if (Agt.SelectSingleNode("FeedPercentage").HasChildNodes)
                            {
                                TempLA.feedPercentages = Agt.SelectSingleNode("FeedPercentage").FirstChild.Value;
                            }
                            XmlNodeList RoadData = Agt.SelectNodes("RoadSec");
                            foreach (XmlNode Rsec in RoadData)
                            {
                                string Snode, Enode;
                                int Lane, OLanes;
                                double Oset, Tlen;

                                Snode = Rsec.SelectSingleNode("StartNode").FirstChild.Value;
                                Enode = Rsec.SelectSingleNode("EndNode").FirstChild.Value;
                                Lane = Convert.ToInt32(Rsec.SelectSingleNode("Lane").FirstChild.Value);
                                OLanes = Convert.ToInt32(Rsec.SelectSingleNode("OfLanes").FirstChild.Value);
                                Oset = Convert.ToDouble(Rsec.SelectSingleNode("Offset").FirstChild.Value);
                                Tlen = Convert.ToDouble(Rsec.SelectSingleNode("TotalLength").FirstChild.Value);

                                LaneAgent.BitOfRoad br = new LaneAgent.BitOfRoad(Snode, Enode, Lane, OLanes, Oset);
                                br.TotalLength = Tlen;
                                TempLA.RoadSegments.Add(br);
                            }
                            SITin.AddArea(new SITarea(TempLA));
                        }
                    }
                }
            }

        }

        //* function to pull specifically the sensor data from the config file...

        private void FindSensors(NetworkData NDin, ref SITbridge SitIn)
        {
            XmlNode SensorData = NetworkStruct.GetElementsByTagName("SensorModel")[0];

            XmlNode CensusData = SensorData.SelectSingleNode("Census");
            XmlNode ProbeData = SensorData.SelectSingleNode("Probe");

            //get loop data
            XmlNodeList Loops = CensusData.SelectNodes("Loop");
            foreach (XmlNode loop in Loops)
            {
                string Name = loop.SelectSingleNode("Name").FirstChild.Value;
                string AreaNme = loop.SelectSingleNode("Area").FirstChild.Value;
                double Position = Convert.ToDouble(loop.SelectSingleNode("Position").FirstChild.Value);
                int Lanes = Convert.ToInt32(loop.SelectSingleNode("LanesCovered").FirstChild.Value);
                double CountSD = Convert.ToDouble(loop.SelectSingleNode("CountSD").FirstChild.Value);
                bool HasSpeed = Convert.ToBoolean(loop.SelectSingleNode("HasSpeed").FirstChild.Value);
                double SpeedSD = 0;
                if (HasSpeed)
                {
                    SpeedSD = Convert.ToDouble(loop.SelectSingleNode("SpeedSD").FirstChild.Value);
                }

                SingleInstanceOfTruth.censusSensor TheSensor;

                if (HasSpeed)
                {
                    TheSensor = new SingleInstanceOfTruth.loopPair(Name, Lanes, Position, CountSD, SpeedSD);
                }
                else
                {
                    TheSensor = new SingleInstanceOfTruth.inductiveLoop(Name, CountSD, Lanes, Position);
                }

                SitIn.AddCensus(TheSensor, AreaNme);
            }

            //*get probe data
            XmlNodeList WiFis = ProbeData.SelectNodes("WiFiType");
            foreach (XmlNode wifi in WiFis)
            {
                string Name = wifi.SelectSingleNode("Name").FirstChild.Value;
                double CountSD = Convert.ToDouble(wifi.SelectSingleNode("CountSD").FirstChild.Value);
                double SpeedSD = Convert.ToDouble(wifi.SelectSingleNode("SpeedSD").FirstChild.Value);
                double PenRate = Convert.ToDouble(wifi.SelectSingleNode("PenetrationRate").FirstChild.Value);
                double PenSD = Convert.ToDouble(wifi.SelectSingleNode("PenetrationSD").FirstChild.Value);

                SitIn.AddProbe(new SingleInstanceOfTruth.WiFi(Name, CountSD, SpeedSD, PenRate, PenSD));

            }


        }




    }
}