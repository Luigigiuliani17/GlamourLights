using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamourLights.Model;

namespace GlamourLights.Controller
{
    class ShopManager
    {
        /// <summary>
        /// calculates the less costly path from a starting point and a destination using dikstra algorithm 
        /// </summary>
        /// <param name="x1"></param> x cord of starting point
        /// <param name="y1"></param> y cord of starting point
        /// <param name="x2"></param> x cord of destination
        /// <param name="y2"></param> y cord of destination
        /// <param name="color"></param> color of the path
        /// <param name="shop_graph"></param> graph of the shop 
        /// <returns></returns> the path found
        public CarpetPath calculatePath(int x1, int y1, int x2, int y2, CarpetColors color, Dictionary<string, Graphvertex> shop_graph )
        {
            //distance of each node from the starting point
            Dictionary<string, int> distance = new Dictionary<string, int>();
            //predecessor of every node
            Dictionary<string, string> predecessor = new Dictionary<string, string>();
            // Q = set of nodes to be explored
            Dictionary < string, Graphvertex > Q = new Dictionary<string, Graphvertex>();
            //key of the vertex in analysis 
            string u = "";

            ///////////////////////////////////// initialization  ///////////////////////////////////7
            foreach (var v in shop_graph)
            {
                //set initial distance to infinite
                distance.Add(v.Key, 9999);
                predecessor.Add(v.Key, null);
                Q.Add(v.Key, v.Value);
            }

            distance[x1 + ";" + y1] = 0;
            Boolean dest_found = false;
            

            /////////////////////////// main part   /////////////////////////////////////////
            while(Q.Count!=0 && !dest_found)
            {
                //find vertex with min distance
                u = null;
                int min = 999;
                foreach (var x in Q)
                {
                    if (distance[x.Key]<min)
                    {
                        min = distance[x.Key];
                        u = x.Key;
                    }
                }

                //if u is our destination, than break;
                if(u == x2+";"+y2)
                {
                    dest_found = true;
                    break;
                }

                Q.Remove(u);

                if(distance[u]==9999)
                {
                    throw new Exception("la destinazione non è raggiungibile!!!");
                    break; 
                }

                foreach(var n in shop_graph[u].adjacent_nodes)
                {
                    int new_dist = distance[u] + n.Value.cost;
                    if (new_dist < distance[n.Key])
                    {
                        distance[n.Key] = new_dist;
                        predecessor[n.Key] = u;
                    }
                }
            }

            /////////////////////path reconstruction////////////////////////////////
            int final_cost = distance[u];
            List<int> x_cord = new List<int>();
            List<int> y_cord = new List<int>();

            while(predecessor[u]!= null)
            {
                x_cord.Insert(0, shop_graph[u].x_cord);
                y_cord.Insert(0, shop_graph[u].y_cord);
                u = predecessor[u];
            }
            x_cord.Insert(0, shop_graph[u].x_cord);
            y_cord.Insert(0, shop_graph[u].y_cord);

            CarpetPath final_result = new CarpetPath(x_cord.ToArray(), y_cord.ToArray(), color, 1 ,final_cost);

            return final_result;    
        }
    }
}
