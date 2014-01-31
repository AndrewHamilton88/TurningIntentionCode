using System;
using System.Collections.Generic;
using System.Text;
using Mapack;

namespace ParamicsSNMPcontrol
{
    public class FFTTrunManager
    {
        Matrix Template;
        public Matrix Demands;
        int Row;
        int Column;
        public FFTTrunManager(double[][] MatIn)
        {
            Template = new Matrix(MatIn);
            Demands = new Matrix(Template.Rows, Template.Columns,0);

            Row = 0; Column = 0;
            pushOn();

        }

        public bool pushOn()
        {
            Demands = new Matrix(Template.Rows, Template.Columns, 0);

            if (Column < Template.Columns)
            {
                ZeroTest(Template[Row, Column]);
                return(true);
            }
            else if (Row < (Template.Rows -1))
            {
                Column = 0;
                Row++;
                ZeroTest(Template[Row, Column]);
                return (true);
            }
            else
            {
                return (false);
            }

        }

        private void ZeroTest(double tester)
        {
            if (tester == 0)
            {
                Column++;
                pushOn();
            }
            else
            {
                Demands[Row, Column] = tester;
                Column++;
            }
        }
    }
}
