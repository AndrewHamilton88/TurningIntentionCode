using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace ParamicsPuppetMaster
{
    public class EditNetwork
    {
        //**Class members

        //Network Path
        public string NetPath;

        //**Class constructor
        public EditNetwork(string pn)
        {
            NetPath = pn;
        }

        //**Class Functions

        //Function for creating a copy of the network
        public void CopyNetwork()
        {
            int L = NetPath.Length;
            string s = NetPath.Substring(L - 2, 2);
            string NewPath;
            if (Char.IsDigit(NetPath[L - 2]) && Char.IsDigit(NetPath[L - 1]))
            {
                int num = Convert.ToInt32(s);
                num++;
                string nums = num.ToString("00");
                NewPath = NetPath.Replace(s, nums);
            }
            else
            {
                NewPath = NetPath.Replace(s, (s + "01"));
            }

            DirectoryInfo Dest = new DirectoryInfo(NewPath);
            DirectoryInfo Source = new DirectoryInfo(NetPath);
            if (Dest.Exists)
            {
                Dest.Delete(true);
            }
            Directory.CreateDirectory(Dest.FullName);
            CopyDirectory(Source, Dest);
            NetPath = NewPath;

        }

        //Select a snapshot to start the simulation from
        public void SelectSnap(string Snaptime)
        {
            DirectoryInfo Log = new DirectoryInfo((NetPath + "\\Log\\"));
            DirectoryInfo[] LogList = Log.GetDirectories();
            int L = LogList.Length;
            FileInfo Snapfile = new FileInfo(LogList[L - 1].FullName + "\\snap-" + Snaptime);
            FileInfo TargetSnap = new FileInfo(NetPath + "\\snapshot");
            Snapfile.CopyTo(TargetSnap.FullName, true);

        }

        //overload to automatically select the last snapshot file
        public void SelectSnap()
        {
            DirectoryInfo Log = new DirectoryInfo((NetPath + "\\Log\\"));
            DirectoryInfo[] LogList = Log.GetDirectories();
            int L = LogList.Length;
            FileInfo[] SnapList = LogList[L - 1].GetFiles();
            FileInfo Snapfile = SnapList[0];

            foreach (FileInfo Snap in SnapList)
            {
                if (Snap.Name.Contains("snap"))
                {
                    Snapfile = Snap;
                }
            }
            FileInfo TargetSnap = new FileInfo(NetPath + "\\snapshot");
            Snapfile.CopyTo(TargetSnap.FullName, true);

        }

        //Function for copying a directory
        static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName,
                    file.Name));
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }
    }


    //**Child class for editing the pathroutes data
    public class Editpathroutes : EditNetwork
    {
        //**Class members
        public string FileName;

        //create lists for the data in pathroutes
        public List<string> RouteNames = new List<string>();
        public List<int> Profile = new List<int>();
        public List<int> Matrix = new List<int>();
        public List<int> Rate = new List<int>();

        //**Constructor
        public Editpathroutes(string NetPath)
            : base(NetPath)
        {
            FileName = "pathroutes";

            using (StreamReader ReadFile = new StreamReader(String.Concat(NetPath, "\\", FileName)))// open the pathroutes file
            {
                try
                {
                    string FileLine;
                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("Path "))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ', '"' });// split the line up into an array of strings
                            //read the data from the file into the class object lists
                            RouteNames.Add(splitline[3]);
                            Profile.Add(Convert.ToInt32(splitline[6]));
                            Matrix.Add(Convert.ToInt32(splitline[8]));
                            Rate.Add(Convert.ToInt32(splitline[10]));
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The pathroutes file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            //Check to make sure that all the lists are the same length
            if (RouteNames.Count != Profile.Count || RouteNames.Count != Matrix.Count || RouteNames.Count != Rate.Count)
            {
                Console.WriteLine("Warning: The lists of data in PathRoutes are not all the same length");
            }

        }


        //**Class Functions

        //use this functions to write any changes made to the data to the PathRoutes file in the Network
        public void WriteChanges()
        {
            using (StreamWriter WriteFile = new StreamWriter(string.Concat(NetPath, "\\", FileName)))
            {
                WriteFile.WriteLine("PathRoutes\n");
                for (int i = 0; i < RouteNames.Count; i++)
                {
                    WriteFile.WriteLine(String.Concat(" Path ", '"', RouteNames[i], '"', " Profile ", Profile[i], " matrix ", Matrix[i], " rate ", Rate[i]));
                }
            }

        }

    }

    //**Child class for editing the Nodes file
    public class EditNodes : EditNetwork
    {
        //*Class Members
        public string FileName;
        public List<N_W_Node> NodeList = new List<N_W_Node>();

        //*Constructors
        public EditNodes(string NetPath)
            : base(NetPath)
        {
            FileName = "nodes";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {
                try
                {
                    string FileLine;
                    N_W_Node temp = new N_W_Node();
                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("node "))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            string NodeNum = splitline[1];
                            string[] splitline2 = FileLine.Split(new Char[] { 't', 'm', ',' });// split the line up into an array of strings
                            double X = Convert.ToDouble(splitline2[1]);
                            double Y = Convert.ToDouble(splitline2[3]);
                            double Z = Convert.ToDouble(splitline2[5]);
                            //temp = new N_W_Node(NodeNum,X,Y,Z);
                            NodeList.Add(new N_W_Node(NodeNum, X, Y, Z));

                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The pathroutes file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }
        public EditNodes(string NetPath, List<N_W_Node> NLin)
            : base(NetPath)
        {
            NodeList = NLin;
            FileName = "nodes";
        }

        //Function for writing the NodeList to the Nodes File in paramics
        public void WriteNodeFile(int[] BoundingBox)
        {
            String Xmin = BoundingBox[0].ToString();
            String Ymin = BoundingBox[1].ToString();
            String Xmax = BoundingBox[2].ToString();
            String Ymax = BoundingBox[3].ToString();

            String BoundaryLine = ("Bounding Box " + Xmin + " m " + Ymin + " m " + Xmax + " m " + Ymax + " m");

            using (StreamWriter WriteFile = new StreamWriter((NetPath + "\\" + FileName))) // create a new text file to write to.
            {
                try
                {
                    WriteFile.WriteLine(BoundaryLine); //Write the firstline containing boundary data
                    foreach (N_W_Node Node in NodeList)
                    {
                        WriteFile.WriteLine("node " + Node.NodeNum + " at " + Node.X.ToString() + " m, " + Node.Y.ToString() + " m, " + Node.Z.ToString() + " m junction");

                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("There was a problem writing Nodes");
                    Console.WriteLine(e.Message);
                }
            }

        }
    }



    //**Child class for editing the paths file
    public class EditPaths : EditNetwork
    {
        //*Class Members
        public string FileName;
        public List<N_W_Path> PathList = new List<N_W_Path>();

        //*Constructor
        public EditPaths(string NetPath)
            : base(NetPath)
        {
            FileName = "paths";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {
                try
                {
                    string FileLine;
                    N_W_Path temp = new N_W_Path();
                    LinkID tLiD = new LinkID();
                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("number "))
                        {
                            temp = new N_W_Path();

                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            //read the data from the file into the class object lists
                            int index = splitline.Length;
                            index--;
                            int num = Convert.ToInt32(splitline[index]);
                            temp.PathIDn = num;
                        }
                        if (FileLine.Contains("name "))
                        {
                            string[] splitline = FileLine.Split(new Char[] { '"' });// split the line up into an array of strings
                            temp.PathName = splitline[1];
                        }
                        if (FileLine.Contains("link "))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ':' });// split the line up into an array of strings
                            string[] splitsplitline1 = splitline[0].Split(new Char[] { ' ' });
                            string[] splitsplitline2 = splitline[1].Split(new Char[] { ' ' });
                            int index1 = splitsplitline1.Length;
                            int index2 = 0;
                            index1--;
                            string num1 = splitsplitline1[index1];
                            string num2 = splitsplitline2[index2];

                            tLiD = new LinkID(num1, num2);
                            temp.PathDescription.Add(tLiD);
                        }
                        if (FileLine.Contains(" end"))
                        {
                            PathList.Add(temp);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The pathroutes file could not be read:");
                    Console.WriteLine(e.Message);
                }

            }
        }

    }

    //**Child class for editing the zones file
    public class EditZones : EditNetwork
    {
        //*Class Members
        public string FileName;
        public List<N_W_Zone> ZoneList = new List<N_W_Zone>();

        //*Constructor
        public EditZones(string NetPath)
            : base(NetPath)
        {
            FileName = "zones";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {
                try
                {
                    string FileLine;
                    N_W_Zone temp = new N_W_Zone();

                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("zone ") && !FileLine.Contains("Count"))
                        {

                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            temp.ZoneNum = Convert.ToInt32(splitline[1]);

                        }
                        else if (FileLine.Contains("m ") && !FileLine.Contains("max") && !FileLine.Contains("min") && !FileLine.Contains("centroid"))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            double[] corner = new double[2] { Convert.ToDouble(splitline[0]), Convert.ToDouble(splitline[2]) };
                            temp.Corners.Add(corner);
                        }
                        else if (FileLine.Contains("max"))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            temp.Max = new double[2] { Convert.ToDouble(splitline[1]), Convert.ToDouble(splitline[3]) };
                        }
                        else if (FileLine.Contains("min"))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            temp.Min = new double[2] { Convert.ToDouble(splitline[1]), Convert.ToDouble(splitline[3]) };
                        }
                        else if (FileLine.Contains("centroid"))
                        {
                            ZoneList.Add(temp);
                            temp = new N_W_Zone();
                        }

                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The pathroutes file could not be read:");
                    Console.WriteLine(e.Message);
                }

            }
        }

    }

    public class EditDetectors : EditNetwork
    {
        //*Class Members
        public string FileName;
        public List<Detector> DetectorList = new List<Detector>();

        //*Constructor
        public EditDetectors(string NetPath)
            : base(NetPath)
        {
            FileName = "detectors";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {
                try
                {
                    string FileLine;
                    Detector temp = new Detector();
                    LinkID tLiD = new LinkID();
                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (!FileLine.Contains("Detector Count"))
                        {
                            string[] splitline = FileLine.Split(new Char[] { '"' });// split the line up into an array of strings
                            temp.Name = splitline[1];

                            string[] splitline2 = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            temp.Type = splitline2[0];
                            temp.LinkDist = Convert.ToDouble(splitline2[5]);
                            tLiD = new LinkID(splitline2[9]);
                            temp.OnLink = tLiD;
                            temp.Length = Convert.ToDouble(splitline2[12]);
                            DetectorList.Add(temp);
                            temp = new Detector();
                        }

                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The detectors file could not be read:");
                    Console.WriteLine(e.Message);
                }

            }
        }

        //*Function for writing a text file listing detector names.
        public void WriteDetectorNames()
        {
            string FileName = "DetectorNameList.txt";

            using (StreamWriter WriteFile = new StreamWriter((NetPath + "\\" + FileName))) // create a new text file to write to.
            {
                try
                {
                    foreach (Detector Dt in DetectorList)
                    {
                        WriteFile.WriteLine(Dt.Name);
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("There was a problem writing DetectorsNameList");
                    Console.WriteLine(e.Message);
                }
            }
        }

    }

    public class EditLink : EditNetwork
    {
        //*Class members
        List<N_W_Link> LinkList = new List<N_W_Link>();
        public string FileName;

        //*Class Constructors
        public EditLink(String NetPath, List<N_W_Link> LLin)
            : base(NetPath)
        {
            FileName = "links";
            LinkList = LLin;
        }

        public EditLink(String NetPath)
            : base(NetPath)
        {
            FileName = "links";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {
                try
                {
                    string FileLine;

                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("link ") && FileLine.Contains("category"))
                        {
                            string[] splitline = FileLine.Split(new Char[] { ' ' });// split the line up into an array of strings
                            string StartNode = splitline[1];
                            string EndNode = splitline[2];
                            int Cat = Convert.ToInt32(splitline[4]);

                            LinkList.Add(new N_W_Link(StartNode, EndNode, Cat, false));//TODO un hardcode false to read oneway data from file

                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The pathroutes file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

        }

        //*Function for writing the "links" file
        public void WriteLinksFile()
        {
            using (StreamWriter WriteFile = new StreamWriter((NetPath + "\\" + FileName))) // create a new text file to write to.
            {
                try
                {

                    foreach (N_W_Link Link in LinkList)
                    {

                        WriteFile.WriteLine("link " + Link.StartNode + " " + Link.EndNode + " category " + Link.category.ToString() + " force merge force across");
                        if (Link.OneWay == false)
                        {
                            WriteFile.WriteLine("link " + Link.EndNode + " " + Link.StartNode + " category " + Link.category.ToString() + " force merge force across toll 0.000");
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("There was a problem writing Links");
                    Console.WriteLine(e.Message);
                }
            }

        }

        //*Function for searching links data to get lane info//TODO impliment this in the database
        public void SearchLaneData(string StartNode, string EndNode, ref int LN, ref double LW)
        {
            bool set = false;
            foreach (N_W_Link Link in LinkList)
            {
                if (Link.StartNode.Equals(StartNode) && Link.EndNode.Equals(EndNode))
                {
                    LN = Link.Lanes;
                    LW = Link.LaneWidth;
                    set = true;
                }
            }
            if (!set)
            {
                Console.WriteLine("The link for which lane data was queries was not found in the EditLinks object. Default values used");
                LN = 1;
                LW = 3.7;
            }
        }

    }

    public class EditConfig : EditNetwork
    {
        public string FileName;
        public EditConfig(String NetPath)
            : base(NetPath)
        {
            FileName = "configuration";
        }

        public void SetDemandRate(double Rate)
        {
            string Contents = "";

            using (StreamReader ReadFile = new StreamReader((NetPath + "\\" + FileName)))// open the paths file
            {

                try
                {
                    string FileLine;
                    while ((FileLine = ReadFile.ReadLine()) != null)//read the file line by line
                    {
                        if (FileLine.Contains("demand weight"))
                        {
                            FileLine = "demand weight " + Rate.ToString() + '\n';
                        }
                        Contents += FileLine + '\n';
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The configuration file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            using (StreamWriter WriteFile = new StreamWriter((NetPath + "\\" + FileName))) // create a new text file to write to.
            {
                try
                {
                    WriteFile.Write(Contents);
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("There was a problem writing configuration file");
                    Console.WriteLine(e.Message);
                }
            }

        }
    }

    public class LinkID
    {
        //*Class members
        public string StartNode;
        public string EndNode;

        //*Constructor
        public LinkID()
        {
            //DEFAULT VALUES
            StartNode = "511";
            EndNode = "511";
        }
        public LinkID(string a, string b)
        {
            StartNode = a;
            EndNode = b;
        }
        public LinkID(string lnk)
        {
            string[] splitstring = lnk.Split(':');
            StartNode = splitstring[0];
            EndNode = splitstring[1];
        }
        public string MakeString()
        {
            return ((StartNode + ":" + EndNode));
        }
    }

    public class N_W_Path
    {
        //*Class Members
        public int PathIDn;
        public string PathName;
        public List<LinkID> PathDescription = new List<LinkID>();

        //*Class Constructor
        public N_W_Path() { }
        public N_W_Path(int a, string b, List<LinkID> c)
        {
            PathIDn = a;
            PathName = b;
            PathDescription = c;
        }
    }

    public class N_W_Node
    {
        //*Class Members
        public string
            NodeNum,
            LongName;
        public double
            X,
            Y,
            Z;

        //*Constructors
        public N_W_Node() { }
        public N_W_Node(string a, double b, double c, double d)
        {
            NodeNum = a;
            X = b;
            Y = c;
            Z = d;
        }
        public void ShrinkName()
        {
            LongName = NodeNum;
            Char[] Cname = LongName.ToCharArray(9, 7);
            NodeNum = new String(Cname);
        }

        public double DistToNode(N_W_Node OtherNode)
        {
            return (Math.Sqrt(Math.Pow(X - OtherNode.X, 2) + Math.Pow(Y - OtherNode.Y, 2) + Math.Pow(Z - OtherNode.Z, 2)));
        }
    }

    public class N_W_Zone
    {
        //*Class Members
        public int ZoneNum;
        public List<double[]> Corners = new List<double[]>();
        public double[]
            Max,
            Min;

        //*Constructors
        public N_W_Zone() { }
    }

    public class Detector
    {
        //*Class Members
        public string Type;
        public string Name;
        public double
            LinkDist,
            Length;
        public LinkID OnLink;

        //*Constructors
        public Detector() { }
    }

    public class N_W_Link
    {
        //*Class Members
        public String
            StartNode,
            EndNode;
        public int
            category,
            Lanes;
        public double
            LaneWidth,
            LinkLength;
        public bool OneWay;

        //*Constructors
        public N_W_Link(String s1, String s2, int i1, bool b1)
        {
            StartNode = s1;
            EndNode = s2;
            category = i1;
            OneWay = b1;

            if (category == 1)
            {
                Lanes = 1;
                LaneWidth = 3.7;
            }
            else if (category == 2)
            {
                Lanes = 2;
                LaneWidth = 3.65;
            }
            else if (category == 3)
            {
                Lanes = 3;
                LaneWidth = 3.67;
            }
            else if (category == 13)
            {
                Lanes = 1;
                LaneWidth = 3.7;
            }
            else if (category == 14)
            {
                Lanes = 2;
                LaneWidth = 3.65;
            }
            else if (category == 15)
            {
                Lanes = 3;
                LaneWidth = 3.65;
            }
            else if (category == 18)
            {
                Lanes = 2;
                LaneWidth = 3.65;
            }
            else if (category == 19)
            {
                Lanes = 3;
                LaneWidth = 3.65;
            }
            else if (category == 20)
            {
                Lanes = 4;
                LaneWidth = 3.65;
            }
            else
            {
                Console.WriteLine("Unknown category encountered. Default values used. Please add the category data to N_W_Link");
                Lanes = 1;
                LaneWidth = 3.7;
            }
        }
    }




}