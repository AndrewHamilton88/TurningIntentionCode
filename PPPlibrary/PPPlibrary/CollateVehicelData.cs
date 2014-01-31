using System;
using System.Collections.Generic;
using System.Text;


namespace ParamicsPuppetMaster
{
    public class CollateVehicelData
    {
        //*Class members
        int IDno;
        string RouteMethod;
        ReadEventsFile REF;
        ReadSnapshotFile RSF;
        EditPaths EP;
        EditNodes EN;
        EditZones EZ;
        ParamicsDBi PDB;


        //*Constructors
        public CollateVehicelData() { IDno = 0; }
        public CollateVehicelData(ReadEventsFile f1, ReadSnapshotFile f3, EditPaths f4, EditNodes f5, ParamicsDBi f6)
        {
            IDno = 0;
            REF = f1;
            RSF = f3;
            EP = f4;
            EN = f5;
            PDB = f6;
            RouteMethod = "Paths";
        }
        public CollateVehicelData(ReadEventsFile f1, ReadSnapshotFile f3, EditZones f4, EditNodes f5, ParamicsDBi f6)
        {
            IDno = 0;
            REF = f1;
            RSF = f3;
            EZ = f4;
            EN = f5;
            PDB = f6;
            RouteMethod = "OD";
        }

        //Function for adding data
        public void AddStep(int StageNumber, int ScenarioNumber)
        {
            try
            {
                foreach (ReadEventsFile.FileData VehiclesBorn in REF.Fdata)
                {
                    VehicleIdentity TempID = new VehicleIdentity();
                    ViDincriment();
                    TempID.ViD = IDno.ToString("00000");
                    TempID.Tag = VehiclesBorn.Tag;
                    TempID.Vtype = VehiclesBorn.Vtype;
                    TempID.BornAt = VehiclesBorn.AtTime;
                    TempID.BornLink = VehiclesBorn.OnLink;
                    TempID.BornStage = StageNumber;
                    TempID.BornScenario = ScenarioNumber;
                    //Destination data are not available, to be added later
                    //Origin data only available in OD

                    if (RouteMethod.Equals("OD"))
                    {
                        TempID.Origin = GetOriginZone(VehiclesBorn.OnLink);
                    }

                    PDB.ViDAddLine(TempID.MakeDBLine());

                }



                foreach (ReadSnapshotFile.FileData SnapShot in RSF.Fdata)
                {
                    string DBcondition = "ABC";

                    if (RouteMethod.Equals("OD"))
                    {
                        //DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype);//+ " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                        DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                    }
                    else if (RouteMethod.Equals("Paths"))
                    {
                        LinkID FirstLink = FirstLinkinRoute(SnapShot);
                        DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND BornLink = '" + FirstLink.MakeString() + "' AND VehicleType = " + SnapShot.Vtype + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                    }

                    List<VehicleIdentity> TempIDL = new List<VehicleIdentity>();
                    TempIDL = PDB.VidGrabLine(DBcondition);
                    VehicleIdentity TempID = new VehicleIdentity();

                    if (TempIDL.Count == 1)
                    {
                        TempID = TempIDL[0];
                    }
                    else if (TempIDL.Count > 1)
                    {
                        try
                        {
                            if (RouteMethod.Equals("OD"))
                            {
                                //DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype);//+ " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                                DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype + " AND MagicNumbers = '" + SnapShot.MagicNumbers + "'" + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                            }
                            else if (RouteMethod.Equals("Paths"))
                            {
                                LinkID FirstLink = FirstLinkinRoute(SnapShot);
                                DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND BornLink = '" + FirstLink.MakeString() + "' AND VehicleType = " + SnapShot.Vtype + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                            }
                            List<VehicleIdentity> TempIDL2 = PDB.VidGrabLine(DBcondition);
                            if (TempIDL2.Count == 0)
                            {
                                throw new Exception();
                            }
                            else if (TempIDL2.Count == 1)
                            {
                                TempID = TempIDL2[0];
                            }
                            else
                            {
                                Console.WriteLine("Something weird is going on");
                            }
                        }
                        catch (Exception e)
                        {
                            if (RouteMethod.Equals("OD"))
                            {
                                //DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype);//+ " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                                DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND Origin = " + SnapShot.Origin + " AND VehicleType = " + SnapShot.Vtype + " AND ISNULL(MagicNumbers)" + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                            }
                            else if (RouteMethod.Equals("Paths"))
                            {
                                LinkID FirstLink = FirstLinkinRoute(SnapShot);
                                DBcondition = ("BornTime = '" + SnapShot.BornTime.TimeOfDay.ToString() + "' AND BornLink = '" + FirstLink.MakeString() + "' AND VehicleType = " + SnapShot.Vtype + " AND Obsolete = '0' AND (BornStage < " + StageNumber + " OR BornScenario = " + ScenarioNumber + ")");
                            }
                            List<VehicleIdentity> TempIDL3 = PDB.VidGrabLine(DBcondition);

                            if (TempIDL3.Count == 0) { Console.WriteLine("Something weird is going on again"); }

                            TempID = TempIDL3[(TempIDL3.Count - 1)];
                            TempID.MagicNumbers = SnapShot.MagicNumbers;
                            PDB.ViDAddLine(TempID.MakeDBLine());
                        }
                    }
                    else if (TempIDL.Count == 0)
                    {
                        Console.WriteLine("What is happenning here?");
                    }


                    if (TempID.ViD.Equals("NoID"))
                    {
                        //throw new Exception("Error: Unique vehicle ID was not assigned to vehicle from snapshot");
                        Console.WriteLine("Vehicle id is unassigned");
                    }

                    if (RouteMethod.Equals("OD"))
                    {
                        if (TempID.Destination == 511)
                        {
                            TempID.Destination = SnapShot.Destination;
                            PDB.ViDAddLine(TempID.MakeDBLine());
                        }
                    }
                    else if (RouteMethod.Equals("Paths"))
                    {
                        if (TempID.Routename.Equals("NULL"))
                        {
                            TempID.Routename = SnapShot.RouteName;
                            PDB.ViDAddLine(TempID.MakeDBLine());
                        }
                    }
                    VehiclePosition TempDat = new VehiclePosition();
                    TempDat.ViD = TempID.ViD;
                    TempDat.OnLink = SnapShot.OnLink;
                    TempDat.AtTime = SnapShot.AtTime;
                    TempDat.LinkDist = SnapShot.LinkDist;
                    TempDat.Vspeed = SnapShot.Vspeed;
                    double[] Position = LinkToCartesian(SnapShot.OnLink, SnapShot.LinkDist);
                    TempDat.X = Position[0];
                    TempDat.Y = Position[1];
                    TempDat.Z = Position[2];
                    TempDat.InStage = StageNumber;
                    TempDat.InScenario = ScenarioNumber;

                    PDB.VposAddLine(TempDat.MakeDBLine());
                    //CollatedData.Add(TempDat);


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
        //Function for incrimenting ID counter
        void ViDincriment()
        {
            IDno++;
        }


        //function for getting Origin zone from fist Link
        public int GetOriginZone(LinkID Link)
        {
            double[] StartNodeLoc = new double[2] { 0, 0 };
            double[] EndNodeLoc = new double[2] { 0, 0 };
            int ZoneNum = 511;

            try
            {

                foreach (N_W_Node node in EN.NodeList)
                {
                    if (Link.StartNode.Equals(node.NodeNum))
                    {
                        StartNodeLoc[0] = node.X;
                        StartNodeLoc[1] = node.Y;
                    }
                    else if (Link.EndNode.Equals(node.NodeNum))
                    {
                        EndNodeLoc[0] = node.X;
                        EndNodeLoc[1] = node.Y;
                    }

                }

                foreach (N_W_Zone zone in EZ.ZoneList)
                {
                    if (StartNodeLoc[0] < zone.Max[0] && StartNodeLoc[0] > zone.Min[0])
                    {
                        if (StartNodeLoc[1] < zone.Max[1] && StartNodeLoc[1] > zone.Min[1])
                        {
                            if (EndNodeLoc[0] < zone.Max[0] && EndNodeLoc[0] > zone.Min[0] && EndNodeLoc[1] < zone.Max[1] && EndNodeLoc[1] > zone.Min[1])
                            {
                                ZoneNum = zone.ZoneNum;
                                break;
                            }
                        }
                    }
                }
                if (ZoneNum == 511)
                {
                    throw new Exception("Error: No zone was found that contains both the link nodes");
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }


            return (ZoneNum);
        }

        //Function for Finding the First Link on a path
        LinkID FirstLinkinRoute(ReadSnapshotFile.FileData Sn)
        {
            LinkID TempLink = new LinkID();
            foreach (N_W_Path Route in EP.PathList)
            {
                if (Sn.RouteName.Equals(Route.PathName))
                {
                    TempLink = Route.PathDescription[0];
                    break;
                }
            }

            return (TempLink);
        }

        //function for calculating vehicle xyz position
        public double[] LinkToCartesian(LinkID Link, double l)
        {
            double X1, X2, Y1, Y2, Z1, Z2;
            X1 = 0; X2 = 0; Y1 = 0; Y2 = 0; Z1 = 0; Z2 = 0;
            foreach (N_W_Node ThisNode in EN.NodeList)
            {
                if (ThisNode.NodeNum.Equals(Link.StartNode))
                {
                    X1 = ThisNode.X;
                    Y1 = ThisNode.Y;
                    Z1 = ThisNode.Z;
                    break;
                }
            }
            foreach (N_W_Node ThisNode in EN.NodeList)
            {
                if (ThisNode.NodeNum.Equals(Link.EndNode))
                {
                    X2 = ThisNode.X;
                    Y2 = ThisNode.Y;
                    Z2 = ThisNode.Z;
                    break;
                }
            }
            double deltaX = X2 - X1;
            double deltaY = Y2 - Y1;
            double deltaZ = Z2 - Z1;
            double Ratio = l / Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2) + Math.Pow(deltaZ, 2));

            double[] Location = new double[3] { (Ratio * deltaX + X1), (Ratio * deltaY + Y1), (Ratio * deltaZ + Z1) };
            return (Location);


        }

        //function for changing the Files
        public void UpdateFiles(ReadEventsFile f1, ReadSnapshotFile f3, EditPaths f4, EditNodes f5, ParamicsDBi f6)
        {
            REF = f1;
            RSF = f3;
            EP = f4;
            EN = f5;
            PDB = f6;
            RouteMethod = "Paths";
        }
        public void UpdateFiles(ReadEventsFile f1, ReadSnapshotFile f3, EditZones f4, EditNodes f5, ParamicsDBi f6)
        {
            REF = f1;
            RSF = f3;
            EZ = f4;
            EN = f5;
            PDB = f6;
            RouteMethod = "OD";
        }



    }


}