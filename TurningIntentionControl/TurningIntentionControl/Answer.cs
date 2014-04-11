using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class Answer
    {
        public double TotalDelay = 0;
        public List<int[]> Cycleplan = new List<int[]>();
        public double TotalDelayPerSecond = 0;
    }
}
