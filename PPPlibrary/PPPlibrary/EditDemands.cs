using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Mapack;

namespace ParamicsPuppetMaster
{
    public class EditDemands : EditNetwork
    {
        Matrix Demands;
        string FileName;

        //*Class Constructors
        public EditDemands(String NetPath, Matrix MatIn)
            : base(NetPath)
        {
            FileName = "demands";
            Demands = MatIn;

            WriteDemandsFile();
           
        }

        private void WriteDemandsFile()
        {
            using (StreamWriter WriteFile = new StreamWriter((NetPath + "\\" + FileName))) // create a new text file to write to.
            {
                try
                {
                    WriteFile.WriteLine("demand period 1");
                    WriteFile.WriteLine("matrix count 1");
                    WriteFile.WriteLine("divisor 1.0000");
                    WriteFile.WriteLine('\n');
                    
                    WriteFile.WriteLine("matrix 1");
                    //TODO this means that consecutive numbering of zones is essential
                    
                    for(int i=0; i<Demands.Rows; i++)
                    {
                        WriteFile.Write("from " + (i+1).ToString());

                        for (int j = 0; j < Demands.Columns; j++) 
                        {
                            WriteFile.Write(" " + Demands[i,j].ToString());
                        }
                        WriteFile.Write('\n');

                    }


                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("There was a problem writing the demands file");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
