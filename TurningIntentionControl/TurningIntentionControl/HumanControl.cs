using System;
using System.Collections.Generic;
using System.Text;

namespace ParamincsSNMPcontrol
{
    class HumanControl
    {
        //control function
        public List<int> LaunchInterface(List<JunctionAgent> Junctions)
        {
            List<int> Temp = new List<int>();
            for (int JcnNum = 0; JcnNum < Junctions.Count; JcnNum++)
            {
                bool InputGood = false;
                int NextStage = 0;
                do
                {
                    Console.Write("\nPlease Enter numnber of next stage for Junction " + (JcnNum + 1) + ":");
                    ConsoleKeyInfo StageKey = Console.ReadKey();
                    int StageAsci = Convert.ToInt32(StageKey.KeyChar);

                    int SelectedStage = StageAsci - 48;

                    if (SelectedStage > 0 && SelectedStage <= Junctions[JcnNum].NoOfStages)
                    {
                        InputGood = true;
                        NextStage = SelectedStage;
                    }
                    else
                    {
                        Console.WriteLine("Error: The value entered is not a valid stage. Please try again");
                    }

                } while (InputGood == false);

                Temp.Add(NextStage);
            }

            return (Temp);
        }
    }
}
