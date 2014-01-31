using System;
using System.Collections.Generic;
using System.Text;
using ParamicsSNMP2007;

namespace ParamincsSNMPcontrol
{
    class DetectorRead
    {
        //*Class Members
        public SnmpConnect Sconnect;
        public List<DetectorInfo> DInfo = new List<DetectorInfo>();

        //*Class Constructor
        public DetectorRead(SnmpConnect A)
        {
            Sconnect = A;

            try
            {

                Sconnect.PCont.ParseDetectors(ref Sconnect.Path);//Double check that detector data are in PCont object

                ParamicsPuppetMaster.EditDetectors ED = new ParamicsPuppetMaster.EditDetectors(Sconnect.Path);// Get detector info from paramics network
                

                foreach (ParamicsPuppetMaster.Detector Dt in ED.DetectorList)// Add a line to the MiB for each detector
                {
                    int IDR = Sconnect.PCont.AddDetectorControlRow(ref Dt.Name);
                    DInfo.Add(new DetectorInfo(Dt, IDR));
                    if (IDR == 0)
                    {
                        throw new Exception("Error: Detector named " + Dt.Name + " was not detected and a control row was not added to the MiB");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //*Function for reading detector info **Prototype stage NOT FINAL FUNCTION STRUCTURE**
        public List<DetectorControl> ReadOutInfo()
        {
            List<DetectorControl> DCList = new List<DetectorControl>();
            try
            {
                foreach (DetectorInfo Dt in DInfo)
                {
                    DetectorControl DC = Sconnect.PCont.GetDetectorControl(ref Dt.RowId);
                    if (DC == null)
                    {
                        throw new Exception("Error: Could not get detector control for detector named " + Dt.Name +".");
                    }
                    DCList.Add(DC);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return (DCList);
        }

        //*Function for reading detector data **Prototype stage NOT FINAL FUNCTION STRUCTURE**
        public List<DetectorData> ReadOutData()
        {
            List<DetectorData> DDList = new List<DetectorData>();
            try
            {
                foreach (DetectorInfo Dt in DInfo)
                {
                    int LaneNum = 1;
                    DetectorData DD = Sconnect.PCont.GetDetectorData(ref Dt.RowId, ref LaneNum);
                    if (DD == null)
                    {
                        throw new Exception("Error: Could not get detector control for detector named " + Dt.Name + ".");
                    }
                    DDList.Add(DD);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return (DDList);
        }

    }
    class DetectorInfo
    {
        //*ClassMembers
        public string Name;
        public int RowId;
        public ParamicsPuppetMaster.Detector Ddata;

        //*ClassConstructor
        public DetectorInfo(ParamicsPuppetMaster.Detector A,int B)
        {
            Name = A.Name;
            RowId = B;
            Ddata = A;
        }
    }
}
