using System;
using System.Collections.Generic;
using System.Text;

namespace ParamincsSNMPcontrol
{
    public class Bid
    {
        //*class Costructor
        public Bid() { }
    }

    public class ScalarBid : Bid
    {
        //*Class Members
        public double BidValue;

        //*class Constructor
        public ScalarBid() : base() { }
    }
}
