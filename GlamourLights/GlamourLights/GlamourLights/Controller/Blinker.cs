using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using GlamourLights.Model;

namespace GlamourLights.Controller

{
    class Blinker
    {
        //These lists will contain strings formatted in such a way "x;y"
        List<string> red = new List<string>();
        List<string> green = new List<string>();
        List<string> blue = new List<string>();
        List<string> yellow = new List<string>();
        int n_overlapping = 0;
        public bool blink { get; set; }
        ShopState state;

        public Blinker(ShopState state)
        {
            this.state = state;
            this.blink = false;
        }

        /// <summary>
        /// this function will check for every new path if there are some overlapping with other path already
        /// existent. If there are overlapping coordinates, these are saved in the right lists (corresponding to the
        /// right color) and the method will return true, otherwise nothing happen and it will return false
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CheckOverlapping(CarpetPath path)
        {
            bool isOverlapping = false;

            if (state.active_path.Count > 0)
            {
                //Fill the array with the pair of coordinates for the path formatted like "x;y"
                string[] path_coord = new string[path.x_cordinates.Length];
                for (int i = 0; i < path.x_cordinates.Length; i++)
                    path_coord[i] = path.x_cordinates[i] + ";" + path.y_cordinates[i];

                //Use the graph to save the coordinates overlapping in the right lists
                Dictionary<string, Graphvertex> graph = state.shop_graph;
                for(int i = 0; i<path_coord.Length; i++)
                {
                    Graphvertex vertex = graph[path_coord[i]];
                    //If the vertex has more than 1 color active, put it in the right lists
                    if (vertex.n_activeColors > 1)
                    {
                        isOverlapping = true;

                        lock(red)
                        {
                            if (vertex.active_colors[0] == true)
                                red.Add(path_coord[i]);
                        }

                        lock(green)
                        {
                            if (vertex.active_colors[1] == true)
                                green.Add(path_coord[i]);
                        }

                        lock(blue)
                        {
                            if (vertex.active_colors[2] == true)
                                blue.Add(path_coord[i]);
                        }

                        lock(yellow)
                        {
                            if (vertex.active_colors[3] == true)
                                yellow.Add(path_coord[i]);
                        }
                    }
                }
                if (isOverlapping)
                {
                    n_overlapping += 1;
                    blink = true;
                    return true;
                }
            } 
            return false;
        }

        /// <summary>
        /// This function will perform the asynchronous blinking until the variable is set as true 
        /// </summary>
        public void StartBlink(SerialPort serial)
        {
            string[] mess = new string[2];
            //This will loop until the variable is set to false blinking the needed points
            while(blink)
            {
                //red list
                if (red.Count > 0)
                {
                    lock(red)
                    {
                        foreach (string s in red)
                        {
                            mess = s.Split(';');
                            if (serial.IsOpen)
                                serial.WriteLine(mess[0] + ":" + mess[1] + ":" + (int)CarpetColors.red + 1);
                        }
                    }
                    Thread.Sleep(200);
                }

                //green list
                if (green.Count > 0)
                {
                    lock(green)
                    {
                        foreach (string s in green)
                        {
                            mess = s.Split(';');
                            if (serial.IsOpen)
                                serial.WriteLine(mess[0] + ":" + mess[1] + ":" + (int)CarpetColors.green + 1);
                        }
                    }
                    Thread.Sleep(200);
                }

                //blue list
                if (blue.Count > 0)
                {
                    lock(blue)
                    {
                        foreach (string s in blue)
                        {
                            mess = s.Split(';');
                            if (serial.IsOpen)
                                serial.WriteLine(mess[0] + ":" + mess[1] + ":" + (int)CarpetColors.blue + 1);
                        }
                    }
                    Thread.Sleep(200);
                }

                //yellow list
                if (yellow.Count > 0)
                {
                    lock(yellow)
                    {
                        foreach (string s in yellow)
                        {
                            mess = s.Split(';');
                            if (serial.IsOpen)
                                serial.WriteLine(mess[0] + ":" + mess[1] + ":" + (int)CarpetColors.yellow + 1);
                        }
                    }
                    Thread.Sleep(200);
                }
            }

            return;
        }

        /// <summary>
        /// This method will update the color lists adding or removing overlapping path from them
        /// </summary>
        public void UpdateBlinker(CarpetPath path)
        {
            Dictionary<string, Graphvertex> graph = state.shop_graph;
            int[] x = path.x_cordinates;
            int[] y = path.y_cordinates;
            string[] coord = new string[x.Length];
            int color_code = (int)path.color;
            List<string> list = new List<string>();
            bool overlapping_found = false;

            //Checking which list must be updated with switch over the color_id
            switch(color_code)
            {
                case 0 :
                    list = blue;
                    break;

                case 1:
                    list = green;
                    break;

                case 2:
                    list = blue;
                    break;

                case 3:
                    list = yellow;
                    break;

            }
            //this will create the array with the keys for the dictionary
            for (int i = 0; i < x.Length; i++)
                coord[i] = x[i] + ";" + y[i];

            //Loop through the dictionary to update colors of vertex and the number of active colors
            for (int i = 0; i < coord.Length; i++)
            {
                graph[coord[i]].active_colors[color_code] = false;
                graph[coord[i]].n_activeColors -= 1;

                //removing coordinates from the right list
                if(list.Contains(coord[i]))
                {
                    overlapping_found = true;
                    list.Remove(coord[i]);
                }
            }

            //setting number of overlapping if found
            if (overlapping_found == true)
                n_overlapping -= 1;

            //check if blinking is needed again, if not the variable is set to false
            if(n_overlapping == 0)
                blink = false;
        }

    }
}
