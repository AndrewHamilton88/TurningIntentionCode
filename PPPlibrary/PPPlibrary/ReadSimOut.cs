using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ParamicsPuppetMaster
{
    public class ReadSimOut
    {
        //*Class Members
        string NetworkName;
        int RunNumber;
        public string LogDir;

        //*Constructor
        public ReadSimOut(string a, int b)
        {
            NetworkName = a;
            RunNumber = b;
            string RunNumberS = RunNumber.ToString("000");
            LogDir = (NetworkName + @"\Log\run-" + RunNumberS);
        }
        public ReadSimOut(string a)
        {
            NetworkName = a;
            DirectoryInfo Log = new DirectoryInfo((NetworkName + "\\Log\\"));
            DirectoryInfo[] LogList = Log.GetDirectories();
            int L = LogList.Length;
            LogDir = LogList[L - 1].FullName;
        }


    }

    public class ReadEventsFile : ReadSimOut
    {
        //*Class Members
        public List<FileData> Fdata = new List<FileData>();

        //*Constructors
        public ReadEventsFile(string NetworkName, int RunNumber)
            : base(NetworkName,RunNumber)
        {
            ConstructorFunction();
        }

        public ReadEventsFile(string NetworkName)
            : base(NetworkName)
        {
            ConstructorFunction();
        }

        //constructor function
        private void ConstructorFunction()
        {
            using (StreamReader ReadFile = new StreamReader((LogDir + @"\events.csv")))
            {
                try
                {
                    string FileLine;
                    while ((FileLine = ReadFile.ReadLine()) != null)
                    {
                        if (!FileLine.Contains("Day"))
                        {
                            string[] splitstring = FileLine.Split(new char[] { ',' });
                            if ((Convert.ToInt32(splitstring[3])) == 23)
                            {
                                FileData TempDat = new FileData();
                                TempDat.Tag = Convert.ToInt32(splitstring[9]);
                                TempDat.Vtype = Convert.ToInt32(splitstring[4]);
                                TempDat.Lane = Convert.ToInt32(splitstring[8]);
                                TempDat.Vspeed = (Convert.ToDouble(splitstring[5])) * 0.44704;//convert speed to m/s
                                TempDat.LinkDist = Convert.ToDouble(splitstring[6]);

                                string[] splitsplitstring = splitstring[2].Split(new char[] { '"', ':' });
                                string LinkStart = splitsplitstring[1];
                                string LinkEnd = splitsplitstring[2];
                                TempDat.OnLink = new LinkID(LinkStart, LinkEnd);
                                TempDat.AtTime = DateTime.Parse(splitstring[1]);//change this to include date if relevant

                                Fdata.Add(TempDat);

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The events file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

        }

        public class FileData
        {
            //*Class members
            public int
            Tag,
            Vtype,
            Lane,
            NextLink,
            NextNextLink;
            public string
                NextTurn,
                NextNextTurn;
            public double
                Vspeed,
                LinkDist;
            public LinkID
                OnLink;
            public DateTime
                AtTime;

            //*constructors
            public FileData() 
            {
                //DEFAULT VALUES
                Tag = 511;//This may cause a problem because 511 can be an actual tag check it out
                Vtype = 511;
                Lane = 511;
                NextLink = 511;
                NextNextLink = 511;
                NextTurn = "NULL";
                NextNextTurn = "NULL";
                Vspeed = 511;
                LinkDist = 511;
                OnLink = new LinkID();
                AtTime = new DateTime(1979, 2, 14, 0, 0, 0);
            }


        }

    }

    

    public class ReadVroutesFile : ReadSimOut
    {
        //*Class Members
        public List<FileData> Fdata = new List<FileData>();

        //*Constructors
        public ReadVroutesFile(string NetworkName, int RunNumber)
            : base(NetworkName, RunNumber)
        {
            ConstructorFunction(); 
        }

        public ReadVroutesFile(string NetworkName)
            : base(NetworkName)
        {
            ConstructorFunction();
        }
        //costructor function
        private void ConstructorFunction()
        {
            using (StreamReader ReadFile = new StreamReader((LogDir + @"\vehicleroutes.csv")))
            {
                try
                {
                    string FileLine;
                    while ((FileLine = ReadFile.ReadLine()) != null)
                    {
                        if (!FileLine.Contains("Vehicle"))
                        {
                            string[] splitstring = FileLine.Split(new char[] { ',' });

                            FileData TempDat = new FileData();

                            TempDat.Tag = Convert.ToInt32(splitstring[0]);
                            TempDat.Vtype = Convert.ToInt32(splitstring[3]);
                            TempDat.Origin = Convert.ToInt32(splitstring[1]);
                            TempDat.Destination = Convert.ToInt32(splitstring[2]);
                            TempDat.NextLink = 512; //have made it 512 as opposed to 511 to help notice a different problem if it arises
                            TempDat.NextNextLink = 512;
                            TempDat.NextTurn = "NULL2";
                            TempDat.NextNextTurn = "NULL2";


                            string[] splitsplitstring = splitstring[7].Split(new char[] { '"', ':' });
                            string LinkStart = splitsplitstring[1];
                            string LinkEnd = splitsplitstring[2];
                            TempDat.EnterLink = new LinkID(LinkStart, LinkEnd);
                            TempDat.EnterTime = DateTime.Parse(splitstring[8]);//change this to include date if relevant

                            Fdata.Add(TempDat);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The events file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }
        public class FileData
        {
            //*class members
            public int
                Tag,
                Vtype,
                Origin,
                Destination,
                NextLink,
                NextNextLink;
            public string
                NextTurn,
                NextNextTurn;
            public LinkID
                EnterLink;
            public DateTime
                EnterTime;
            //*class constructor
            public FileData() 
            {
                //DEFAULT VALUES
                Tag = 511;//This may cause a problem because 511 can be an actual tag check it out
                Vtype = 511;
                Origin = 511;
                Destination = 511;
                NextLink = 511;
                NextNextLink = 511;
                NextTurn = "NULL";
                NextNextTurn = "NULL";
                EnterLink = new LinkID();
                EnterTime = new DateTime(1979, 2, 14, 0, 0, 0);
            }

        }

    }

    public class ReadSnapshotFile : ReadSimOut
    {
        //*Class members
        public List<FileData> Fdata = new List<FileData>();
            
        //*Constructor
        public ReadSnapshotFile(string NetworkName, int RunNumber)
            : base(NetworkName, RunNumber)
        {
            ConstructorFunction();
        }
        public ReadSnapshotFile(string NetworkName)
            : base(NetworkName)
        {
            ConstructorFunction();
        }
        //constructor function
        private void ConstructorFunction()
        {
            try
            {
                DirectoryInfo LogDirI = new DirectoryInfo(LogDir);
                FileInfo[] snaplist = LogDirI.GetFiles("snap*");
                DateTime TimeNow = new DateTime();
                DateTime ZeroHour = DateTime.Parse("00:00:00");
                LinkID LinkNow = new LinkID();
                foreach (FileInfo snapfile in snaplist)
                {
                    using (StreamReader ReadFile = new StreamReader(snapfile.FullName))
                    {
                        string FileLine;
                        while ((FileLine = ReadFile.ReadLine()) != null)
                        {
                            if (FileLine.Contains("snapshot at time"))
                            {
                                string[] splitline = FileLine.Split(new char[] { ' ' });
                                double TimeSecs = Convert.ToDouble(splitline[3]);
                                TimeNow = ZeroHour.AddSeconds(TimeSecs);
                            }
                            else if (FileLine.Contains("on link"))
                            {
                                string[] splitline = FileLine.Split(new char[] { ' ', ':' });
                                LinkNow = new LinkID(splitline[2], splitline[3]);
                            }
                            else if (FileLine.Contains("type "))
                            {
                                FileData TempDat = new FileData();

                                if (FileLine.Contains("path"))
                                {
                                    string[] splitline = FileLine.Split(new char[] { ' ', '"' });
                                    TempDat.Vtype = Convert.ToInt32(splitline[1]) + 1;
                                    TempDat.Origin = Convert.ToInt32(splitline[15]) + 1;
                                    TempDat.Destination = Convert.ToInt32(splitline[16]) + 1;
                                    TempDat.Vspeed = Convert.ToDouble(splitline[13]);
                                    TempDat.LinkDist = Convert.ToDouble(splitline[8]);
                                    TempDat.RouteName = splitline[6];
                                    TempDat.OnLink = LinkNow;
                                    TempDat.AtTime = TimeNow;
                                    double bornsecs = Convert.ToDouble(splitline[17]);
                                    TempDat.BornTime = ZeroHour.AddSeconds(bornsecs);
                                    TempDat.MagicNumbers = ("[" + splitline[10]);
                                    TempDat.MagicNumbers += ("," + splitline[20] + "]");//This needs to be checked
                                    

                                    Fdata.Add(TempDat);
                                }
                                else
                                {
                                    string[] splitline = FileLine.Split(new char[] { ' ' });
                                    TempDat.Vtype = (Convert.ToInt32(splitline[1])) + 1;
                                    TempDat.Origin = (Convert.ToInt32(splitline[10])) + 1;
                                    TempDat.Destination = (Convert.ToInt32(splitline[11])) + 1;
                                    TempDat.Vspeed = Convert.ToDouble(splitline[8]);
                                    TempDat.LinkDist = Convert.ToDouble(splitline[3]);
                                    TempDat.NextLink = Convert.ToInt32(splitline[13]);
                                    TempDat.NextNextLink = Convert.ToInt32(splitline[14]);
                                    TempDat.NextTurn = "NULL3";
                                    TempDat.NextNextTurn = "NULL3";

                                    TempDat.OnLink = LinkNow;
                                    TempDat.AtTime = TimeNow;
                                    double bornsecs = Convert.ToDouble(splitline[12]);
                                    TempDat.BornTime = ZeroHour.AddSeconds(bornsecs);
                                    TempDat.MagicNumbers = ("[" + splitline[5]);
                                    TempDat.MagicNumbers += ("," + splitline[15] + "]");
                                    

                                    Fdata.Add(TempDat);

                                }

                            }


                        }

                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Error reading snapfiles:");
                Console.WriteLine(e.Message);

            }
        }
        public class FileData
        {
            //*class members
            public int
            Vtype,
            Origin,
            Destination,
            NextLink,
            NextNextLink;
            public double
                Vspeed,
                LinkDist;
            public string
                RouteName,
                MagicNumbers,
                NextTurn,
                NextNextTurn;
            public LinkID
                OnLink;
            public DateTime
                AtTime,
                BornTime;

            //*class constructor
            public FileData() 
            {
                //DEFAULT VALUES
                Vtype = 511;
                Origin = 511;
                Destination = 511;
                NextLink = 511;
                NextNextLink = 511;
                NextTurn = "NULL";
                NextNextTurn = "NULL";
                Vspeed = 511;
                LinkDist = 511;
                RouteName = "NULL";
                OnLink = new LinkID();
                AtTime = new DateTime(1979, 2, 14, 0, 0, 0);
                BornTime = new DateTime(1979, 2, 14, 0, 0, 0);
                MagicNumbers = "[511,511]";
            }

        }
    }
    public class ReadSnapshotFileLite : ReadSimOut
    {
        //*Class members
        public List<FileData> Fdata = new List<FileData>();
        //string ApproachNode;
        //string JunctionNode;

        //*Constructor
        public ReadSnapshotFileLite(string NetworkName, int RunNumber)
            : base(NetworkName, RunNumber)
        {
            ConstructorFunction();
        }
        public ReadSnapshotFileLite(string NetworkName)
            : base(NetworkName)
        {
            ConstructorFunction();
        }
        //constructor function
        private void ConstructorFunction()
        {
            try
            {
                DirectoryInfo LogDirI = new DirectoryInfo(LogDir);
                FileInfo[] snaplist = LogDirI.GetFiles("snap*");
                DateTime TimeNow = new DateTime();
                DateTime ZeroHour = DateTime.Parse("00:00:00");
                LinkID LinkNow = new LinkID();
                ReadTurningIntention RTI = new ReadTurningIntention();
                string TempVariableNextLink = "";
                string TempVariableNextNextLink = "";
                string TempVariableNextTurn = "";
                string TempVariableNextNextTurn = ""; 

                
                FileInfo snapfile = snaplist[snaplist.Length- 1];
                    using (StreamReader ReadFile = new StreamReader(snapfile.FullName))
                    {
                        string FileLine;
                        while ((FileLine = ReadFile.ReadLine()) != null)
                        {
                            if (FileLine.Contains("snapshot at time"))
                            {
                                string[] splitline = FileLine.Split(new char[] { ' ' });
                                double TimeSecs = Convert.ToDouble(splitline[3]);
                                TimeNow = ZeroHour.AddSeconds(TimeSecs);
                            }
                            else if (FileLine.Contains("on link"))
                            {
                                string[] splitline = FileLine.Split(new char[] { ' ', ':' });
                                LinkNow = new LinkID(splitline[2], splitline[3]);
                                //ApproachNode = splitline[2];
                                //JunctionNode = splitline[3];
                            }
                            else if (FileLine.Contains("type "))
                            {
                                FileData TempDat = new FileData();

                                if (FileLine.Contains("path"))
                                {
                                    string[] splitline = FileLine.Split(new char[] { ' ', '"' });
                                    TempDat.Vtype = Convert.ToInt32(splitline[1]) + 1;
                                    TempDat.Origin = Convert.ToInt32(splitline[15]) + 1;
                                    TempDat.Destination = Convert.ToInt32(splitline[16]) + 1;
                                    TempDat.Vspeed = Convert.ToDouble(splitline[13]);
                                    TempDat.LinkDist = Convert.ToDouble(splitline[8]);
                                    TempDat.RouteName = splitline[6];
                                    TempDat.OnLink = LinkNow;
                                    TempDat.AtTime = TimeNow;
                                    double bornsecs = Convert.ToDouble(splitline[17]);
                                    TempDat.BornTime = ZeroHour.AddSeconds(bornsecs);
                                    TempDat.MagicNumbers = ("[" + splitline[10]);
                                    TempDat.MagicNumbers += ("," + splitline[20] + "]");//This needs to be checked

                                    Fdata.Add(TempDat);
                                }
                                else
                                {
                                    string[] splitline = FileLine.Split(new char[] { ' ' });
                                    TempDat.Vtype = (Convert.ToInt32(splitline[1])) + 1;
                                    TempDat.Origin = (Convert.ToInt32(splitline[10])) + 1;
                                    TempDat.Destination = (Convert.ToInt32(splitline[11])) + 1;
                                    TempDat.Lane = (Convert.ToInt32(splitline[7]));

                                    TempDat.Vspeed = Convert.ToDouble(splitline[8]);
                                    TempDat.LinkDist = Convert.ToDouble(splitline[3]);
                                    TempDat.OnLink = LinkNow;
                                    TempDat.AtTime = TimeNow;
                                    double bornsecs = Convert.ToDouble(splitline[12]);
                                    TempDat.BornTime = ZeroHour.AddSeconds(bornsecs);
                                    TempDat.MagicNumbers = ("[" + splitline[5]);
                                    TempDat.MagicNumbers += ("," + splitline[15] + "]");
                                    TempVariableNextLink = RTI.NextLinkNumber(LinkNow.StartNode,LinkNow.EndNode,splitline[13]);
                                    if (TempVariableNextLink == "null")
                                    {
                                        TempDat.NextLink = 99999;           //TODO 99999 could be a next link number!
                                        TempDat.NextNextLink = 99999;
                                    }
                                    else
                                    {
                                        TempDat.NextLink = Convert.ToInt32(TempVariableNextLink);
                                    }
                                    TempVariableNextNextLink = RTI.NextLinkNumber(LinkNow.EndNode,TempVariableNextLink,splitline[14]);
                                    if (TempVariableNextNextLink == "null")
                                    {
                                        TempDat.NextNextLink = 99999;
                                    }
                                    else
                                    {
                                        TempDat.NextNextLink = Convert.ToInt32(TempVariableNextNextLink);
                                    }
                                    TempVariableNextTurn = RTI.NextTurnDirection(LinkNow.StartNode, LinkNow.EndNode, splitline[13]);
                                    if (TempVariableNextTurn == "null")
                                    {
                                        TempDat.NextTurn = "None";           //99999 shows an error
                                        TempDat.NextNextTurn = "None";
                                    }
                                    else
                                    {
                                        TempDat.NextTurn = TempVariableNextTurn;
                                    }
                                    TempVariableNextNextTurn = RTI.NextTurnDirection(LinkNow.EndNode, TempVariableNextLink, splitline[14]);
                                    if (TempVariableNextNextTurn == "null")
                                    {
                                        TempDat.NextNextTurn = "None";      //99999 shows an error
                                    }
                                    else
                                    {
                                        TempDat.NextNextTurn = TempVariableNextNextTurn;
                                    }
                                    //TempDat.NextTurn = "Null10";
                                    //TempDat.NextNextTurn = "Null10";
                                    Fdata.Add(TempDat);

                                }

                            }


                        }

                    }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Error reading snapfiles:");
                Console.WriteLine(e.Message);

            }
        }
        public class FileData
        {
            //*class members
            public int
            Vtype,
            Origin,
            Destination,
            Lane,
            NextLink,
            NextNextLink;
            public double
                Vspeed,
                LinkDist;
            public string
                RouteName,
                MagicNumbers,
                NextTurn,
                NextNextTurn;
            public LinkID
                OnLink;
            public DateTime
                AtTime,
                BornTime;

            //*class constructor
            public FileData()
            {
                //DEFAULT VALUES
                Vtype = 511;
                Origin = 511;
                Destination = 511;
                NextLink = 511;
                NextNextLink = 511;
                NextTurn = "NULL";
                NextNextTurn = "NULL";
                Lane = 0;
                Vspeed = 511;
                LinkDist = 511;
                RouteName = "NULL";
                OnLink = new LinkID();
                AtTime = new DateTime(1979, 2, 14, 0, 0, 0);
                BornTime = new DateTime(1979, 2, 14, 0, 0, 0);
                MagicNumbers = "[511,511]";
            }

        }
    }

    public class ReadStatsGeneral : ReadSimOut
    {
        // class member
        public double AverageDelay;
        public int DelayCount;

        //class constructor
        public ReadStatsGeneral(string NetworkName)
            : base(NetworkName)
        {
            AverageDelay = 0;
            DelayCount = 0;
            using (StreamReader ReadFile = new StreamReader((LogDir + @"\stats-general.csv")))
            {
                try
                {
                    string FileLine;
                    string data = "";
                    while ((FileLine = ReadFile.ReadLine()) != null)
                    {
                        if (!FileLine.Contains("Day"))
                        {
                            data = FileLine;
                            DelayCount++;
                        }
                    }
                    string[] SplitLine = data.Split(',');
                    AverageDelay = Convert.ToDouble(SplitLine[5]);
                    
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The events file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
            
        }

    }

    public class ReadPointLoopFiles : ReadSimOut
    {
        //*class members
        public List<LoopRecord> LoopSensors = new List<LoopRecord>();

        //class constructor
        public ReadPointLoopFiles(string NetworkName, int ModelTimeofDay, int dT)
            : base(NetworkName)
        {
            string[] Paths = Directory.GetFiles(LogDir, "point-*.csv");
            TimeSpan TS = new TimeSpan(0, 0, ModelTimeofDay / 100);
            string TimeOfDay = TS.ToString();

            foreach(string p in Paths)
            {
                FileInfo f = new FileInfo(p);
                DateTime StartTime = new DateTime();
                DateTime EndTime = new DateTime();

                StartTime = StartTime.AddSeconds(ModelTimeofDay / 100 - dT);
                EndTime = EndTime.AddSeconds(ModelTimeofDay / 100);

                LoopRecord LoopData = new LoopRecord(StartTime,EndTime,f.Name);

                using (StreamReader ReadFile = new StreamReader(File.Open(p,FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    try
                    {
                        string FileLine;
                        while ((FileLine = ReadFile.ReadLine()) != null)
                        {
                            if(!FileLine.Contains("Day"))
                            {
                                string[] Fields = FileLine.Split(',');
                                DateTime Detection = new DateTime();
                                DateTime Holder = Convert.ToDateTime(Fields[1]);
                                Detection = Detection.Add(Holder.TimeOfDay);

                                int Comparitor1 = Detection.CompareTo(StartTime);
                                int Comparitor2 = Detection.CompareTo(EndTime);

                                if(Comparitor1 ==1)
                                {
                                    if(Comparitor2 <=0)
                                    {
                                        LoopData.AddData(Detection,Convert.ToDouble(Fields[8]));
                                    }
                                }
                            }
                        }
           

                    }
                    catch (Exception e)
                    {
                        // Let the user know what went wrong.
                        Console.WriteLine("A point loop file could not be read:");
                        Console.WriteLine(e.Message);
                    }
                }
                LoopSensors.Add(LoopData);
            

            }
        }

    }

    public class LoopRecord
    {
        //*class members
        public string Name;
        public DateTime StartRecord;
        public DateTime EndRecord;
        public List<DateTime> Stamps;
        public List<double> Speeds;
        public int Count;

        //*constructor
        public LoopRecord(DateTime ST, DateTime ET, string n)
        {
            StartRecord = ST; EndRecord = ET; Name = n;
            Stamps = new List<DateTime>();
            Speeds = new List<double>();
            Count = 0;
        }

        public void AddData(DateTime time, double v)
        {
            Stamps.Add(time);
            Speeds.Add(v);
            Count = Stamps.Count;
        }

        public double AverageSpeed()
        {
            double AvSp = 0;
            foreach (double sp in Speeds)
            {
                AvSp += sp;
            }
            if (Speeds.Count != 0)
            {
                return (AvSp / Speeds.Count);
            }
            else
            {
                return (0);
            }


        }

    }
}
