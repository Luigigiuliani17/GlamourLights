using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Controller
{
    class BlinkPoint
    {
        public string coord { get; set; }
        public bool isValid { get; set; }
        public BlinkPoint(string coord)
        {
            this.coord = coord;
            this.isValid = true;
        }
    }
}
