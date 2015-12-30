using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    public class Graphvertex
    {
        public int cost { get; set; }
        public int x_cord { get; set; }
        public int y_cord { get; set; }
        public Dictionary<string, Graphvertex> adjacent_nodes { get; set; }

        public Graphvertex(int cost, int x, int y)
        {
            this.cost = cost;
            this.x_cord = x;
            this.y_cord = y;
            
            adjacent_nodes = new Dictionary<string, Graphvertex>();
        }
    }
}
