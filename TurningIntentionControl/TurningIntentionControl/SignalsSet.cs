using System;
using System.Collections.Generic;
using System.Text;

namespace ParamincsSNMPcontrol
{
    public class SignalsSet
    {
        //*Class Members
        public SnmpConnect Sconnect;
        public List<SignalNode> SigNodes = new List<SignalNode>();

        //*Class Constructor -- Note Currently only set up for stage signal control different coding required for flow control
        public SignalsSet(SnmpConnect A, List<string> NodeNames)//NOTE: rather that passing NodeNames it would be better to extract info from model files if possible
        {
            Sconnect = A;

            try
            {
                Sconnect.PCont.ParseNodes(ref Sconnect.Path);//Double check nodes data are loaded

                foreach (string Nd in NodeNames)
                {
                    string[] Ndlist = Nd.Split(',');
                    List<int> NodeCodes = new List<int>();

                    foreach (string Ndf in Ndlist)
                    {
                        string NdfN = Ndf;
                        int NodeID = Sconnect.PCont.AddSignalNodeControlRow(ref NdfN);
                        //int FlowID = Sconnect.PCont.AddSignalFlowControlRow(ref Ndf);
                        if (NodeID == 0)
                        {
                            throw new Exception("Signal junction node with name " + Nd + " was not detected or added to the MiB");
                        }
                        NodeCodes.Add(NodeID);
                    }
                    SignalNode temp = new SignalNode(Nd, NodeCodes);
                    SigNodes.Add(temp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //*Function to set signal flows directly (PROTOTYPE)
        public void SetFlows()
        {
            SignalNode SN = SigNodes[0];
            ParamicsSNMP2007.SignalFlowControl sfcSFC;
            ParamicsSNMP2007.SignalFlowPriorityCol colSFP;
            ParamicsSNMP2007.SignalFlowPriority sfpSFP;
            double
                LinMove,
                LoutMove;
            //sfcSFC = Sconnect.PCont.GetSignalFlowControl(ref SN.MiBFlowRow);
            //colSFP = Sconnect.PCont.GetSignalFlowPriorities(ref SN.MiBFlowRow);


        }

        //*Function to set signal stages (PROTOTYPE)
        public void SetStages(int Stage, int Junction)
        {
            SignalNode SD = SigNodes[Junction];

            foreach (int Ncode in SD.MiBNodeRow)
            {
                int NcodeN = Ncode;
                ParamicsSNMP2007.SignalNodeControl JuncControl = Sconnect.PCont.GetSignalNodeControl(ref NcodeN);
                if(Stage != JuncControl.CurrentStage)
                {
                    Sconnect.PCont.SetSignalNodeControlGotoNext(ref NcodeN, ref Stage);
                }
            }
        }
    }

    public class SignalNode
    {
        //*Class members
        public string NodeName;
        public List<int> MiBNodeRow;


        //*Class constructor
        public SignalNode(string N, List<int> MN)
        {
            NodeName = N;
            MiBNodeRow = MN;
        }
    }
}
