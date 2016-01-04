using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Collections.Concurrent;
using GlamourLights.Model;

namespace GlamourLights.Controller

{
    class Blinker
    {
        //These lists will contain strings formatted in such a way "x;y"
        ConcurrentBag<BlinkPoint> red = new ConcurrentBag<BlinkPoint>();
        ConcurrentBag<BlinkPoint> green = new ConcurrentBag<BlinkPoint>();
        ConcurrentBag<BlinkPoint> blue = new ConcurrentBag<BlinkPoint>();
        ConcurrentBag<BlinkPoint> yellow = new ConcurrentBag<BlinkPoint>();
        int n_overlapping = 0;
        public bool blink { get; set; }
        public bool insideBlink { get; set; }
        ShopState state;

        public Blinker(ShopState state)
        {
            this.state = state;
            this.blink = false;
            this.insideBlink = false;
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
            Console.WriteLine("CheckOverlapping --> START");
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
                        Console.WriteLine("Ci sono sovrapposizioni");
                        isOverlapping = true;

                            if (vertex.active_colors[0] == true)
                                    red.Add(new BlinkPoint(path_coord[i]));

                            if (vertex.active_colors[1] == true)
                                    green.Add(new BlinkPoint(path_coord[i]));

                            if (vertex.active_colors[2] == true)
                                    blue.Add(new BlinkPoint(path_coord[i]));

                            if (vertex.active_colors[3] == true)
                                    yellow.Add(new BlinkPoint(path_coord[i]));
                    }
                }
                if (isOverlapping)
                {
                    n_overlapping += 1;
                    blink = true;
                    Console.WriteLine("Valore di blink: " + blink);
                    return true;
                    Console.WriteLine("CheckOverlapping --> END with TRUE");
                }
            }
            Console.WriteLine("CheckOverlapping --> END with FALSE");
            return false;
        }

        /// <summary>
        /// This function will perform the asynchronous blinking until the variable is set as true 
        /// </summary>
        public void StartBlink(SerialPort serial)
        {
            Console.WriteLine("StartBlink --> START");
            insideBlink = true;
            string[] mess = new string[2];
            //This will loop until the variable is set to false blinking the needed points
            Console.WriteLine("STARTING loop");
            while (blink)
            {
                //red list
                if (red.Count > 0)
                {
                    Console.WriteLine("Blink ROSSO");
                        foreach (BlinkPoint p in red)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.red + 1);
                            Console.WriteLine("serial message red: " + message);
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }
                if (green.Count > 0)
                {
                        foreach (BlinkPoint p in green)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.green + 1);
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }

                //blue list
                if (blue.Count > 0)
                {
                    Console.WriteLine("Blink BLUE");
                        foreach (BlinkPoint p in blue)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.blue + 1);
                            Console.WriteLine("serial message blue: " + message);
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }

                //yellow list
                if (yellow.Count > 0)
                {
                        foreach (BlinkPoint p in yellow)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.yellow + 1);
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }
                Console.WriteLine("RESTARTING loop");
            }
            Console.WriteLine("ENDING loop");
            Console.WriteLine("StartBlink --> END");
            insideBlink = false;
            return;
        }

        /// <summary>
        /// This method will update the color lists adding or removing overlapping path from them
        /// </summary>
        public void UpdateBlinker(CarpetPath path)
        {
            Console.WriteLine("UpdateBlinker --> START");
            Dictionary<string, Graphvertex> graph = state.shop_graph;
            int[] x = path.x_cordinates;
            int[] y = path.y_cordinates;
            string[] coord = new string[x.Length];
            int color_code = (int)path.color;
            ConcurrentBag<BlinkPoint> list = new ConcurrentBag<BlinkPoint>();
            bool overlapping_found = false;

            //Checking which list must be updated with switch over the color_id
            switch(color_code)
            {
                case 0 :
                    list = red;
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

                //invaliding coordinates from the right list
                foreach (BlinkPoint p in list)
                {
                    if (p.coord == coord[i])
                    {
                        overlapping_found = true;
                        p.isValid = false;
                    }
                }
            }

            //setting number of overlapping if found
            if (overlapping_found == true)
                n_overlapping -= 1;
            Console.WriteLine("Number of overlapping " + n_overlapping);
            //check if blinking is needed again, if not the variable is set to false
            if (n_overlapping == 0)
                blink = false;
            Console.WriteLine("Is necessary to blink: " + blink);

            Console.WriteLine("UpdateBlinker --> END");
        }

    }
}
