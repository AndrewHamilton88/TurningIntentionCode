using System;
using System.Collections.Generic;
using System.Text;
using ParamicsSNMP2007;

namespace ParamincsSNMPcontrol
{
    public class SnmpConnect
    {
        //*Class Members
        public string
            IP,
            Path;
        public int port;
        public PController PCont;
        public Status ConnectionStatus;

        //*Class Constructor
        public SnmpConnect(string A, string B, int C)
        {
            Path = A; IP = B; port = C;

            try
            {
                VBforParamics.VBPconnect VBCon = new VBforParamics.VBPconnect();

                VBCon.ping(IP, port);
                PCont = VBCon.myPController;

                ConnectionStatus = PCont.GetStatus();
                if (ConnectionStatus == null)
                {
                    throw new Exception("Error: Cannot recover connection status, connection may not be established");
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

    }
}
