using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_LW1
{
    class Server
    {
        public int objL1 = 0;
        public int objL2 = 0;

        public bool Available { get; set; }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public double SleepTime { get; set; }
        public Server()
        {
            Available = true;
        }
    }
}
