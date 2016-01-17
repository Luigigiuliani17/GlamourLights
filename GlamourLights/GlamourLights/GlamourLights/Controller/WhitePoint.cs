using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Controller
{
    class WhitePoint
    {
        public string coord { get; set; }
        public int how_many { get; set; }
        public WhitePoint(string coord)
        {
            this.coord = coord;
            this.how_many = 0;
        }
    }
}
