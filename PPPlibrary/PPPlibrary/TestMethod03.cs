using System;
using System.Collections.Generic;
using System.Text;

namespace ParamicsPuppetMaster
{
    class TestMethod03
    {
        //*Class Members
        static string NetworkPath = @"C:\Program Files\MOVA\PCMOVA 1.1.8\Example Data\PARAMICS\Simple Junction\Simple";
        static string Database = "paramicsdb";
        static string Machine = "localhost";
        static string UserName = "root";
        static string Password = "trg";

        //Class Constructor
        public TestMethod03() 
        {
            /*ParamicsDBiLite PDB = new ParamicsDBiLite(Database, Machine, UserName, Password);//Create an object to link to the paramics database

            List<double[]> SpD = PDB.GetSpeedAndDistane("AtTime = '00:00:30' AND((OnLink = '3:1' AND (LaneNum = 1 )) )");
            
            //PDB.ClearTableContents();//empty any old tables in the database
            EditNodes EN = new EditNodes(NetworkPath);//Read in the data on the nodes in the network
            
            ReadSnapshotFileLite RSN = new ReadSnapshotFileLite(NetworkPath);

            LiteToPDB TDB = new LiteToPDB(RSN,EN,PDB);
            TDB.AddLineToDB();*/

            /*BidDataTable BDT = new BidDataTable(Database, Machine, UserName, Password);
            BDT.ClearTableContents();
            //BDT.AddBidLine("00:00:30", 0.23, 1.51, 0.789, 2);
            BDT.AddBidLine("00:00:10", 0.23, 1.51, 0.789, 1);

            TwinBidDataTable TDT = new TwinBidDataTable(Database, Machine, UserName, Password);
            TDT.ClearTableContents();
            TDT.AddBidLine("00:00:20", 1.2, 1.3, 1.4, 2.1, 2.2, 2.3, 2, 1);
            */
         
        }

    }
}
