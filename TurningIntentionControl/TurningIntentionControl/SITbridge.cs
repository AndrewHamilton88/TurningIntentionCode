using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ParamincsSNMPcontrol
{
    public class SITbridge
    {
        NetworkDataSIT NetDat;
        //*class members
        List<SITarea> AreaList;
        List<Census> CensusList;
        List<Probe> ProbeList;

        public SITbridge(NetworkDataSIT nd)
        {
            NetDat = nd;
            AreaList = new List<SITarea>();
            CensusList = new List<Census>();
            ProbeList = new List<Probe>();


        }

        //*class function
        public void AddArea(SITarea sIn)
        {
            AreaList.Add(sIn);
        }

        //*function to connect each area with their upstream area.
        public void AreaMash()
        {
            foreach (SITarea SA in AreaList)
            {
                if (!SA.EdgeArea)
                {
                    for (int i = 0; i < SA.UpstreamIndex; i++)
                    {
                        string nme = SA.UpstreamNames[i];
                        double fPC = SA.feedPercentage[i];
                        foreach (SITarea mSA in AreaList)
                        {
                            if (nme.Equals(mSA.Agent.Name))
                            {
                                SA.Area.addFeed(new SingleInstanceOfTruth.FeedArea(mSA.Area, fPC));
                            }
                        }
                    }
                }
            }
        }

        public void AddCensus(SingleInstanceOfTruth.censusSensor sC, string A)
        {
            Census pC = new Census(sC, A);
            CensusList.Add(pC);

            foreach (SITarea SA in AreaList)
            {
                if (SA.Agent.Name.Equals(pC.Area))
                {
                    SA.Area.addCensus(pC.SitCensus);
                    SA.Area.addToCensusSensorNum(1);
                }
            }

        }

        public void AddProbe(SingleInstanceOfTruth.probeSensor sP)
        {
            ProbeList.Add(new Probe(sP));

            foreach (SITarea SA in AreaList)
            {
                SA.Area.addProbe(new SingleInstanceOfTruth.WiFi(sP.name, sP.vehicleCountSD, sP.vehicleAverageSpeedSD, sP.penetrationRate, sP.penetrationRateSD));
                SA.Area.addToProbeSensorNum(1);
            }
        }

        public void StateUpdate(int ModelTimeOfDay, NetworkDataSIT NDin)
        {
            TimeSpan TS = new TimeSpan(0, 0, ModelTimeOfDay / 100);
            DateTime TimeStep = new DateTime();
            TimeStep = TimeStep.Add(TS);

            foreach (SITarea SA in AreaList)
            {
                SA.Area.updateArea(TimeStep, 10);//TODO unhardcode the timestep length!!!!!
            }
            WriteSITdatabase(ModelTimeOfDay, NDin);
            writeSITfiles(@"Z:\visualizationBuffer");
        }

        public void PullSensorData(int ToD)
        {
            PullProbeData(ToD);
            PullCensusData(ToD);
        }

        public List<SITarea> getAreas()
        {
            return (AreaList);
        }

        private void PullProbeData(int ToD)
        {
            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();

            foreach (SITarea SA in AreaList)
            {
                foreach (SingleInstanceOfTruth.probeSensor p in SA.Area.probeSensors)
                {
                    double AvSpeed = 0;
                    double AvDist = 0;
                    int Count = 0;

                    foreach (LaneAgent.BitOfRoad br in SA.Agent.RoadSegments)
                    {
                        string ConditionLine = "AtTime = '" + TimeOfDay + "' AND";
                        ConditionLine += " OnLink = '" + br.StartNode + ":" + br.EndNode + "' AND ";
                        ConditionLine += "LaneNum = " + br.LaneNum + " AND ";
                        ConditionLine += "Source = '" + p.name + "'";
                        List<double[]> SpeedDist = NetDat.PDB.GetSpeedAndDistane(ConditionLine);

                        foreach (double[] SD in SpeedDist)
                        {
                            AvSpeed += SD[0];
                            AvDist += SD[1] + br.Offset;
                            Count++;
                        }
                    }
                    if (Count != 0)
                    {
                        AvSpeed = AvSpeed / Count;
                        AvDist = AvDist / Count;
                    }

                    /*//Set speed to local speed limit if count is zeoro//TODO bit hacky sort this out!
                    if (Count < 0.5)
                    {
                        AvSpeed = 13.41;
                    }*/
                    p.addMeasurements(Count, AvSpeed);

                }
            }
        }

        private void PullCensusData(int ToD)
        {
            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();
            TimeSpan StartTime = new TimeSpan();

            foreach (SITarea SA in AreaList)
            {
                foreach (SingleInstanceOfTruth.censusSensor cl in SA.Area.censusSensors)
                {
                    int VCountI = 0; double AvSpeed = 0;
                    NetDat.LTB.getCountAndSpeed(cl.name, TimeOfDay, ref VCountI, ref AvSpeed, ref StartTime);
                    TimeSpan dt = TS.Subtract(StartTime);
                    double VCount = Convert.ToDouble(VCountI);

                    /*//Set speed to local speed limit if count is zeoro//TODO bit hacky sort this out!
                    if (VCount < 0.5)
                    {
                        AvSpeed = 13.41;
                    }*/

                    //***Add Gaussian Noise**///
                    ParamicsPuppetMaster.GaussSampler GS1 = new ParamicsPuppetMaster.GaussSampler(0, cl.vehicleCountSD);
                    //VCount += GS1.RandomSample();
                    //***********************///

                    if (cl.hasV)
                    {
                        ParamicsPuppetMaster.GaussSampler GS2 = new ParamicsPuppetMaster.GaussSampler(0, cl.vehicleAverageSpeedSD);
                        //AvSpeed += GS2.RandomSample();
                        cl.addMeasurements(VCount, dt, AvSpeed);
                    }
                    else
                    {
                        cl.addMeasurements(VCount, dt);
                    }
                }
            }

        }

        private void WriteSITdatabase(int ToD, NetworkDataSIT NDin)
        {
            TimeSpan TS = new TimeSpan(0, 0, ToD / 100);
            string TimeOfDay = TS.ToString();
            int Znum = 0;

            foreach (SITarea SA in AreaList)
            {
                NDin.SIT.AddSITLine(TimeOfDay, Znum, SA.Area.name, SA.Area.stateNow.getNumberOfVehicles(), SA.Area.stateNow.getAverageSpeedOfVehicles(), 0, "EKF");
                Znum++;
            }
        }

        private void writeSITfiles(String Path)
        {
            string[] countLines = new string[AreaList.Count];
            string[] speedLines = new string[AreaList.Count];
            for (int i = 0; i < AreaList.Count; i++)
            {
                countLines[i] = AreaList[i].Area.name + "," + Math.Round(AreaList[i].Area.stateNow.getNumberOfVehicles() * 20);
                speedLines[i] = AreaList[i].Area.name + "," + Math.Round(AreaList[i].Area.stateNow.getAverageSpeedOfVehicles() * 5);
            }

            File.WriteAllLines(Path + @"\someCounts", countLines);
            File.WriteAllLines(Path + @"\someSpeeds", speedLines);
        }

        public void ClearRedSignals()
        {
            foreach (SITarea SA in AreaList)
            {
                SA.Area.isStopped = false;
            }
        }
        public void SetAreaRed(string areaName)
        {
            foreach (SITarea SA in AreaList)
            {
                if (SA.Area.name.Equals(areaName))
                {
                    SA.Area.isStopped = true;
                }
            }
        }
    }

    public class SITarea
    {
        //*class members
        public LaneAgent Agent;
        public string[] UpstreamNames;
        public double[] feedPercentage;
        public int UpstreamIndex;
        public SingleInstanceOfTruth.area Area;
        public bool EdgeArea;


        //*class constructor
        public SITarea(LaneAgent LA)
        {
            Agent = LA;
            Area = new SingleInstanceOfTruth.area(LA.GetHashCode(), LA.Name, null, LA.geometricLength(), LA.AreaLength(), 0, 0);
            if (Agent.UpstreamAgents != null)
            {
                UpstreamNames = Agent.UpstreamAgents.Split(',');
                UpstreamIndex = UpstreamNames.Length;
                EdgeArea = false;
                if (Agent.feedPercentages != null)
                {
                    string[] feedStrings = Agent.feedPercentages.Split(',');
                    if (feedStrings.Length != UpstreamIndex)
                    {
                        Console.WriteLine("Error the number of feed percentages supplied does not match the number of upstream areas");
                    }
                    feedPercentage = new double[UpstreamIndex];
                    for (int i = 0; i < UpstreamIndex; i++)
                    {
                        feedPercentage[i] = Convert.ToDouble(feedStrings[i]);
                    }
                }
                else
                {
                    feedPercentage = new double[UpstreamIndex];
                    for (int i = 0; i < UpstreamIndex; i++)
                    {
                        feedPercentage[i] = 1;
                    }
                }

            }
            else
            {
                EdgeArea = true;
            }
        }

    }

    public class Census
    {
        public SingleInstanceOfTruth.censusSensor SitCensus;
        public string Area; //the area with which the sencor is associated;

        public Census(SingleInstanceOfTruth.censusSensor sC, string A)
        {
            SitCensus = sC; Area = A;
        }
    }

    public class Probe
    {
        public SingleInstanceOfTruth.probeSensor SitProbe;

        public Probe(SingleInstanceOfTruth.probeSensor sP)
        {
            SitProbe = sP;
        }
    }
}