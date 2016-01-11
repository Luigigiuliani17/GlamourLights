using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GlamourLights.Model
{
    public class matrixButton : Button
    {
        public int x_cord { get; set; }
        public int y_cord { get; set; }

        //kind = -1 if wall, 0 if shelf, 1 if free, 2 if start
        public int kind { get; set; }

        public matrixButton (int x, int y, int kind) 
        {
            this.x_cord = x;
            this.y_cord = y;
            this.kind = kind;
        }
    }
}
