using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;

namespace ParamicsPuppetMaster
{
    public class GISdbConnect
    {
        //*Class Members
        public String NetworkPath;
        public String DataBaseDSN;
        public int[] BoundingBox = new int[4];
        private OdbcConnection Connect;
        private List<N_W_Node> NodesList = new List<N_W_Node>();
        private List<N_W_Link> LinksList = new List<N_W_Link>();
        private int AnodeCounter;

        //*Class Constructor
        public GISdbConnect(String S1, String S2, int[] da1)
        {
            //Assign the class memvers
            NetworkPath = S1;
            DataBaseDSN = S2;
            BoundingBox = da1;


            //Create the database connection object
            try
            {
                Connect = new OdbcConnection(DataBaseDSN);
                Connect.Open();
                Connect.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection To GIS databse failed, check the DSN parameter");
                Console.WriteLine("System error info:"); 
                Console.WriteLine(e);
            }
            //Initialize A node counter
            AnodeCounter = 0;
        }

        //*Function To create a reduced Nodes table containing only the data within the bounding box
        public void ShrinkNodesTable()
        {
            String Xmin = BoundingBox[0].ToString();
            String Ymin = BoundingBox[1].ToString();
            String Xmax = BoundingBox[2].ToString();
            String Ymax = BoundingBox[3].ToString();

            OdbcCommand DropExistingTable = new OdbcCommand("DROP TABLE IF EXISTS \"Road_Node_point_Shrunk\";",Connect);
            OdbcCommand CreateNewTable = new OdbcCommand("CREATE TABLE \"Road_Node_point_Shrunk\" AS SELECT * FROM \"Road_Node_point\" WHERE x(the_geom) > " + Xmin + " AND x(the_geom) < " + Xmax + " AND y(the_geom) > " + Ymin + " AND y(the_geom) < " + Ymax + ";",Connect); 
            Connect.Open();
            DropExistingTable.ExecuteNonQuery();
            CreateNewTable.ExecuteNonQuery();

            OdbcCommand DropLinksTable = new OdbcCommand("DROP TABLE IF EXISTS \"Road_Link_polyline_Shrunk\";", Connect);
            OdbcCommand CreateLinksTable = new OdbcCommand("CREATE TABLE \"Road_Link_polyline_Shrunk\" AS SELECT * FROM \"Road_Link_polyline\" WHERE EXISTS(SELECT \"TOID\" FROM \"Road_Node_point_Shrunk\" WHERE \"TOID\" = \"START_NODE\") AND EXISTS(SELECT \"TOID\" FROM \"Road_Node_point_Shrunk\" WHERE \"TOID\" = \"END_NODE\") AND (length(the_geom) > 6.0) AND (\"START_NODE\" != \"END_NODE\");", Connect);

            DropLinksTable.ExecuteNonQuery();
            CreateLinksTable.ExecuteNonQuery();
            Connect.Close();
        }

        //*Function To parse data from the GIS database to the paramics model
        public void ParseNetwork()
        {

            
            //Get nodes data and write Nodes file
            ReadNodesFromGISdb();
            SuperProcessLinksData();
            //ReadLinksFromGISdb();

            EditNodes NodesFileEditor = new EditNodes(NetworkPath, NodesList);
            NodesFileEditor.WriteNodeFile(BoundingBox);

            //Get Links data and write Links file
            //ReadLinksFromGISdb();

            EditLink LinksFileEditor = new EditLink(NetworkPath, LinksList);
            LinksFileEditor.WriteLinksFile();
        }

        //*Function to read Nodes data from database 
        private void ReadNodesFromGISdb()
        {
            List<N_W_Node> LN1 = new List<N_W_Node>();
            
            OdbcCommand C1 = new OdbcCommand("SELECT asText(the_geom), \"TOID\" FROM \"Road_Node_point_Shrunk\";", Connect);
            Connect.Open();
            OdbcDataReader Reader = C1.ExecuteReader();
            

            while (Reader.Read() == true)
            {
                String[] SplitLine = Reader.GetString(0).Split(new Char[] { '(', ' ', ')' });
                String Sname = Reader.GetString(1);
                N_W_Node TempNode = new N_W_Node(Sname, Convert.ToDouble(SplitLine[1]), Convert.ToDouble(SplitLine[2]), 0);
                TempNode.ShrinkName();
                LN1.Add(TempNode);
            }
            
            Connect.Close();
            NodesList = LN1;
            
        }

