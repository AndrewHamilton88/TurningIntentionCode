using System;
using System.Collections.Generic;
using System.Text;

namespace ParamicsPuppetMaster
{
    class TestMethod02
    {
        //*Class Members
        static string NetworkPath = @"C:\Program Files\Paramics 2000\.paramics\data\samples\JD-Tutorial\Simple junction";
        static string Database = "paramicsdb";
        static string Machine = "T2027-PARAMICS";
        static string UserName = "sb4p07";
        static string Password = "trg";

        //Class Constructor
        public TestMethod02() 
        {
            ParamicsDBi PDB = new ParamicsDBi(Database, Machine, UserName, Password);//Create an object to link to the paramics database
            PDB.ClearTableContents();//empty any old tables in the database
            EditNodes EN = new EditNodes(NetworkPath);//Read in the data on the nodes in the network
            EditZones EZ = new EditZones(NetworkPath);//Read in the data on the Zones in the network
            //EditPaths EP = new EditPaths(NetworkPath);

            RunParamicsNetwork Runner = new RunParamicsNetwork(NetworkPath);//Define class to open and run the paramics simulation remotely
            Runner.RunNetwork();//Run the paramics simulation
            ReadEventsFile REF = new ReadEventsFile(NetworkPath);//Read simulation output
            ReadSnapshotFile RSN = new ReadSnapshotFile(NetworkPath);

            CollateVehicelData CVD = new CollateVehicelData(REF, RSN, EZ, EN, PDB);
            CVD.AddStep(0,0);//collate the output data and write it to the database

            EditNetwork ENW = new EditNetwork(NetworkPath);// define a class for editing the network

            for (int i = 1; i < 4; i++)
            {
                ENW.SelectSnap();//change the network snapshot file so that it represents the final state of the previous run

                for (int j = 1; j < 4; j++)
                {
                    Runner.RunNetworkSnap();//run the network from the new snapshot

                    REF = new ReadEventsFile(NetworkPath);//read the simulation outputs
                    RSN = new ReadSnapshotFile(NetworkPath);

                    CVD.UpdateFiles(REF, RSN, EZ, EN, PDB);
                    CVD.AddStep(i, j);//collate the output data and write it to the database

                }
                //Select scenario 3 to proceed with
                int[] DumpedScenarios = new int[2] { 1, 2 };
                PDB.MarkObsolete(i, DumpedScenarios);
            }

        }
    }
}
