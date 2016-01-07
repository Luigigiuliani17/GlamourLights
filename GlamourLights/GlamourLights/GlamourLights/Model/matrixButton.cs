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

        public matrixButton (int x, int y)
        {

            this.x_cord = x_cord;
            this.y_cord = y_cord;
        }
    }
}
