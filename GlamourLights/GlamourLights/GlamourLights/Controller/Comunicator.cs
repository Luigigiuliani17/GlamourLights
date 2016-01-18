using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO.Ports;
using System.Threading;
using GlamourLights.Model;

namespace GlamourLights.Controller
{

    /// <summary>
    ///  Class that will manage the comunication between the software and hardware (luminous carpet or in our case
    /// with the arduino board)
    /// comunication will be unilateral(software -> hardware)
    /// with well formatted strings, composed in this way:
    ///"x_pos:y_pos:color"
    ///which represents the "segment" of LED that will be switched on (or off, if the color is black) 
    ///x_pos  = the x coordinates of the point
    ///y_pos = the y coordinates of the point
    ///color = "red" or "green" or "blue" or "yellow" or "black" or "shelves or "walls" or "white"
    ///         the last one represents the switching off the LED, the others the colors of the LEDs to switch on
    /// In the class will be also calculated the "prohibite" zones, that will be never illuminated by any LED and
    ///rapresent the shelves and the walls of the shop
    ///the matrix numbers will represent the following things:
    ///-1 = wall
    ///0 = shelf
    ///To switch lights on, the x_pos and y_pos will be setted to "-1:-1:LEDcode" to switch lights on and to "-2:-2:LEDcode"
    ///to switch lights off
    /// </summary>

    public class Comunicator
    {
        ShopState state;
        SerialPort serial = new SerialPort();
        Blinker blink;

        /// <summary>
        /// The constructor will initialize the shop state and create a new blinker and start both threads of the normal
        /// color blinker and the special white blinker. Also opens the right serial port
        /// </summary>
        /// <param name="shop"></param>
        public Comunicator(ShopState shop)
        {
            this.state = shop;
            this.blink = new Blinker(shop);
            serial.PortName = "COM3";
            serial.BaudRate = 38400;
            serial.Open();
            blink.blink = true;
            blink.whiteBlink = true;
            new Thread(delegate ()
            {
                blink.StartBlink(serial);
            }).Start();
            new Thread(delegate ()
            {
                blink.WhiteBlinking(serial);
            }).Start();

        }

