//Class containing some usefull functions to  
//extract data from the paramics database

//S.Box 24 July 2008

using System;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace ParamicsPuppetMaster
{

    /*public class GetDataFromPDB : ParamicsDBi
    {
        public GetDataFromPDB(string TableName)
            : base(TableName)
        {
        }
        public void PrintVehicleSpeed(double TimeSt, int TagNo)
        {
            string DBquery = ("SELECT Speed FROM " + TableName + " WHERE (TIMESTAMP = " + TimeSt + " AND TagNUmber = " + TagNo + ");");

            MySqlCommand LookUpData = new MySqlCommand(DBquery, DBconnect);

            LookUpData.Connection.Open();

            MySqlDataReader RecordData;
            RecordData = LookUpData.ExecuteReader();

            while (RecordData.Read())
            {
                Console.WriteLine("Vehicle Speed is " + RecordData.GetDouble(0) + " m/s");
            }

            RecordData.Close();

            LookUpData.Connection.Close();

        }
    }*/
}