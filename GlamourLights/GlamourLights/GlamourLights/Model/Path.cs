using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    class Path
    {
        public Colors color { get; set; }
        public int[] x_cordinates { get; set; }
        public int[] y_cordinates { get; set; }
        public int path_number { get; set; }

        /// <summary>
        /// function that creates a new path 
        /// </summary>
        /// <param name="x_cordinates"></param> array of x_cordinates of the path
        /// <param name="y_cordinates"></param> array of y_cordinates of the path
        /// <param name="color"></param> color of the path
        public Path(int[] x_cordinates, int[] y_cordinates, Colors color, int path_number)
        {
            this.x_cordinates = x_cordinates;
            this.y_cordinates = y_cordinates;
            this.color = color;
            this.path_number = path_number;
        }
    }
}
