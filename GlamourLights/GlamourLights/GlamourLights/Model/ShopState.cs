using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    public enum CarpetColors{
       red = 0,
       green = 1,
       blue =2,
       yellow = 3
    }
    /// <summary>
    /// class that contains all the informations about the shop state
    /// - matrix of the shop (-1 = wall; 0= shelf; 1= free; 2= free used as stating point for the paths)
    /// - shop graph. It is build removing the nodes with costs in {-1, 0}
    /// - active colors (to indicate what color is used at the moment
    /// - active path (to save the path of an active color)
    /// </summary>
    public class ShopState
    {
        // -1 = wall, 0= shelf 1= free 2 = starting point
        public int[,] shop_layout_matrix { get; set; }

        // graph used for algorithm
        public Dictionary<string, Graphvertex> shop_graph { get; set; }

        public bool[] active_colors { get; set; }
        public List<CarpetPath> active_path { get; set; }

        //dictionary of active_lights with lightId as key and values: true (if lights used) /false (if lights not used) 
        public Dictionary<int, bool> active_lights { get; set; }

        //  Default cost of a node
        public const int DEFAULT_COST = 1;
        public const int MAX_USERS_NUMBER = 4;

        //TODO define how to do this correctly
        public int[] x_start;
        public int[] y_start;
        public int[] start_usage;

        public ShopDb shopDb { get; set; }

        public Dictionary<int, string> shelves_position { get; set; }
        public Dictionary<int, string> department_position { get; set; }
        public Dictionary<string, int> lights_position { get; set; }

        /// <summary>
        /// constructor that creates a new shop state by 
        /// -parsing the matrix
        /// -creating the graph
        /// -setting to false every active_color
        /// TO CALL WHEN BOOTING THE APPLICATION
        /// </summary>
        public ShopState()
        {
            //set at false every used colour
            active_colors = new bool[MAX_USERS_NUMBER];
            active_path = new List<CarpetPath>();
            for (int i = 0; i < MAX_USERS_NUMBER; i++)
            {
                active_colors[i] = false;
            }

            MatrixParser par = new MatrixParser();
            shop_layout_matrix = par.parseMatrix();
            createShopGraph();
            shopDb = new ShopDb();

            //retrieve shleves, department  adn lights position
            shelves_position = par.parseShelvesPosition();
            department_position = par.parseDepartmentPosition();
            lights_position = par.parseLightsPositions();

            //set to false every lights (no light used at start!)
            active_lights = new Dictionary<int, bool>();
            foreach (var l in lights_position)
            {
                active_lights[l.Value] = false;
            }

            //find and set the initial positions
            List<int> x = new List<int>();
            List<int> y = new List<int>();
            for (int i = 0; i < shop_layout_matrix.GetLength(0); i++)
                for (int j = 0; j < shop_layout_matrix.GetLength(1); j++)
                    if (shop_layout_matrix[i, j] == 2)
                    {
                        x.Add(i);
                        y.Add(j);
                    }
            x_start = x.ToArray();
            y_start = y.ToArray();
            start_usage = new int[x_start.Length];
            for(int i=0; i<start_usage.Length; i++)
            {
                start_usage[i] = 0;
            }
        }

        private void createShopGraph()
        {
            shop_graph = new Dictionary<string, Graphvertex>();
            //get matrix dimension
            int numRows = shop_layout_matrix.GetUpperBound(0) + 1;
            int numCols = shop_layout_matrix.GetUpperBound(1) + 1;

            //generate graph from the parsed matrix
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
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
                if (x != 0)
                    if (shop_layout_matrix[x - 1, y] > 0)
                    {
                        string k = (x - 1) + ";" + y;
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add right (if exists)
                if (y != numCols - 1)
                    if (shop_layout_matrix[x, y + 1] > 0)
                    {
                        string k = x + ";" + (y + 1);
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add down (if exists)
                if (x != numRows - 1)
                    if (shop_layout_matrix[x + 1, y] > 0)
                    {
                        string k = (x + 1) + ";" + y;
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }

                //add left (if exists)
                if (y != 0)
                    if (shop_layout_matrix[x, y - 1] > 0)
                    {
                        string k = x + ";" + (y - 1);
                        Graphvertex vcheck;
                        shop_graph.TryGetValue(k, out vcheck);
                        vcur.adjacent_nodes.Add(k, vcheck);
                    }
            }
        }

        /// <summary>
        /// rebuild the shop matrix txt by preparing the input and calling matrixParser.uploadTxt
        /// </summary>
        public void rebuild_shop_matrix()
        {
            //create a list of lines
            List<String> lines = new List<string>();
            for (int i = 0; i < shop_layout_matrix.GetLength(0); i++)
                for (int j = 0; j < shop_layout_matrix.GetLength(1); j++)
                {
                    lines.Add(i + ";" + j + ";" + shop_layout_matrix[i, j]);

                }

            //call the right method of the parser
            MatrixParser par = new MatrixParser();
            par.uploadTxt(lines, "../../Resources/shop_layout.txt");
        }

        /// <summary>
        /// rebuild the shelves position txt by preparing the input and calling matrixparser.uploadTxt
        /// </summary>
        public void rebuild_shelves_position()
        {
            //prepare input by creating a list of string to be written
            List<String> lines = new List<string>();
            foreach (var v in shelves_position)
            {
                //retrieve x,y coordinates of each shelf
                String[] parameters;
                int x_cord, y_cord;
                parameters = shelves_position[v.Key].Split(';');
                x_cord = Int32.Parse(parameters[0]);
                y_cord = Int32.Parse(parameters[1]);

                lines.Add(v.Key + ";" + x_cord + ";" + y_cord);
            }

            //call the right method of the parser
            MatrixParser par = new MatrixParser();
            par.uploadTxt(lines, "../../Resources/shelves_position.txt");
        }
    }
}
