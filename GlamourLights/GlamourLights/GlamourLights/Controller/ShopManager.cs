using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamourLights.Model;
using System.Data.Entity;
using System.Threading;

namespace GlamourLights.Controller
{
    public class ShopManager
    {
        public ShopState shopState { get; set; }
        public Comunicator com { get; set; }
        public Recommender recc { get; set; }

        public  const double MAX_DEVIATION_FACTOR = 1.8;

        public ShopManager(ShopState shop)
        {
            this.shopState = shop;
            this.com = new Comunicator(shop);
            
        }

        /// <summary>
        /// calculates the less costly path from a starting point and a destination using dikstra algorithm 
        /// </summary>
        /// <param name="x1"></param> x cord of starting point
        /// <param name="y1"></param> y cord of starting point
        /// <param name="x2"></param> x cord of destination
        /// <param name="y2"></param> y cord of destination
        /// <param name="color"></param> color of the path
        /// <returns></returns> the path found
        public CarpetPath calculateSubPath(int x1, int y1, int x2, int y2, CarpetColors color )
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
            foreach (var v in shopState.shop_graph)
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

                foreach(var n in shopState.shop_graph[u].adjacent_nodes)
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
                x_cord.Insert(0, shopState.shop_graph[u].x_cord);
                y_cord.Insert(0, shopState.shop_graph[u].y_cord);
                u = predecessor[u];
            }
            x_cord.Insert(0, shopState.shop_graph[u].x_cord);
            y_cord.Insert(0, shopState.shop_graph[u].y_cord);

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
        /// <param name="shelves_position"></param> dictionary of positions of shelves (x, y saved as a string of kind x;y )
        /// <returns></returns> a new path passing also from the recommendations (if it is possible)
        public CarpetPath calculateCompletePath(item[] recommendations, int x1, int y1, int x2, int y2, CarpetColors color)
        {
          
            //support variables
            int noRecCost = 0, recCost = 0;
            CarpetPath noRecPath;
            
            //calculatepath cost of the path without recommendations
            noRecPath = calculateSubPath(x1, y1, x2, y2, color);
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
                    parameters = shopState.shelves_position[i.shelf1.shelfId].Split(';') ;
                    xrec1 = Int32.Parse(parameters[0]);
                    yrec1 = Int32.Parse(parameters[1]);

                    //extract coordinates of second recommendation
                    parameters = shopState.shelves_position[j.shelf1.shelfId].Split(';');
                    xrec2 = Int32.Parse(parameters[0]);
                    yrec2 = Int32.Parse(parameters[1]);

                    //verify if both recommendations have a light available
                    int lightId1 = shopState.lights_position[xrec1 + ";" + yrec1];
                    int lightId2 = shopState.lights_position[xrec2 + ";" + yrec2];
                    if (shopState.active_lights[lightId1] || shopState.active_lights[lightId2])
                        continue;

                    //calculate paths from start to rec1, from rec1 to rec2, from rec2 to destination
                    CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color);
                    CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, xrec2, yrec2, color);
                    CarpetPath subPath3 = calculateSubPath(xrec2, yrec2, x2, y2, color);

                    //calculate total cost and verify if it is acceptable
                    recCost = subPath1.cost + subPath2.cost + subPath3.cost;
                    double ratio = (double)recCost / (double)noRecCost;
                    //if it is ok, then create final path and return it
                    if (ratio < MAX_DEVIATION_FACTOR)
                    {
                        subPath1.appendPath(subPath2);
                        subPath1.appendPath(subPath3);
                        subPath1.lightsCodes[0] = shopState.lights_position[xrec1 + ";" + yrec1];
                        subPath1.lightsCodes[1] = shopState.lights_position[xrec2 + ";" + yrec2];
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
                parameters = shopState.shelves_position[i.shelf1.shelfId].Split(';');
                xrec1 = Int32.Parse(parameters[0]);
                yrec1 = Int32.Parse(parameters[1]);

                //if the recomendation has the light not available, then skip it
                int lightId1 = shopState.lights_position[xrec1 + ";" + yrec1];
                if (shopState.active_lights[lightId1])
                    continue;

                //calculate subpath from start to rec and from rec to destination
                CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color);
                CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, x2, y2, color);

                //calculate cost and compare with noPath cost, if acceptable then construct path and return
                recCost = subPath1.cost + subPath2.cost;
                double ratio = (double)recCost / (double)noRecCost;
                if (ratio < MAX_DEVIATION_FACTOR)
                {
                    subPath1.appendPath(subPath2);
                    subPath1.lightsCodes[0] = shopState.lights_position[xrec1 + ";" + yrec1];
                    subPath1.lightsCodes[1] = -1;
                    return subPath1;
                }
            }
            //if no path with recommendations found, then return noRecPath
            return noRecPath;
        }

        /// <summary>
        /// functions that looks for an available colors and return his code. It returns -1 if no colors available
        /// </summary>
        /// <returns></returns> number of color available, -1 otherwise
        public int getAvailableColor()
        {
            //if there is a color not active, return its number
            for(int i=0; i<shopState.active_colors.Length; i++)
            {
                if (!shopState.active_colors[i])
                {
                    return i;
                }
            }
            //else return -1
            return -1;
        }

        /// <summary>
        /// //////ITEM VERSION/////// functions that makes all the procedures involved in the path finding process
        /// 1- if the user is registered, calculate personalized recommendations (not personalised otherwise)
        /// 2- calculate the path with the recommendations
        /// 3- activate the functions on the communicator to display the path and to turn on the lights
        /// </summary>
        /// <param name="item_chosen"></param> the id of the item chosen from the user
        /// <param name="cust"></param> the id of the customer (-1 if guest user)
        /// <param name="col"></param> the color assgined to the customer
        public void executePathFinding(item item_chosen, int customer_id, CarpetColors col)
        {
            //calculate recommendations
            item[] recommendations;
            if (customer_id >= 0)
                recommendations = recc.getPersonalizedRecommendations(customer_id);
            else
                recommendations = recc.getNotPersonalizedRecommendations();

            //retrieve destination coordinates
            String[] parameters;
            int xdest1, ydest1;
            parameters = shopState.shelves_position[item_chosen.shelf1.shelfId].Split(';');
            xdest1 = Int32.Parse(parameters[0]);
            ydest1 = Int32.Parse(parameters[1]);

            //calculate carpet path
            CarpetPath path = calculateCompletePath(recommendations, shopState.x_start, shopState.y_start, xdest1, ydest1, col);

            //launch comunicator function to activate path
            com.DrawPath(path);
        }

        /// <summary>
        /// ///DEPARTMENT VERSION/////// functions that makes all the procedures involved in the path finding process
        /// 1- if the user is registered, calculate personalized recommendations (not personalised otherwise)
        /// 2- calculate the path with the recommendations
        /// 3- activate the functions on the communicator to display the path and to turn on the lights
        /// </summary>
        /// <param name="department_code"></param>
        /// <param name="customer_id"></param>
        /// <param name="col"></param>
        public void executePathFinding(int department_code, int customer_id, CarpetColors col)
        {
            //calculate recommendations
            item[] recommendations;
            if (customer_id >= 0)
                recommendations = recc.getPersonalizedRecommendations(customer_id);
            else
                recommendations = recc.getNotPersonalizedRecommendations();

            //retrieve department coordinates
            String[] parameters;
            int xdest1, ydest1;
            parameters = shopState.department_position[department_code].Split(';');
            xdest1 = Int32.Parse(parameters[0]);
            ydest1 = Int32.Parse(parameters[1]);

            //calculate carpet path
            CarpetPath path = calculateCompletePath(recommendations, shopState.x_start, shopState.y_start, xdest1, ydest1, col);

            //launch comunicator function to activate path
            //this will be launched on a thread, to allow some kind of parallelism and not freeze the application while 
            //the path is be drawn
            Thread myThread = new Thread(() => com.DrawPath(path));
            myThread.Start();
            

        }
    }
}
