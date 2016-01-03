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
        public bool[] active_colors { get; set; }
        public int n_activeColors { get; set; }
        public Dictionary<string, Graphvertex> adjacent_nodes { get; set; }

        public Graphvertex(int cost, int x, int y)
        {
            this.cost = cost;
            this.x_cord = x;
            this.y_cord = y;
            n_activeColors = 0;
            active_colors = new bool[4];
            for (int i = 0; i < active_colors.Length; i++)
                active_colors[i] = false;
            adjacent_nodes = new Dictionary<string, Graphvertex>();
        }
    }
}