        //*Function for reading The Links Data from the database
        private void ReadLinksFromGISdb()
        {
            List<N_W_Link> LL1 = new List<N_W_Link>();

            OdbcCommand C2 = new OdbcCommand("SELECT \"START_NODE\",\"END_NODE\",\"LEGEND\" FROM \"Road_Link_polyline\" WHERE EXISTS(SELECT \"TOID\" FROM \"Road_Node_point_Shrunk\" WHERE \"TOID\" = \"START_NODE\") AND EXISTS(SELECT \"TOID\" FROM \"Road_Node_point_Shrunk\" WHERE \"TOID\" = \"END_NODE\") AND (length(the_geom) > 6.0) AND (\"START_NODE\" != \"END_NODE\");", Connect);
            Connect.Open();
            OdbcDataReader R2 = C2.ExecuteReader();
            

            while (R2.Read() == true)
            {
                Char[] CStartNode = R2.GetString(0).ToCharArray(9, 7);
                String SStarNode = new String(CStartNode);
                Char[] CEndNode = R2.GetString(1).ToCharArray(9, 7);
                String SEndNode = new String(CEndNode);
                N_W_Link TempLink = new N_W_Link(SStarNode, SEndNode, 1,false);
                LL1.Add(TempLink);
            }
            Connect.Close();
            LinksList = LL1;
        }
        //Function to get additional nodes from link steps and to get the mini links in between.
        private void SuperProcessLinksData()
        {
            OdbcCommand C2 = new OdbcCommand("SELECT \"START_NODE\",\"END_NODE\",\"LEGEND\",astext(the_geom),\"TOID\" FROM \"Road_Link_polyline_Shrunk\";", Connect);
            Connect.Open();
            OdbcDataReader R2 = C2.ExecuteReader();
            while (R2.Read() == true)
            {
                //Check for membership of One way table**************************
                
                String LinkTOID = R2.GetString(4);
                OdbcCommand C3 = new OdbcCommand("SELECT \"DIR_LINK\" FROM \"Road_Route_Info_polyline\" WHERE \"DIR_LINK\" LIKE '%" + LinkTOID + "' AND \"LEGEND\" LIKE 'One way%';", Connect);
                OdbcDataReader R3 = C3.ExecuteReader();
                int CountCheck = 0;
                String OneWayLine = null;
                try
                {
                    while (R3.Read() == true)
                    {
                        OneWayLine = R3.GetString(0);
                        CountCheck++;
                    }
                    if (CountCheck > 1)
                    {
                        throw new Exception("Multiple correlations were found between the links table and the one way table");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //***************************************************************

                //Set up road catagory*******************************************
                String Legend = R2.GetString(2);
                int Catagory = 1;
                bool OneWay = false;
                bool Reverse = false;
                if (Legend.Contains("dual"))
                {
                    Catagory = 2;
                }
                if (OneWayLine != null)
                {
                    OneWay = true;
                    if (OneWayLine.Contains("-"))
                    {
                        Reverse = true;
                    }
                }

                //***************************************************************
                
                
                //Set up start and end Nodes*************************************
                //Char[] Csnode = R2.GetString(0).ToCharArray(9, 7);
                String FirstNode = R2.GetString(0);

                //Char[] Cenode = R2.GetString(1).ToCharArray(9, 7);
                String SecondNode = R2.GetString(1);

                String GeoSt = R2.GetString(3);
                List<String[]> NodeCoordinates = GeometryToPoints(GeoSt);
                int NumberOfNodes = NodeCoordinates.Count;
                int FinalIndex = NumberOfNodes - 1;
                String Ssnode, Senode;

                if (Reverse == true)
                {
                    NodeCoordinates.Reverse();
                    Ssnode = SecondNode;
                    Senode = FirstNode;
                }
                else
                {
                    Ssnode = FirstNode;
                    Senode = SecondNode;
                }
                N_W_Node StartNode = new N_W_Node(Ssnode, Convert.ToDouble(NodeCoordinates[0][0]), Convert.ToDouble(NodeCoordinates[0][1]), 0);
                N_W_Node EndNode = new N_W_Node(Senode, Convert.ToDouble(NodeCoordinates[FinalIndex][0]), Convert.ToDouble(NodeCoordinates[FinalIndex][1]), 0);

                
                StartNode.ShrinkName();
                EndNode.ShrinkName();
                N_W_Node LastNode = StartNode;

                String ANodeName = "";
                //******************************************************************

                

                for(int i = 1; i < FinalIndex; i++)
                {
                    ANodeName = ("A" + AnodeCounter.ToString("00000"));
                    
                    N_W_Node NewNode = new N_W_Node(ANodeName,Convert.ToDouble(NodeCoordinates[i][0]),Convert.ToDouble(NodeCoordinates[i][1]),0);
                    
                    double NodeDistS = NodeDistanceMeasure(NewNode,StartNode);
                    double NodeDistE = NodeDistanceMeasure(NewNode,EndNode);
                    double NodeDistL = NodeDistanceMeasure(NewNode,LastNode);

                    
                    if(NodeDistS > 5 && NodeDistE > 5 && NodeDistL > 5)
                    {

                        NodesList.Add(NewNode);
                        N_W_Link NewLink = new N_W_Link(LastNode.NodeNum, NewNode.NodeNum, Catagory, OneWay);
                        LinksList.Add(NewLink);
                        LastNode = NewNode;
                        AnodeCounter++;
                    }

                }
                N_W_Link LastLink = new N_W_Link(LastNode.NodeNum, EndNode.NodeNum, Catagory, OneWay);
                LinksList.Add(LastLink);
            }
            Connect.Close();
        }

        private List<String[]> GeometryToPoints(String GeomString)
        {
            List<String[]> ListNodeCoords = new List<string[]>();

            String[] PairStrinArr = GeomString.Split(new char[] {'(',',',')'});

            foreach(String pair in PairStrinArr)
            {
                if (!pair.Contains("LINESTRING") && !pair.Equals(""))
                {
                    String[] NodeCoords = pair.Split(' ');
                    ListNodeCoords.Add(NodeCoords);
                }
            }
           
            return (ListNodeCoords);    
        }

        private double NodeDistanceMeasure(N_W_Node Node1, N_W_Node Node2)
        {
            double X = Math.Abs(Node1.X - Node2.X);
            double Y = Math.Abs(Node1.Y - Node2.Y);
            return(Math.Pow((Math.Pow(X,2)+Math.Pow(Y,2)),0.5));
        }
    }
}
