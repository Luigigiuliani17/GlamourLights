using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamourLights.Model;
using System.Data.Entity;

namespace GlamourLights.Controller
{
    public class ShopManager
    {
        public ShopState shopState { get; set; }
        public  const double MAX_DEVIATION_FACTOR = 1.8;

        public ShopManager(ShopState shop)
        {
            this.shopState = shop;
        }

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
        public CarpetPath calculateSubPath(int x1, int y1, int x2, int y2, CarpetColors color, Dictionary<string, Graphvertex> shop_graph )
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

            CarpetPath final_result = new CarpetPath(x_cord.ToArray(), y_cord.ToArray(), color, final_cost);

            return final_result;    
        }

        /// <summary>
        /// calculate the path between a starting point, a destination and up to 2 recommendation. (using calculateSubPath() multiple times)
        /// -If the path crossing both recommendation is too long, it will try to calculate the path with only 1 recommendation. 
        /// -if with one recommendation is still too long, it will calculate the path without recommendations
        /// </summary>
        /// <param name="recommendations"></param> list of items recommended (can be of any lenght, max 2 will be used in the path)
        /// <param name="x1"></param> starting x
        /// <param name="y1"></param> starting y
        /// <param name="x2"></param> ending x
        /// <param name="y2"></param> ending y
        /// <param name="color"></param> path color
        /// <param name="shop_graph"></param> graph of the shop
        /// <param name="shelves_position"></param> dictionary of positions of shelves (x, y saved as a string of kind x;y )
        /// <returns></returns> a new path passing also from the recommendations (if it is possible)
        public CarpetPath calculateCompletePath(item[] recommendations, int x1, int y1, int x2, int y2, CarpetColors color, Dictionary<string, Graphvertex> shop_graph, Dictionary<int, string> shelves_position)
        {
          
            //support variables
            int noRecCost = 0, recCost = 0;
            CarpetPath noRecPath, recPath;
            
            //calculatepath cost of the path without recommendations
            noRecPath = calculateSubPath(x1, y1, x2, y2, color, shop_graph);
            noRecCost = noRecPath.cost;

            //calculate path and path cost with 2 recommendations:
            //starting from the 2 first recomendations, try to find a combination of 2 of them that has a path cost acceptable
            foreach(item i in recommendations)
            {
                foreach(item j in recommendations)
                {
                    if (i.Equals(j))
                        continue;

                    //extract coordinates of first recommendation
                    String[] parameters;
                    int xrec1, yrec1, xrec2, yrec2;
                    parameters = shelves_position[i.shelf1.shelfId].Split(';') ;
                    xrec1 = Int32.Parse(parameters[0]);
                    yrec1 = Int32.Parse(parameters[1]);

                    //extract coordinates of second recommendation
                    parameters = shelves_position[j.shelf1.shelfId].Split(';');
                    xrec2 = Int32.Parse(parameters[0]);
                    yrec2 = Int32.Parse(parameters[1]);

                    //calculate paths from start to rec1, from rec1 to rec2, from rec2 to destination
                    CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color, shop_graph);
                    CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, xrec2, yrec2, color, shop_graph);
                    CarpetPath subPath3 = calculateSubPath(xrec2, yrec2, x2, y2, color, shop_graph);

                    //calculate total cost and verify if it is acceptable
                    recCost = subPath1.cost + subPath2.cost + subPath3.cost;
                    double ratio = (double)recCost / (double)noRecCost;
                    //if it is ok, then create final path and return it
                    if (ratio < MAX_DEVIATION_FACTOR)
                    {
                        subPath1.appendPath(subPath2);
                        subPath1.appendPath(subPath3);
                        return subPath1;
                    }
                }
            }

            //NOTE: code below executed only if no path available with 2 recommendations
            foreach (item i in recommendations)
            {
                //extract recommendations coordinates
                String[] parameters;
                int xrec1, yrec1;
                parameters = shelves_position[i.shelf1.shelfId].Split(';');
                xrec1 = Int32.Parse(parameters[0]);
                yrec1 = Int32.Parse(parameters[1]);

                //calculate subpath from start to rec and from rec to destination
                CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color, shop_graph);
                CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, x2, y2, color, shop_graph);

                //calculate cost and compare with noPath cost, if acceptable then construct path and return
                recCost = subPath1.cost + subPath2.cost;
                double ratio = (double)recCost / (double)noRecCost;
                if (ratio < MAX_DEVIATION_FACTOR)
                {
                    subPath1.appendPath(subPath2);                 
                    return subPath1;
                }
            }
            //if no path with recommendations found, then return noRecPath
            return noRecPath;
        }
    }
}
