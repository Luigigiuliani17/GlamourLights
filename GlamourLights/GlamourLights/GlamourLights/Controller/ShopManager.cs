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
        public int remaining_cost { get; set; }

        public  const double MAX_DEVIATION_FACTOR = 1.8;

        public ShopManager(ShopState shop)
        {
            this.shopState = shop;
            this.com = new Comunicator(shop);
            this.recc = new Recommender(shop.shopDb);
            this.remaining_cost = 0;
            
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

        internal void saveChanges()
        {
            shopState.rebuild_shop_matrix();
            shopState.rebuild_shelves_position();
            shopState.rebuild_department_position();
            shopState.rebuild_hotspot_position();
            shopState = new ShopState();
        }

        /// <summary>
        /// change the kind of a single cell of the matrix from "shelf" to "free" and viceversa 
        /// (if by mistake the user try to change a wall or a starting point, no changed will be made)
        /// </summary>
        /// <param name="x_cord"></param> of the cell to be changed
        /// <param name="y_cord"></param> of the cell to be changed
        internal void changeKind(int x_cord, int y_cord)
        {
            if(shopState.shop_layout_matrix[x_cord,y_cord]==1)
            {
                shopState.shop_layout_matrix[x_cord, y_cord] = 0;

            }
            else
            {
                if (shopState.shop_layout_matrix[x_cord, y_cord] == 0)
                    shopState.shop_layout_matrix[x_cord, y_cord] = 1;
            }
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

                    //verify if both recommendations have a light available, if not change recommendation
                    try
                    {
                        int lightId1 = shopState.lights_position[xrec1 + ";" + yrec1];
                        int lightId2 = shopState.lights_position[xrec2 + ";" + yrec2];
                        if (shopState.active_lights[lightId1] || shopState.active_lights[lightId2])
                            continue;
                    } catch (KeyNotFoundException e)
                    {
                        continue;
                    }
                    
                    //calculate paths from start to rec1, from rec1 to rec2, from rec2 to destination
                    CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color);
                    //Adding to the path cost 5
                    for (int e = 0; e < subPath1.x_cordinates.Length; e++)
                    {
                       shopState.shop_graph[subPath1.x_cordinates[e] + ";" + subPath1.y_cordinates[e]].cost += 5;
                    }
                    CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, xrec2, yrec2, color);
                    //Adding to the path cost 5
                    for (int e = 0; e < subPath2.x_cordinates.Length; e++)
                    {
                        shopState.shop_graph[subPath2.x_cordinates[e] + ";" + subPath2.y_cordinates[e]].cost += 5;
                    }
                    CarpetPath subPath3 = calculateSubPath(xrec2, yrec2, x2, y2, color);

                    //removing from the path cost 5
                    for (int e = 0; e < subPath1.x_cordinates.Length; e++)
                    {
                        shopState.shop_graph[subPath1.x_cordinates[e] + ";" + subPath1.y_cordinates[e]].cost += -5;
                    } //removing from the path cost 5
                    for (int e = 0; e < subPath2.x_cordinates.Length; e++)
                    {
                        shopState.shop_graph[subPath2.x_cordinates[e] + ";" + subPath2.y_cordinates[e]].cost += -5;
                    }

                    //calculate total cost and verify if it is acceptable
                    recCost = subPath1.cost + subPath2.cost + subPath3.cost;
                    remaining_cost = recCost - noRecCost;
                    double ratio = (double)recCost / (double)noRecCost;
                    //if it is ok, then create final path and return it
                    if (ratio < MAX_DEVIATION_FACTOR)
                    {
                        //try to insert hotspots
                //        subPath1 = tryToInsertHotspot(subPath1);
                //        subPath2 = tryToInsertHotspot(subPath2);
                 //       subPath3 = tryToInsertHotspot(subPath3);

                        subPath1.appendPath(subPath2);
                        subPath1.appendPath(subPath3);
                        subPath1.lightsCodes[0] = shopState.lights_position[xrec1 + ";" + yrec1];
                        subPath1.lightsCodes[1] = shopState.lights_position[xrec2 + ";" + yrec2];

                        //add recommendation positions
                        subPath1.x_recommendations[0] = xrec1;
                        subPath1.y_recommendations[0] = yrec1;
                        subPath1.x_recommendations[1] = xrec2;
                        subPath1.y_recommendations[1] = yrec2;

                        //add final light
                        if (shopState.lights_position.ContainsKey(subPath1.x_cordinates.Last() + ";" + subPath1.y_cordinates.Last()))
                            subPath1.destination_light_code = shopState.lights_position[subPath1.x_cordinates.Last() + ";" + subPath1.y_cordinates.Last()];
                        else
                            subPath1.destination_light_code = -1;

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
                try
                {
                    int lightId1 = shopState.lights_position[xrec1 + ";" + yrec1];
                    if (shopState.active_lights[lightId1])
                        continue;
                } catch ( KeyNotFoundException e)
                {
                    continue;
                }
                

                //calculate subpath from start to rec and from rec to destination
                CarpetPath subPath1 = calculateSubPath(x1, y1, xrec1, yrec1, color);
                //Adding to the path cost 5
                for (int e = 0; e < subPath1.x_cordinates.Length; e++)
                {
                    shopState.shop_graph[subPath1.x_cordinates[e] + ";" + subPath1.y_cordinates[e]].cost += 5;
                }
                CarpetPath subPath2 = calculateSubPath(xrec1, yrec1, x2, y2, color);

                //removing from the path cost 5
                for (int e = 0; e < subPath1.x_cordinates.Length; e++)
                {
                    shopState.shop_graph[subPath1.x_cordinates[e] + ";" + subPath1.y_cordinates[e]].cost += -5;
                }

                //calculate cost and compare with noPath cost, if acceptable then construct path and return
                recCost = subPath1.cost + subPath2.cost;
                remaining_cost = recCost - noRecCost;
                double ratio = (double)recCost / (double)noRecCost;
                if (ratio < MAX_DEVIATION_FACTOR)
                {
                    //try to insert hotspots in the 2 subpaths
                    subPath1 = tryToInsertHotspot(subPath1,1);

                    subPath1.appendPath(subPath2);
                    subPath1.lightsCodes[0] = shopState.lights_position[xrec1 + ";" + yrec1];
                    subPath1.lightsCodes[1] = -1;

                    //add recomendation positions
                    subPath1.x_recommendations[0] = xrec1;
                    subPath1.y_recommendations[0] = yrec1;

                    //add final light
                    if (shopState.lights_position.ContainsKey(subPath1.x_cordinates.Last() + ";" + subPath1.y_cordinates.Last()))
                        subPath1.destination_light_code = shopState.lights_position[subPath1.x_cordinates.Last() + ";" + subPath1.y_cordinates.Last()];
                    else
                        subPath1.destination_light_code = -1;

                    return subPath1;
                }
            }
            /////////////////NOTE: code below executed if no recommendation found!!!!!!!!!!!!!!!!!!
            //if no path with recommendations found, then return noRecPath
            remaining_cost = (int)MAX_DEVIATION_FACTOR * noRecCost - noRecCost;
            noRecPath = tryToInsertHotspot(noRecPath, 2);

            //add final light
            if (shopState.lights_position.ContainsKey(noRecPath.x_cordinates.Last() + ";" + noRecPath.y_cordinates.Last()))
                noRecPath.destination_light_code = shopState.lights_position[noRecPath.x_cordinates.Last() + ";" + noRecPath.y_cordinates.Last()];
            else
                noRecPath.destination_light_code = -1;

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

            //find the best stating point
            int min = 99;
            int min_index = 0;
            for(int i=0; i<shopState.start_usage.Length; i++)
            {
                if(shopState.start_usage[i]<min)
                {
                    min = shopState.start_usage[i];
                        min_index = i;
                }
            }


            //calculate carpet path
            CarpetPath path = calculateCompletePath(recommendations, shopState.x_start[min_index], shopState.y_start[min_index], xdest1, ydest1, col);

            //launch comunicator function to activate path
            //this will be launched on a thread, to allow some kind of parallelism and not freeze the application while 
            //the path is be drawn
            Thread myThread = new Thread(() => com.DrawPath(path));
            myThread.Start();
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

            //find the best stating point
            int min = 99;
            int min_index = 0;
            for (int i = 0; i < shopState.start_usage.Length; i++)
            {
                if (shopState.start_usage[i] < min)
                {
                    min = shopState.start_usage[i];
                    min_index = i;
                }
            }

            //calculate carpet path
            CarpetPath path = calculateCompletePath(recommendations, shopState.x_start[min_index], shopState.y_start[min_index], xdest1, ydest1, col);

            //launch comunicator function to activate path
            //this will be launched on a thread, to allow some kind of parallelism and not freeze the application while 
            //the path is be drawn
            Thread myThread = new Thread(() => com.DrawPath(path));
            myThread.Start();
        }

        /// <summary>
        /// function that tries to insert a definite number of hotspots between the start and the arrival of a path
        /// </summary>
        /// <param name="oldPath"></param> path in which you want to insert an hotspot
        /// <param name="num_hospots"></param> number of hotspot to try to insert. it will insert the max number possible
        /// <returns></returns> a path with an hotspot (if possible)
        public CarpetPath tryToInsertHotspot(CarpetPath oldPath, int num_hotspots)
        {
            CarpetPath newPath1 = new CarpetPath();
            CarpetPath newPath2 = new CarpetPath();
            CarpetPath newPath3 = new CarpetPath();
            int newCost;

            if(num_hotspots == 1)
            {
                string[] parameters;
                int x, y;
                foreach (string h in shopState.hotspot_position)
                {
                    //retrieve coordinates of every hotspot
                    parameters = h.Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    newPath1 = calculateSubPath(oldPath.x_cordinates[0], oldPath.y_cordinates[0], x, y, oldPath.color);

                    //Adding to the path cost 5
                    for (int e = 0; e < newPath1.x_cordinates.Length; e++)
                    {
                        shopState.shop_graph[newPath1.x_cordinates[e] + ";" + newPath1.y_cordinates[e]].cost += 5;
                    }
                    newPath2 = calculateSubPath(x, y, oldPath.x_cordinates.Last(), oldPath.y_cordinates.Last(), oldPath.color);

                    //removing from the path cost 5
                    for (int e = 0; e < newPath1.x_cordinates.Length; e++)
                    {
                        shopState.shop_graph[newPath1.x_cordinates[e] + ";" + newPath1.y_cordinates[e]].cost += -5;
                    }

                    newCost = newPath1.cost + newPath2.cost;

                    //if new cost - old cost > remaining cost, continue
                    if (newCost - oldPath.cost > remaining_cost)
                        continue;
                    else
                    {
                        //update remaining cost
                        remaining_cost = remaining_cost - (newCost - oldPath.cost);
                        newPath1.appendPath(newPath2);
                        //add to recommendation list;

                        newPath1.x_recommendations[1] = x;
                        newPath1.y_recommendations[1] = y;
                        return newPath1;
                    }
                }
            }

            if(num_hotspots == 2)
            {
                foreach(string h in shopState.hotspot_position)
                {
                    foreach(string k in shopState.hotspot_position)
                    {
                        //if same hotspot just continue
                        if (h.Equals(k))
                            continue;

                        //extract the cordinates of the 2 hotspots
                        string[] parameters;
                        int x1, y1, x2, y2;
                        parameters = h.Split(';');
                        x1 = Int32.Parse(parameters[0]);
                        y1 = Int32.Parse(parameters[1]);
                        parameters = k.Split(';');
                        x2 = Int32.Parse(parameters[0]);
                        y2 = Int32.Parse(parameters[1]);

                        /////////calculate subpaths  start-hotspot1 ,h1-h2,  h2-destination
                        newPath1 = calculateSubPath(oldPath.x_cordinates[0], oldPath.y_cordinates[0], x1, y1, oldPath.color);
                        //Adding to the path cost 5
                        for (int e = 0; e < newPath1.x_cordinates.Length; e++)
                        {
                            shopState.shop_graph[newPath1.x_cordinates[e] + ";" + newPath1.y_cordinates[e]].cost += 5;
                        }
                        newPath2 = calculateSubPath(x1, y1, x2, y2, oldPath.color);
                        //Adding to the path cost 5
                        for (int e = 0; e < newPath2.x_cordinates.Length; e++)
                        {
                            shopState.shop_graph[newPath2.x_cordinates[e] + ";" + newPath2.y_cordinates[e]].cost += 5;
                        }

                        newPath3 = calculateSubPath(x2, y2, oldPath.x_cordinates.Last(),oldPath.y_cordinates.Last(), oldPath.color);
                        //removing from the path cost 5
                        for (int e = 0; e < newPath1.x_cordinates.Length; e++)
                        {
                            shopState.shop_graph[newPath1.x_cordinates[e] + ";" + newPath1.y_cordinates[e]].cost += -5;
                        } //removing from the path cost 5
                        for (int e = 0; e < newPath2.x_cordinates.Length; e++)
                        {
                            shopState.shop_graph[newPath2.x_cordinates[e] + ";" + newPath2.y_cordinates[e]].cost += -5;
                        }

                        newCost = newPath1.cost + newPath2.cost + newPath3.cost;
                        //if new cost - old cost > remaining cost, continue
                        if (newCost - oldPath.cost > remaining_cost)
                            continue;
                        else
                        {
                            //update remaining cost
                            remaining_cost = remaining_cost - (newCost - oldPath.cost);
                            newPath1.appendPath(newPath2);
                            newPath1.appendPath(newPath3);

                            //add to recommendation list;
                            newPath1.x_recommendations[0] = x1;
                            newPath1.y_recommendations[0] = y1;
                            newPath1.x_recommendations[1] = x2;
                            newPath1.y_recommendations[1] = y2;
                            //return final ok path
                            return newPath1;
                        }
                    }
                }
                //done if no couple of hotspots available ---> try to insert only 1 single hotspot
                return tryToInsertHotspot(oldPath, 1);
            }

            //done if no hotspot reachable
            return oldPath;
        }
    }
}
