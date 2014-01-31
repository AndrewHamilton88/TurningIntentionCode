//Class containing some usefull functions to manage a 
//database of Paramics vehicle data

//S.Box 24 July 2008

using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace ParamicsPuppetMaster
{

    public class ParamicsDBi
    {
        //*Class Members
        public static string ViDTab = "VehicleIdentity";
        public static string VposTab = "VehiclePosition";
        public string DBconnectString; // = "Database=ParamicsDB; Data Source=t2027-paramics; User Id=sb4p07; Password=trg";
        public MySqlConnection DBconnect;// = new MySqlConnection(DBconnectString);


        //*Constructors 
        public ParamicsDBi() { }
        public ParamicsDBi(string Database, string Source, string user, string password)
        {
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);

            string BuildVidTab = ("CREATE TABLE IF NOT EXISTS " + ViDTab);
            BuildVidTab += " (ViD VARCHAR(5), VehicleType INT, BornTime TIME, BornLink VARCHAR(7), RouteName VARCHAR(15), Origin INT, Destination INT, ParamicsTag INT, BornStage INT, BornScenario INT, NextLink INT, NextNextLink INT, Obsolete BOOL, MagicNumbers VARCHAR(13), Misc1 VARCHAR(15), Misc2 VARCHAR(15), PRIMARY KEY (ViD)); ";

            string BuildVposTab = ("CREATE TABLE IF NOT EXISTS " + VposTab);
            BuildVposTab += " (ViD VARCHAR(5), AtTime TIME, OnLink VARCHAR(7), AlongLink DOUBLE, Speed DOUBLE, Xpos DOUBLE, Ypos DOUBLE, Zpos DOUBLE, InStage INT, InScenario INT, NextLink INT, NextNextLink INT, Obsolete BOOL, Misc1 VARCHAR(15), Misc2 VARCHAR(15), FOREIGN KEY (ViD) REFERENCES " + ViDTab + "(ViD), PRIMARY KEY (ViD,AtTime,InStage,InScenario)); ";


            MySqlCommand MakeTable = new MySqlCommand((BuildVidTab + BuildVposTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();

        }

        
        //*Class Functions

        //Function removes the tables from the database COMPLETELY
        public void DropTableFromParamicsDB()
        {
            string DeleteTable = ("DROP TABLE " + VposTab + "; DROP TABLE " + ViDTab + ";");

            MySqlCommand DropTable = new MySqlCommand(DeleteTable, DBconnect);

            DropTable.Connection.Open();
            DropTable.ExecuteNonQuery();
            DropTable.Connection.Close();
        }

        //Function clears all the data from TableName but leaves the table.
        public void ClearTableContents()
        {
            string ClearTableLine = ("DELETE FROM " + VposTab + "; DELETE FROM " + ViDTab + ";");

            MySqlCommand ClearTable = new MySqlCommand(ClearTableLine, DBconnect);

            ClearTable.Connection.Open();
            ClearTable.ExecuteNonQuery();
            ClearTable.Connection.Close();
        }

        //Function for adding a line to ViDTab
        public void ViDAddLine(string[] iDdat)
        {
            string TheLine = (" REPLACE INTO " + ViDTab + " VALUES(");
            foreach (string iDd in iDdat)
            {
                TheLine += (iDd + ",");
            }
            TheLine = TheLine.Remove((TheLine.Length - 1));
            TheLine += ");";

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }

        //overload
        public void ViDAddLine(List<string[]> LiDdat)
        {
            string MultiLine = " ";
            foreach (string[] iDdat in LiDdat)
            {
                string TheLine = (" REPLACE INTO " + ViDTab + " VALUES(");
                foreach (string iDd in iDdat)
                {
                    TheLine += (iDd + ",");
                }
                TheLine = TheLine.Remove((TheLine.Length - 1));
                TheLine += ");";

                MultiLine += TheLine;

                
            }
            MySqlCommand AddLine = new MySqlCommand(MultiLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }

        //function for adding a line to VposTab
        public void VposAddLine(string[] iDdat)
        {
            string TheLine = (" INSERT INTO " + VposTab + " VALUES(");
            foreach (string iDd in iDdat)
            {
                TheLine += (iDd + ",");
            }
            TheLine = TheLine.Remove((TheLine.Length - 1));
            TheLine += ");";

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }

        public List<VehicleIdentity> VidGrabLine(string Condition)
        {
            string QueryLine = ("SELECT * FROM " + ViDTab + " WHERE ");
            QueryLine += (Condition + ";");

            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            List<VehicleIdentity> TempIDL = new List<VehicleIdentity>();
            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();
            
            while (RecordData.Read())
            {
                VehicleIdentity TempID = new VehicleIdentity();
                TempID.ViD = RecordData.GetString(0);
                TempID.Vtype = RecordData.GetInt32(1);
                TempID.BornAt = DateTime.Parse(RecordData.GetString(2));
                TempID.BornLink = new LinkID(RecordData.GetString(3));
                if (!RecordData.IsDBNull(4))
                {
                    TempID.Routename = RecordData.GetString(4);
                }
                if (!RecordData.IsDBNull(5))
                {
                    TempID.Origin = RecordData.GetInt32(5);
                }
                if (!RecordData.IsDBNull(6))
                {
                    TempID.Destination = RecordData.GetInt32(6);
                }
                TempID.Tag = RecordData.GetInt32(7);
                TempID.BornStage = RecordData.GetInt32(8);
                TempID.BornScenario = RecordData.GetInt32(9);
                TempID.Obsolete = RecordData.GetBoolean(10);
                
                if (!RecordData.IsDBNull(11))
                {
                    TempID.MagicNumbers = RecordData.GetString(11);
                }

                TempIDL.Add(TempID);

            }

            RecordData.Close();
            LookUpData.Connection.Close();

            return (TempIDL);
        }

        //Function for marking scenarios as obsolete
        public void MarkObsolete(int Stage, int[] Scenarios)
        {
            foreach (int Scenario in Scenarios)
            {
                string DBcommand = (" UPDATE " + ViDTab + " SET Obsolete = '1' WHERE ");
                DBcommand += ("BornStage = "+ Stage.ToString() + " AND BornScenario = " + Scenario.ToString() + " ;");
                DBcommand += (" UPDATE " + VposTab + " SET Obsolete = '1' WHERE ");
                DBcommand += ("InStage = " + Stage.ToString() + " AND InScenario = " + Scenario.ToString() + " ;");

                MySqlCommand AddLine = new MySqlCommand(DBcommand, DBconnect);

                AddLine.Connection.Open();
                AddLine.ExecuteNonQuery();
                AddLine.Connection.Close();
 
            }


        }

  
    }
}