        /// <summary>
        /// This function will take the matrix structure in the ShopState, looping on it recognizing the walls = 4
        /// (identified with -1 in txt file) and the shelves = 5 (identified with 0 in txt file), put them in string form and comunicate them
        /// to the arduino board via serial communication.
        /// The string created will be formatted in this way: "x_pos:y_pos:color"
        /// </summary>
        public void InitializeMatrix()
        {
            int[,] matrix = state.shop_layout_matrix;
            //Here we create the strings and pass them to the board, looping through the matrix 
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {

                    if (serial.IsOpen)
                    {
                        if (matrix[i, j] == -1)
                        {
                            serial.WriteLine(i + ":" + j + ":" + "5."); //walls
                            Thread.Sleep(10);
                        }
                        if (matrix[i, j] == 0)
                        {
                            serial.WriteLine(i + ":" + j + ":" + "6."); //shelves 
                            Thread.Sleep(10);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will be called every time is necessary to draw a path on the carpet
        /// the parameters passed will be an Object of type CarpetPath, containing all the information necessary 
        /// to create the strings and process the requests
        /// if racommandations are present, the array of lights will be analyzed and if are present some codes different than 
        /// -1, the right string to switch on the lights are written and sent
        /// Every time a path is called a timer will be istantiated to temporize the appearence of the path itself.
        /// and the path to be deleted is set also.
        /// </summary>
        public void DrawPath(CarpetPath path)
        {
            int[] x_coord = path.x_cordinates;
            int[] y_coord = path.y_cordinates;
            int[] x_white = path.x_recommendations;
            int[] y_white = path.y_recommendations;
            //Retriving the color name
            int path_id = (int)path.color;
            int white = 4;
            int color = path_id + 1;
            //Setting the correct things in the shop state
            state.active_colors[path_id] = true;
            state.active_path.Add(path);
            foreach (CarpetPath l in state.active_path)
            {
                Console.WriteLine(l.color);
            }
            //updating usage of start point
            for (int i = 0; i < state.start_usage.Length; i++)
            {
                if (state.x_start[i] == x_coord[0] && state.y_start[i] == y_coord[0])
                    state.start_usage[i] += 1;
            }
            //Adding to the path cost 5
            for (int i = 0; i < x_coord.Length; i++)
            {
                state.shop_graph[x_coord[i] + ";" + y_coord[i]].cost += 5;
            }
            //if the path has raccomandations lights to switch on, here are sent the right string to the matrix
            //Here we set also the lights that are buisy in the shop
            for (int i = 0; i < path.lightsCodes.Length; i++)
            {
                if (path.lightsCodes[i] != -1 || path.destination_light_code != -1)
                    state.active_lights[path.lightsCodes[i]] = true;
                if (serial.IsOpen)
                {
                    if (path.lightsCodes[i] != -1)
                        serial.WriteLine("-1:-1:" + path.lightsCodes[i] + ".");
                }
            }
            //Send a string for every coordinate, plus the color
            for (int i = 0; i < path.x_cordinates.Length; i++)
            {
                if (serial.IsOpen)
                {
                    //White points control, here we add +1 to the counter in the right whitepoint in the blinker white list
                    if ((x_coord[i] == x_white[0] && y_coord[i] == y_white[0]) || (x_coord[i] == x_white[1] && y_coord[i] == y_white[1]) ||
                        (x_coord[i] == x_coord[path.x_cordinates.Length - 1] && y_coord[i] == y_coord[path.y_cordinates.Length - 1]))
                    {
                        blink.AddWhitePoint(x_coord[i], y_coord[i]);

                    } else {
                        serial.WriteLine(x_coord[i] + ":" + y_coord[i] + ":" + color + ".");
                        this.UpdateColorVertex(x_coord[i], y_coord[i], (int)path.color);
                        blink.AddOverlapping(x_coord[i], y_coord[i]);
                        Thread.Sleep(200);
                    }
                }
            }
            //Last destination light
            if (path.destination_light_code != -1)
                serial.WriteLine("-1:-1:" + path.destination_light_code + ".");
            //Timer part, in wich we bind the number of path to send to the handler, setting the time to wait 30 seconds
            var timer = new System.Timers.Timer { Interval = 30000, AutoReset = false };
            timer.Elapsed += (sender, e) => CallErasePath(sender, e, path_id);
            timer.Start();
        }

        /// <summary>
        /// This method will be called every time e path's timer finish.
        /// will be passed to it the number of path, with wich the method will call the true method that will erase
        /// the path in another thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="path_id"></param>
        void CallErasePath(object sender, ElapsedEventArgs e, int path_id)
        {
            new Thread(delegate ()
            {
                this.ErasePath(path_id);
            }).Start();
        }

        /// <summary>
        /// This method is fired by the event occurring when the timer elapse
        /// This method will form the right string to be passed to the serial port to switch off the lights
        /// and update the color of vertex in the graph and the counter of the withepoints for the blinker
        /// </summary>
        /// <param name="path_id"></param>
        private void ErasePath(int path_id)
        {
            //I have to find the path to erase from the list, giving the path_number
            CarpetPath path_to_erase = new CarpetPath();
            foreach (CarpetPath p in state.active_path)
            {
                if ((int)p.color == path_id)
                    path_to_erase = p;
            }
            //Retrieving coordinates
            int[] x_coord = path_to_erase.x_cordinates;
            int[] y_coord = path_to_erase.y_cordinates;
            //updating usage of start point
            for (int i = 0; i < state.start_usage.Length; i++)
            {
                if (state.x_start[i] == x_coord[0] && state.y_start[i] == y_coord[0])
                    state.start_usage[i] -= 1;
            }
            //here we switch off the light 
            //and we set that the switched off lights are free
            for (int i = 0; i < path_to_erase.lightsCodes.Length; i++)
            {
                if (path_to_erase.lightsCodes[i] != -1 || path_to_erase.destination_light_code != -1)
                    state.active_lights[path_to_erase.lightsCodes[i]] = false;
                if (serial.IsOpen)
                {
                    if (path_to_erase.lightsCodes[i] != -1)
                    {
                        string mes = "-2:-2:" + path_to_erase.lightsCodes[i] + ".";
                        serial.WriteLine(mes);
                    }
                }
            }
            //Update blinker: cheking if the path is overlapping with another
            //and update the state of the blink points that have to actually blink
            blink.UpdateBlinker(path_to_erase);
            //erase path
            if (serial.IsOpen)
            {
                for (int i = 0; i < path_to_erase.x_cordinates.Length; i++)
                {
                    serial.WriteLine(x_coord[i] + ":" + y_coord[i] + ":" + "-1.");
                    Thread.Sleep(10);
                }
            }
            if (path_to_erase.destination_light_code != -1)
            {
                string mes1 = "-2:-2:" + path_to_erase.destination_light_code + ".";
                serial.WriteLine(mes1);
            }
            //Set the accupation of the specific color to false again
            //Erasing a path from the list 
            state.active_colors[path_id] = false;
            state.active_path.Remove(path_to_erase);
            foreach (CarpetPath l in state.active_path)
            {
                Console.WriteLine(l.color);
            }
            //Lowering the path cost by 5
            for (int i = 0; i < x_coord.Length; i++)
            {
                state.shop_graph[x_coord[i] + ";" + y_coord[i]].cost -= 5;
            }
        }

        /// <summary>
        /// Update the color of each vertex of the graph coord by coord 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color_code"></param>
        private void UpdateColorVertex(int x, int y, int color_code)
        {
            Dictionary<string, Graphvertex> graph = state.shop_graph;
            string coord = x + ";" + y;

            //update colors of vertex and the number of active colors
            graph[coord].active_colors[color_code] = true;
            graph[coord].n_activeColors += 1;
        }

        /// <summary>
        /// This method will be invoked to stop both blinkers at the end of the program
        /// </summary>
        public void StopBlinker()
        {
            blink.blink = false;
            blink.whiteBlink = false;
        }
    }
}
