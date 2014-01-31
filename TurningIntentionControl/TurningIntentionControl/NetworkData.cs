using System;
using System.Collections.Generic;
using System.Text;

namespace ParamincsSNMPcontrol
{
    public class NetworkData : SnmpConnect
    {
        //*Class Members
        public ParamicsPuppetMaster.EditNodes EN;
        public ParamicsPuppetMaster.EditLink EL;
        ParamicsPuppetMaster.EditZones EZ;
        public ParamicsPuppetMaster.ParamicsDBiLite PDB;
        ParamicsPuppetMaster.CollateVehicelData CVD;
        public ParamicsPuppetMaster.BidDataTable BDT;


        //*Class Constructor
        public NetworkData(string NPath, string NIP, int NPort, int NoOfAgents, int NoOfJuntions)
            : base(NPath, NIP, NPort)
        {
            EN = new ParamicsPuppetMaster.EditNodes(Path);
            EL = new ParamicsPuppetMaster.EditLink(Path);
            EZ = new ParamicsPuppetMaster.EditZones(Path);
            //PDB = new ParamicsPuppetMaster.ParamicsDBiLite("paramicsdb", "localhost", "root", "trg", "VehicleDataSimple");
            PDB = new ParamicsPuppetMaster.ParamicsDBiLite("paramicsdb", "localhost", "root", "trg", "LinkTurningMovements");
            CVD = new ParamicsPuppetMaster.CollateVehicelData();
            BDT = new ParamicsPuppetMaster.BidDataTable("paramicsdb", "localhost", "root", "trg", NoOfAgents, NoOfJuntions);

        }

        //*Function for extracting IVPdata
        public void IVPextract()
        {
            IVPextract(PDB);
        }
        protected void IVPextract(ParamicsPuppetMaster.ParamicsDBiLite PDBin)
        {
            ParamicsPuppetMaster.ReadSnapshotFileLite RSN = new ParamicsPuppetMaster.ReadSnapshotFileLite(Path);
            ParamicsPuppetMaster.LiteToPDB TDB = new ParamicsPuppetMaster.LiteToPDB(RSN, EN, EL, PDBin);
            TDB.AddLineToDB();
        }

        public void IVPextractWiggle(string source, double Xsig, double Vsig, double SenPC)
        {
            string[] Sources = { source };
            double[] XsigA = { Xsig };
            double[] VsigA = { Vsig };
            double[] SenPCA = { SenPC };
            IVPextractWiggle(Sources, XsigA, VsigA, SenPCA);
        }

        public void IVPextractWiggle(string[] Source, double[] Xsig, double[] Vsig, double[] SenPC)
        {
            IVPextractWiggle(PDB, Source, Xsig, Vsig, SenPC);
        }
        protected void IVPextractWiggle(ParamicsPuppetMaster.ParamicsDBiLite PDBin, string[] source, double[] Xsig, double[] Vsig, double[] SenPC)
        {
            if (Xsig.Length != Vsig.Length || Xsig.Length != SenPC.Length || Xsig.Length != source.Length)
            {
                throw new Exception("The lengths of the array input to this function do not match");
            }
            ParamicsPuppetMaster.ReadSnapshotFileLite RSN = new ParamicsPuppetMaster.ReadSnapshotFileLite(Path);
            ParamicsPuppetMaster.LiteToPDB TDB = new ParamicsPuppetMaster.LiteToPDB(RSN, EN, EL, PDBin);
            for (int i = 0; i < Xsig.Length; i++)
            {
                TDB.AddLineToDBStoch(source[i], Xsig[i], Vsig[i], SenPC[i]);
            }
        }
        //*Function for recording the decision data
        public void RecordDecision(string Time, double[] bids, int[] decisions, int Vcount, double LifeSpan)
        {

            BDT.AddBidLine(Time, bids, decisions, Vcount, LifeSpan);

        }



    }

    public class NetworkDataSIT : NetworkData
    {
        public ParamicsPuppetMaster.SITData SIT;
        public ParamicsPuppetMaster.LoopTable LTB;

        //*Class Constructor
        public NetworkDataSIT(string NPath, string NIP, int NPort, int NoOfAgents, int NoOfJuntions, string VdatTab, string SITtab)
            : base(NPath, NIP, NPort, NoOfAgents, NoOfJuntions)
        {
            SIT = new ParamicsPuppetMaster.SITData("paramicsdb", "localhost", "root", "trg", SITtab, NoOfAgents, NoOfJuntions);
            PDB = new ParamicsPuppetMaster.ParamicsDBiLite("paramicsdb", "localhost", "root", "trg", VdatTab);
            LTB = new ParamicsPuppetMaster.LoopTable("paramicsdb", "localhost", "root", "trg", "LoopTable");
        }

        public void LoopExtract(int Modtim, int dt)
        {
            ParamicsPuppetMaster.ReadPointLoopFiles RLF = new ParamicsPuppetMaster.ReadPointLoopFiles(Path, Modtim, dt);
            foreach (ParamicsPuppetMaster.LoopRecord record in RLF.LoopSensors)
            {
                LTB.AddLoopLine(record.Name, record.StartRecord, record.EndRecord, record.Count, record.AverageSpeed());
            }
        }

    }

}