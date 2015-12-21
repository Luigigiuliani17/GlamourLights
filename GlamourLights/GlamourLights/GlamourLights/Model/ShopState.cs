using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    public enum Colors{
        RED=0, GREEN=1, BLUE=2, YELLOW=3
    }
    /// <summary>
    /// class that contains all the informations about the shop state
    /// - matrix of the shop (-1 = wall; 0= shelf; 1= free; 2= free used as stating point for the paths)
    /// - shop graph. It is build removing the nodes with costs in {-1, 0}
    /// 
    /// </summary>
    class ShopState
    {
        // -1 = wall, 0= shelf 1= free 2 = starting point
        public int[,] shop_layout_matrix { get; set; }
        // graph used for algorithm
        public Dictionary<string, Graphvertex> shop_graph { get; set; }
        public bool[] used_colors; 
        //  Default cost of a node
        public const int DEFAULT_COST = 1;
        public const int MAX_USERS_NUMBER = 4;


        public ShopState()
        {
            //set at false every used colour
            used_colors = new bool[MAX_USERS_NUMBER];
            for(int i=0; i<MAX_USERS_NUMBER; i++)
            {
                used_colors[i] = false;
            }
            
            MatrixParser par = new MatrixParser();
            shop_layout_matrix = par.parseMatrix();
            createShopGraph();
        }

        private void createShopGraph()
        {
            shop_graph = new Dictionary<string, Graphvertex>();
            //get matrix dimension
            int numRows = shop_layout_matrix.GetUpperBound(0)+1;
            int numCols = shop_layout_matrix.GetUpperBound(1)+1;
           
            //generate graph from the parsed matrix
            for (int i=0; i< numRows; i++) 
                for(int j=0; j<numCols; j++)
                {
                    //if there is a wall or a shelf
                    if (shop_layout_matrix[i, j] == -1 || shop_layout_matrix[i, j] == 0)
                        continue;
                    //create a new vertex and add it to the vertex dictionary
                    Graphvertex Vnext = new Graphvertex(DEFAULT_COST, i, j);
                    
                    string k = i + ";" + j;
                    shop_graph.Add(k, Vnext);
                }
            //add adjacent vertex to all the vertexes
            foreach (var vcur in shop_graph.Values)
            {
                int x = vcur.x_cord;
                int y = vcur.y_cord;

                //add up (if exists)
                if (x!=0)
                    if(shop_layout_matrix[x-1,y]>0)
                    {
                        string k = (x-1) + ";" + y;
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add right (if exists)
                if (y != numCols-1)
                    if (shop_layout_matrix[x, y+1] > 0)
                    {
                        string k = x + ";" + (y+1);
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add down (if exists)
                if (x != numRows - 1)
                    if (shop_layout_matrix[x+1, y] > 0)
                    {
                        string k = (x+1) + ";" + y;
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add left (if exists)
                if (y != 0)
                    if (shop_layout_matrix[x, y-1] > 0)
                    {
                        string k = x + ";" + (y-1);
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }
            }
        }
    }
}
