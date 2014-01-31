using System;
using System.Collections.Generic;
using System.Text;

namespace ParamicsPuppetMaster
{
    public class LiteToPDB
    {
        //*Class members
        int IDno;
        ReadSnapshotFileLite RSF;
        EditNodes EN;
        EditLink EL;
        ParamicsDBiLite PDB;

        //*Constructors
        public LiteToPDB() { IDno = 0; }
        public LiteToPDB(ReadSnapshotFileLite f1, EditNodes f2,EditLink f3, ParamicsDBiLite f4) 
        { 
            IDno = 0;
            RSF = f1;
            EN = f2;
            EL = f3;
            PDB = f4;
        }

        //function for populating the database with lite snapshot data.
        public void AddLineToDB()
        {

            foreach (ReadSnapshotFileLite.FileData SFD in RSF.Fdata)
            {
                VehicleDataLite TEMP = new VehicleDataLite(); 
                TEMP.AtTime = SFD.AtTime;
                TEMP.OnLink = SFD.OnLink;
                TEMP.LinkDist = SFD.LinkDist;
                TEMP.Vspeed = SFD.Vspeed;
                TEMP.Vtype = SFD.Vtype;
                TEMP.Lane = SFD.Lane;
                TEMP.BornTime = SFD.BornTime;
                TEMP.NextLink = SFD.NextLink;
                TEMP.NextNextLink = SFD.NextNextLink;
                TEMP.NextTurn = SFD.NextTurn; //null9
                TEMP.NextNextTurn = SFD.NextNextTurn; //null9
                //TEMP.NextNextTurn = "Null9";

                PDB.VDataAddLine(TEMP.MakeDBLine());
            }
        }

        //function for adding a stochastic line to the database
        public void AddLineToDBStoch(string source, double Xsigma, double dt, double pcV)
        {
            //note dt is the sampling rate of the simulated positioning instrument;
            double VehicleFrac = pcV / 100;//fraction of vehicles carrying sensors

            double Vsigma = Math.Sqrt(2*Math.Pow(Xsigma,2.0)/Math.Pow(dt,2.0));
            GaussSampler Xdist = new GaussSampler(0, Xsigma);
            GaussSampler Vdist = new GaussSampler(0, Vsigma);

            Random FlatRan = new Random();

            foreach (ReadSnapshotFileLite.FileData SFD in RSF.Fdata)
            {
                double dice = FlatRan.NextDouble();
                if (dice > VehicleFrac) continue;
                int LaneNum = 0;
                int Lane;
                double LaneWidth = 0.0;
                double Speed;
                double Distance;

                double DistError = Xdist.RandomSample();
                double LatError = Xdist.RandomSample();
                double Verror = Vdist.RandomSample();

                EL.SearchLaneData(SFD.OnLink.StartNode, SFD.OnLink.EndNode, ref LaneNum, ref LaneWidth);
                Speed = Math.Abs(SFD.Vspeed + Verror);
                Distance = Math.Abs(SFD.LinkDist + DistError);

                
                int LaneCorrection = 0;
                double Jump = LaneWidth / 2;

                if (LatError > Jump && LatError < 2 * Jump)
                {
                    LaneCorrection = -1;
                }
                else if (-LatError > Jump && -LatError < 2 * Jump)
                {
                    LaneCorrection = 1;
                }
                else if (LatError > 2 * Jump)
                {
                    LaneCorrection = -2;
                }
                else if (-LatError > 2 * Jump)
                {
                    LaneCorrection = 2;
                }

                Lane = SFD.Lane + LaneCorrection;

                if (Lane > (LaneNum - 1))
                {
                    Lane = LaneNum - 1;
                }
                else if (Lane < 0)
                {
                    Lane = 0;
                }
                
                VehicleDataLite TEMP = new VehicleDataLite();
                TEMP.Source = source;
                TEMP.AtTime = SFD.AtTime;
                TEMP.OnLink = SFD.OnLink;
                TEMP.LinkDist = Distance;
                TEMP.Vspeed = Speed;
                TEMP.Vtype = SFD.Vtype;
                TEMP.Lane = Lane;
                TEMP.BornTime = SFD.BornTime;
                TEMP.NextLink = SFD.NextLink;
                TEMP.NextNextLink = SFD.NextNextLink;
                TEMP.NextTurn = "NULL8";
                TEMP.NextNextTurn = "NULL8";

                PDB.VDataAddLine(TEMP.MakeDBLine());
            }

        }
    }
}
