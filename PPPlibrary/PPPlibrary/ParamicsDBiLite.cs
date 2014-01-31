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

    public class ParamicsDBiLite
    {
        //*Class Members
        public string VDataTab;
        public string DBconnectString; // = "Database=ParamicsDB; Data Source=t2027-paramics; User Id=sb4p07; Password=trg";
        public MySqlConnection DBconnect;// = new MySqlConnection(DBconnectString);


        //*Constructors 
        //public ParamicsDBiLite() { VDataTab = "VehicleDataSimple"; }
        public ParamicsDBiLite() { VDataTab = "LinkTurningMovements"; }
        public ParamicsDBiLite(string Database, string Source, string user, string password, string TableName)
        {
            VDataTab = TableName;
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);

            string BuildVDataTab = ("CREATE TABLE IF NOT EXISTS " + VDataTab);
            BuildVDataTab += " (AtTime TIME, OnLink VARCHAR(50), AlongLink DOUBLE, Speed DOUBLE, VehicleType INT, LaneNum INT,BornTime TIME, Source VARCHAR(15), NextLink INT, NextNextLink INT, NextTurn VARCHAR(15), NextNextTurn VARCHAR(15),  PRIMARY KEY (AtTime,OnLink,AlongLink,LaneNum)); ";

            MySqlCommand MakeTable = new MySqlCommand((BuildVDataTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();

        }

        
        //*Class Functions

        //Function removes the tables from the database COMPLETELY
        public void DropTableFromParamicsDB()
        {
            string DeleteTable = ("DROP TABLE IF EXISTS " + VDataTab + ";");

            MySqlCommand DropTable = new MySqlCommand(DeleteTable, DBconnect);

            DropTable.Connection.Open();
            DropTable.ExecuteNonQuery();
            DropTable.Connection.Close();
        }

        //Function clears all the data from TableName but leaves the table.
        public void ClearTableContents()
        {
            string ClearTableLine = ("DELETE FROM " + VDataTab + ";");

            MySqlCommand ClearTable = new MySqlCommand(ClearTableLine, DBconnect);

            ClearTable.Connection.Open();
            ClearTable.ExecuteNonQuery();
            ClearTable.Connection.Close();
        }

        //Function for adding a line to ViDTab
        public void VDataAddLine(string[] iDdat)
        {
            string TheLine = (" REPLACE INTO " + VDataTab + " VALUES(");
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

        public int CountInVDataTab(string Condition)
        {
            string QueryLine = ("SELECT COUNT(*) FROM " + VDataTab + " WHERE ");
            QueryLine += (Condition + ";");

            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();

            int Count = 0;

            while (RecordData.Read())
            {
                Count = RecordData.GetInt32(0);
            }

            RecordData.Close();
            LookUpData.Connection.Close();

            return (Count);
            
        }

        public List<double[]> GetSpeedAndDistane(string Condition)
        {
            string QueryLine = ("SELECT Speed, AlongLink FROM " + VDataTab + " WHERE ");
            QueryLine += (Condition + ";");

            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();
            

            List<double[]> SpeedsDist = new List<double[]>();

            while (RecordData.Read())
            {
                double[] SpD = new double[2];
                SpD[0] = RecordData.GetDouble(0);
                SpD[1] = RecordData.GetDouble(1);
                SpeedsDist.Add(SpD);

            }

            RecordData.Close();
            LookUpData.Connection.Close();

            return (SpeedsDist);
            
        }

        public List<string[]> GetTurningDirections(string Condition) //AH Function - same as above but strings instead of doubles, returns string[2] where [0] = NextTurn and [1] = NextNextTurn
        {
            string QueryLine = ("SELECT NextTurn, NextNextTurn FROM " + VDataTab + " WHERE ");
            QueryLine += (Condition + ";");

            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();


            List<string[]> NextAndNextNextTurn = new List<string[]>();

            while (RecordData.Read())
            {
                string[] SpD = new string[2];
                SpD[0] = RecordData.GetString(0);
                SpD[1] = RecordData.GetString(1);
                NextAndNextNextTurn.Add(SpD);
            }

            RecordData.Close();
            LookUpData.Connection.Close();

            return (NextAndNextNextTurn);

        }

        public void GetVLifeTime(string Condition, ref double LifeTime, ref int NumV)
        {
            string QueryLine = ("SELECT SUM(TIME_TO_SEC(SUBTIME(AtTime,BornTime))),COUNT(*) FROM " + VDataTab + " WHERE ");
            QueryLine += (Condition + ";");
            
            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();


            LifeTime = 0;
            NumV = 0;

            try
            {
                while (RecordData.Read())
                {
                    LifeTime = RecordData.GetDouble(0);
                    NumV = RecordData.GetInt32(1);
                }
            }
            catch (Exception e)
            {
                //let this fail quietly and log zero's because it is probably because there are no vehicles in the sim.
            }

            RecordData.Close();
            LookUpData.Connection.Close();

        }

        public void DBFinalShutdown()
        {
            DBconnect.Dispose();
        }
       
  
    }

    public class BidDataTable : ParamicsDBiLite
    {
        protected int
            ANum,
            JNum;

        //*Constructors 
        public BidDataTable() { VDataTab = "BidData"; }
        public BidDataTable(string Database, string Source, string user, string password, int AgtNum, int JcnNum)
        {
            ANum = AgtNum;
            JNum = JcnNum;
            VDataTab = "BidData";
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);

            DropTableFromParamicsDB();

            string BuildBidDataTab = ("CREATE TABLE " + VDataTab + "(AtTime TIME");
            for (int i = 1; i <= AgtNum; i++)
            {
                BuildBidDataTab += ", Bid" + i.ToString() + " DOUBLE";
            }
            for (int i = 1; i <= JcnNum; i++)
            {
                BuildBidDataTab += ", Stage" + i.ToString() + " INT";
            }
            BuildBidDataTab += ",VehicleCount INT, TotalLifeSpan  DOUBLE";
            BuildBidDataTab += ", PRIMARY KEY (AtTime)); ";

            MySqlCommand MakeTable = new MySqlCommand((BuildBidDataTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();

        }

        //*Function to add bid data to the database
        public void AddBidLine(string Time, double[] Bids, int[] Decisions, int Vcount, double LifeSpan)
        {
            //Check that the dimensions of the arrays match the database colums
            if (Bids.Length != ANum || Decisions.Length != JNum)                            //AH cheated by multiplying the ANum by 3 so that it could record all 12 phase bids instead of stage bids
            {
                throw new Exception("The line you are trying to add to the database doesn't match the number of colums");
            }

            string TheLine = (" REPLACE INTO " + VDataTab + " VALUES('" + Time + "'");
            foreach (double d in Bids)
            {
                TheLine += ", " + d.ToString();
            }
            foreach (int i in Decisions)
            {
                TheLine += ", " + i.ToString();
            }
            TheLine += ", " + Vcount.ToString();
            TheLine += ", " + LifeSpan.ToString();
            TheLine += ");";

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }

    }


    public class SITData : BidDataTable
    {
         //*Constructors 
        public SITData() { VDataTab = "SITData"; }
        public SITData(string Database, string Source, string user, string password, string TableName, int AgtNum, int JcnNum)
        {
            ANum = AgtNum;
            JNum = JcnNum;
            VDataTab = TableName;
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);

            DropTableFromParamicsDB();

            string BuildSITDataTab = ("CREATE TABLE " + VDataTab + "(AtTime TIME, Zone INT, ZoneName VARCHAR(15), Vcount DOUBLE, AvSpeed DOUBLE, AvDist DOUBLE, Source VARCHAR(15), PRIMARY KEY (AtTime,Zone)); ");

            MySqlCommand MakeTable = new MySqlCommand((BuildSITDataTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();

        }

        //*Function to add bid data to the database
        public void AddSITLine(string Time, int zone, string name, double vcount, double AvSp, double AvX, string source)
        {
            string TheLine = (" REPLACE INTO " + VDataTab + " VALUES('" + Time + "'");
            
            TheLine += ", " + zone.ToString();
            TheLine += ", '" + name;
            TheLine += "', " + vcount.ToString();
            TheLine += ", " + AvSp.ToString();
            TheLine += ", " + AvX.ToString();
            TheLine += ", '" + source + "'";
            TheLine += ");";

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }
    }

    public class LoopTable : BidDataTable
    {
        //*Constructors 
        public LoopTable() { VDataTab = "LoopTable"; }
        public LoopTable(string Database, string Source, string user, string password, string TableName)
        {
            VDataTab = TableName;
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);

            DropTableFromParamicsDB();

            string BuildSITDataTab = ("CREATE TABLE " + VDataTab + "(LoopName VARCHAR(30), StartTime TIME, EndTime TIME, VCount INT, AvSpeed DOUBLE, PRIMARY KEY (LoopName, EndTime)); ");

            MySqlCommand MakeTable = new MySqlCommand((BuildSITDataTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();
        }

        //*Function to add bid data to the database
        public void AddLoopLine(string Name, DateTime StartTime, DateTime EndTime, int vcount, double AvSp)
        {
            string TheLine = (" REPLACE INTO " + VDataTab + " VALUES('" + Name + "'");

            TheLine += ", '" + StartTime.TimeOfDay.ToString();
            TheLine += "', '" + EndTime.TimeOfDay.ToString();
            TheLine += "', " + vcount.ToString();
            TheLine += ", " + AvSp.ToString();
            TheLine += ");";

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();

        }

        public void getCountAndSpeed(string Name, string EndTime, ref int vcount, ref double AvSp, ref TimeSpan StartTime)
        {
            string QueryLine = ("SELECT VCount, AvSpeed, StartTime FROM " + VDataTab + " WHERE ");
            QueryLine += "(LoopName LIKE '%" + Name + "%') AND ";
            QueryLine += "EndTime = '" + EndTime + "';";

            MySqlCommand LookUpData = new MySqlCommand(QueryLine, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();



            while (RecordData.Read())
            {
                //TODO come back to this
                vcount = RecordData.GetInt32("VCount");
                AvSp = RecordData.GetDouble("AvSpeed");
                string TimeString = RecordData.GetString("StartTime");
                StartTime = Convert.ToDateTime(TimeString).TimeOfDay;
            }

            RecordData.Close();
            LookUpData.Connection.Close();

            
        }

    }


    public class PlayerScoreTable : ParamicsDBiLite
    {
        public PlayerScoreTable(string Database, string Source, string user, string password)
        {
            VDataTab = "PlayerScores";
            DBconnectString = ("Database=" + Database + "; Data Source=" + Source + "; User Id=" + user + "; Password=" + password + ";");

            DBconnect = new MySqlConnection(DBconnectString);


            string BuildBidDataTab = ("CREATE TABLE IF NOT EXISTS " + VDataTab + "(GameID INT, SysDate DATE, SysTime TIME");
            BuildBidDataTab += ", PlayerName VARCHAR(30), ScoreOne DOUBLE, ScoreTwo DOUBLE, AggScore  DOUBLE";
            BuildBidDataTab += ", PRIMARY KEY (GameID)); ";

            MySqlCommand MakeTable = new MySqlCommand((BuildBidDataTab), DBconnect);

            MakeTable.Connection.Open();
            MakeTable.ExecuteNonQuery();
            MakeTable.Connection.Close();

            //OverWriteRowsWithGID();
        }

        public int GetGameID()
        {
            int GameID = -2;
            string TheLine = "SELECT MAX(GameID) from PlayerScoreSchema." + VDataTab + ";";

            MySqlCommand GetGameID = new MySqlCommand((TheLine), DBconnect);
            GetGameID.Connection.Open();


            MySqlDataReader RecordData;
            RecordData = GetGameID.ExecuteReader();

            try
            {
                while (RecordData.Read())
                {
                    if (RecordData.HasRows)
                    {
                        GameID = RecordData.GetInt32(0);
                    }
                }
            }
            catch
            {
                //do nothing for now. - means there's nothing in the database
            }


            RecordData.Close();
            GetGameID.Connection.Close();

            return GameID;
        }



        public void AddPlayerLine(string PlayerName, int Gid)
        {
            double ScoreOne = 0.0;
            double ScoreTwo = 0.0;
            double AggScore = 0.0;

            string TheLine = (" REPLACE INTO PlayerScoreSchema." + VDataTab + " VALUES('" + Gid.ToString() + "', '");
            TheLine += (DateTime.Now.Date.ToString("yyyyMMdd") + "', '");
            TheLine += (DateTime.Now.ToString("HH:mm:ss") + "', '");
            TheLine += (PlayerName + "', ");
            TheLine += (ScoreOne.ToString() + ", ");
            TheLine += (ScoreTwo.ToString() + ", ");
            TheLine += (AggScore.ToString() + ");");

            MySqlCommand AddLine = new MySqlCommand(TheLine, DBconnect);

            AddLine.Connection.Open();
            AddLine.ExecuteNonQuery();
            AddLine.Connection.Close();
        }

        public void AddScoreLine(double ScoreOne, double ScoreTwo, double AggScore, int GameID)
        {
            string TheLine = ("UPDATE " + VDataTab + " SET ScoreOne = " + Convert.ToString(ScoreOne));
            TheLine += (", ScoreTwo = " + Convert.ToString(ScoreTwo));
            TheLine += (", AggScore = " + Convert.ToString(AggScore));
            TheLine += (" WHERE GameID = " + Convert.ToString(GameID) + ";");

            MySqlCommand ModifyLine = new MySqlCommand(TheLine, DBconnect);

            ModifyLine.Connection.Open();
            ModifyLine.ExecuteNonQuery();
            ModifyLine.Connection.Close();

        }

        public string GetTopScores(int Number, DateTime sinceDateTime)
        {
            string Returner = "";

            string TheLine = ("SELECT * FROM " + VDataTab);
            TheLine += " WHERE AggScore > 0 AND SysDate >= '" + sinceDateTime.ToString("yyyyMMdd") + "' AND SysTime >= '" + sinceDateTime.ToString("HH:mm:ss");
            TheLine += "' ORDER BY AggScore ASC LIMIT " + Convert.ToString(Number) + ";";

            MySqlCommand GetScores = new MySqlCommand((TheLine), DBconnect);
            GetScores.Connection.Open();


            MySqlDataReader RecordData;
            RecordData = GetScores.ExecuteReader();

            while (RecordData.Read())
            {
                if (RecordData.HasRows)
                {
                    Returner += RecordData.GetString(3) + ",";
                    Returner += Convert.ToString(RecordData.GetDouble(4)) + ",";
                    Returner += Convert.ToString(RecordData.GetDouble(5)) + ",";
                    Returner += Convert.ToString(RecordData.GetDouble(6)) + ";";
                }
            }

            RecordData.Close();
            GetScores.Connection.Close();

            return Returner;
        }
    }


}