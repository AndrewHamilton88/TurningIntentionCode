using System;
using System.Collections.Generic;
using System.Text;

namespace ParamicsPuppetMaster
{
    public class VehiclePosition
    {
        //*class members
        public string
            ViD,
            NextTurn,
            NextNextTurn;
        public double
            LinkDist,
            Vspeed,
            X,
            Y,
            Z;
        public int
            InStage,
            InScenario,
            NextLink,
            NextNextLink;
        public LinkID
            OnLink;
        public DateTime
            AtTime;
        public bool
            Obsolete;
        //*Constructor
        public VehiclePosition() 
        {
            //DEFAULT VALUES
            ViD = "NoID";
            LinkDist = 511;
            Vspeed = 511;
            X = 511;
            Y = 511;
            Z = 511;
            NextLink = 511;
            NextNextLink = 511;
            NextTurn = "NULL";
            NextNextTurn = "NULL";
            OnLink = new LinkID();
            AtTime = new DateTime(1979, 2, 14, 0, 0, 0);
            Obsolete = false;
        }

        //*Funtions
        //function for inseting into database row
        public string[] MakeDBLine()
        {
            string[] TheLine = new string[17];
            TheLine[0] = ("'" + ViD + "'");
            TheLine[1] = ("'" + AtTime.TimeOfDay.ToString() + "'");
            TheLine[2] = ("'" + OnLink.StartNode.ToString() + ":" + OnLink.EndNode.ToString() + "'");
            TheLine[3] = LinkDist.ToString();
            TheLine[4] = Vspeed.ToString();
            TheLine[5] = X.ToString();
            TheLine[6] = Y.ToString();
            TheLine[7] = Z.ToString();
            TheLine[8] = InStage.ToString();
            TheLine[9] = InScenario.ToString();
            int ObsoleteINT = Convert.ToInt32(Obsolete);
            TheLine[10] = ("'" + ObsoleteINT.ToString() + "'");
            TheLine[11] = "NULL";
            TheLine[12] = "NULL";
            TheLine[13] = NextLink.ToString();
            TheLine[14] = NextNextLink.ToString();
            TheLine[15] = "NULL5";
            TheLine[16] = "NULL5";

            return (TheLine);

        }


    }
    public class VehicleIdentity
    {
        //*class members
        public string
            ViD,
            Routename,
            MagicNumbers,
            NextTurn,
            NextNextTurn;
        public int
            Tag,
            Vtype,
            Origin,
            Destination,
            BornStage,
            NextLink,
            NextNextLink,
            BornScenario;
        public LinkID BornLink;
        public DateTime BornAt;
        public bool Obsolete;

        //*Consructor
        public VehicleIdentity()
        {
            ViD = "NoID";
            Routename = "NULL";
            Tag = 511;
            Vtype = 511;
            Origin = 511;
            Destination = 511;
            NextLink = 511;
            NextNextLink = 511;
            NextTurn = "NULL";
            NextNextTurn = "NULL";
            BornLink = new LinkID();
            BornAt = new DateTime(1979, 2, 14, 0, 0, 0);
            Obsolete = false;
            MagicNumbers = "[511,511]";
        }

        //*Funtions
        //function for inseting into database row
        public string[] MakeDBLine()
        {
            string[] TheLine = new string[18];
            TheLine[0] = ("'" + ViD + "'");
            TheLine[1] = Vtype.ToString();
            TheLine[2] = ("'" + BornAt.TimeOfDay.ToString() + "'");
            TheLine[3] = ("'" + BornLink.StartNode.ToString() + ":" + BornLink.EndNode.ToString() + "'");
            if (Routename.Equals("NULL"))
            {
                TheLine[4] = "NULL";
            }
            else
            {
                TheLine[4] = ("'" + Routename + "'");
            }
            if (Origin != 511)
            {
                TheLine[5] = Origin.ToString();
            }
            else
            {
                TheLine[5] = "NULL";
            }
            if (Destination != 511)
            {
                TheLine[6] = Destination.ToString();
            }
            else
            {
                TheLine[6] = "NULL";
            }

            TheLine[7] = Tag.ToString();
            TheLine[8] = BornStage.ToString();
            TheLine[9] = BornScenario.ToString();
            int ObsoleteINT = Convert.ToInt32(Obsolete);
            TheLine[10] = ("'" + ObsoleteINT.ToString() + "'");
            if (MagicNumbers.Equals("[511,511]"))
            {
                TheLine[11] = "NULL";
            }
            else
            {
                TheLine[11] = ("'" + MagicNumbers + "'");
            }
            TheLine[12] = "NULL";
            TheLine[13] = "NULL";

            if (NextLink != 511)
            {
                TheLine[14] = NextLink.ToString();
            }
            else
            {
                TheLine[14] = "NULL";
            }
            if (NextNextLink != 511)
            {
                TheLine[15] = NextNextLink.ToString();
            }
            else
            {
                TheLine[15] = "NULL";
            }
            TheLine[16] = "NULL6"; //NextTurn
            TheLine[17] = "NULL6"; //NextNextTurn

            return (TheLine);

        }

    }

    public class VehicleDataLite
    {
        //*class members
        public string Source,
            NextTurn,
            NextNextTurn;
        public double
            LinkDist,
            Vspeed;
        public int
            Vtype,
            Lane,
            NextLink,
            NextNextLink;
        public LinkID
            OnLink;
        public DateTime
            AtTime,
            BornTime;
        //*Constructor
        public VehicleDataLite()
        {
            //DEFAULT VALUES
            Source = "default";
            LinkDist = 511;
            Vspeed = 511;
            NextLink = 511;
            NextNextLink = 511;
            NextTurn = "default";
            NextNextTurn = "default";
            Lane = 0;
            OnLink = new LinkID();
            AtTime = new DateTime(1979, 2, 14, 0, 0, 0);
            BornTime = new DateTime(1979, 2, 14, 0, 0, 0);
        }

        //*Funtions
        //function for inseting into database row
        public string[] MakeDBLine()
        {
            string[] TheLine = new string[12];
            TheLine[0] = ("'" + AtTime.TimeOfDay.ToString() + "'");
            TheLine[1] = ("'" + OnLink.StartNode.ToString() + ":" + OnLink.EndNode.ToString() + "'");
            TheLine[2] = LinkDist.ToString();
            TheLine[3] = Vspeed.ToString();
            TheLine[4] = Vtype.ToString();
            TheLine[5] = Lane.ToString();
            TheLine[6] = ("'" + BornTime.TimeOfDay.ToString() + "'");
            TheLine[7] = ("'" + Source + "'");
            TheLine[8] = NextLink.ToString();
            TheLine[9] = NextNextLink.ToString();
            TheLine[10] = ("'" + NextTurn + "'"); ; //NextTurn
            TheLine[11] = ("'" + NextNextTurn + "'"); //NextNextTurn
            

            return (TheLine);

        }


    }
}
