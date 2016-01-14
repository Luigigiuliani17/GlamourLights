
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Collections.Concurrent;
using GlamourLights.Model;
using System.Runtime.CompilerServices;

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
            int color = (int)path.color;
            bool[] color_found = new bool[4];

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
                        //Check wich how many colors the path is overlapping
                        for (int j = 0; j<vertex.active_colors.Length; j++)
                        {
                            if (vertex.active_colors[j] == true)
                                color_found[j] = true;
                        }
                    }
                }
                for (int z = 0; z < color_found.Length; z++)
                {
                    Console.WriteLine("valori dell'array dei colori: " + color_found[z]);
                    if (color_found[z] == true && color != z)
                        this.UpdateNOverlapping(1);
                }
                if(n_overlapping > 0)
                { 
                    blink = true;
                    Console.WriteLine("Valore di blink: " + blink);
                    Console.WriteLine("numero di overlapping e' " + n_overlapping);
                    Console.WriteLine("CheckOverlapping --> END with TRUE");
                    return true;
                    
                }
            }
            Console.WriteLine("CheckOverlapping --> END with FALSE");
            return false;
        }


        /// <summary>
        /// Add every single point of the path to a list to blink
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color_code"></param>
        public void AddOverlapping(int x, int y)
        {
            if (state.active_path.Count > 0)
            {
                string coord = x + ";" + y;
                //Use the graph to save the coordinates overlapping in the right lists
                Dictionary<string, Graphvertex> graph = state.shop_graph;
               
                    Graphvertex vertex = graph[coord];
                    //If the vertex has more than 1 color active, put it in the right lists
                    if (vertex.n_activeColors > 1)
                    {
                    Console.WriteLine("sto per fare l'update a una delle liste.");
                    if (vertex.active_colors[0] == true)
                    {
                        Console.WriteLine("aggiungo un rosso");
                        red.Add(new BlinkPoint(coord));
                    }

                    if (vertex.active_colors[1] == true)
                    {
                        Console.WriteLine("aggiungo un verde");
                        green.Add(new BlinkPoint(coord));
                    }

                    if (vertex.active_colors[2] == true)
                    {
                        Console.WriteLine("aggiungo un blu");
                        blue.Add(new BlinkPoint(coord));
                    }

                    if (vertex.active_colors[3] == true)
                    {
                        Console.WriteLine("aggiungo un giallo");
                        yellow.Add(new BlinkPoint(coord));
                    }
                    }
                }
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
                        foreach (BlinkPoint p in red)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.red + 1) + ".";
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
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.green + 1) + ".";
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }

                //blue list
                if (blue.Count > 0)
                {
                        foreach (BlinkPoint p in blue)
                        {
                        if (p.isValid == true)
                        {
                            mess = p.coord.Split(';');
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.blue + 1) + ".";
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
                            string message = mess[0] + ":" + mess[1] + ":" + ((int)CarpetColors.yellow + 1) + ".";
                            if (serial.IsOpen)
                                serial.WriteLine(message);
                        }
                        }
                    Thread.Sleep(300);
                }
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
            bool[] color_found = new bool[4];

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
                Graphvertex vertex = graph[coord[i]];
                vertex.active_colors[color_code] = false;
                vertex.n_activeColors -= 1;

                for (int j = 0; j < vertex.active_colors.Length; j++)
                    if (vertex.active_colors[j] == true)
                        color_found[j] = true;

                //invaliding coordinates from the right list
                foreach (BlinkPoint p in list)
                {
                    if (p.coord == coord[i])
                    {
                        p.isValid = false;
                    }
                }
            }

            //setting number of overlapping if found
            for (int z = 0; z < color_found.Length; z++)
            {
                if (color_found[z] == true)
                    this.UpdateNOverlapping(-1);
            }
            Console.WriteLine("Number of overlapping " + n_overlapping);
            //check if blinking is needed again, if not the variable is set to false
            /*if (n_overlapping == 0)
                blink = false;*/
            Console.WriteLine("Is necessary to blink: " + blink);

            Console.WriteLine("UpdateBlinker --> END");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="n"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void UpdateNOverlapping(int n)
        {
            n_overlapping += n;
        }

    }
}
