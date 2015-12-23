using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    class CarpetPath
    {
        public CarpetColors color { get; set; }
        public int[] x_cordinates { get; set; }
        public int[] y_cordinates { get; set; }
        public int path_number { get; set; }
        public int cost { get; set; }

        /// <summary>
        /// function that creates a new path 
        /// </summary>
        /// <param name="x_cordinates"></param> array of x_cordinates of the path
        /// <param name="y_cordinates"></param> array of y_cordinates of the path
        /// <param name="color"></param> color of the path
        public CarpetPath(int[] x_cordinates, int[] y_cordinates, CarpetColors color, int path_number, int cost)
        {
            this.x_cordinates = x_cordinates;
            this.y_cordinates = y_cordinates;
            this.color = color;
            this.path_number = path_number;
            this.cost = cost;
        }
    }
}
