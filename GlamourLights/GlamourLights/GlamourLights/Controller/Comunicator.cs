using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GlamourLights.Model;

namespace GlamourLights.Controller
{
    /* Class that will manage the comunication between the software and hardware (luminous carpet or in our case
    with the arduino board)
    comunication will be unilateral (software -> hardware)
    with well formatted strings, composed in this way:
    "x0_pos:x1_pos:y0_pos:y1_pos:color"
    which represents the "segment" of LED that will be switched on (or off, if the color is black) 
    x0_pos -> x1_pos = the first and the last x coordinates of the segment 
    y0_pos -> y1_pos = the first and the last y coordinates of the segment
    color = "red" or "green" or "blue" or "yellow" or "black"
             the last one represents the switching off the LED, the others the colors of the LEDs to switch on

    In the class will be also calculated the "prohibite" zones, that will be never illuminated by any LED and 
    rapresent the shelves and the walls of the shop
    the matrix numbers will represent the following things:
    -1 = wall
     0 = shelf
    */
    class Comunicator
    {
        System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();
        ShopState state = new ShopState();
        List<CarpetPath> active_path;
        public Comunicator()
        {
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 9600;
        }

        /// <summary>
        /// This function will take the matrix structure in the ShopState, looping on it recognizing the walls
        /// (identified with -1) and the shelves (identified with 0), put them in string form and comunicate them
        /// to the arduino board via serial communication.
        /// </summary>
        public void InitializeMatrix()
        {
            serialPort1.Open();
            int[,] matrix = state.shop_layout_matrix;

            //Here we create the strings and pass them to the board, looping through the matrix 
            for (int i = 0; i < matrix.Rank; i++)
                for (int j = 0; j < matrix.GetLength(i); j++)
                {
                    if(matrix[i,j] == -1)
                    {
                        if (serialPort1.IsOpen)
                        {
                            serialPort1.WriteLine(String.Format("{x}:{y}:wall", i, j)); 
                        }
                    }
                    if (matrix[i, j] == 0)
                    {
                        if (serialPort1.IsOpen)
                        {
                            serialPort1.WriteLine(String.Format("{x}:{y}:shelf", i, j)); 
                        }
                    }
                }
            serialPort1.Close();
        }

        /// <summary>
        /// This method will be called every time is necessary to draw a path on the carpet
        /// the parameters passed will be an Object of type CarpetPath, containing all the information necessary 
        /// to create the strings and process the requests
        /// Every time a path is called a timer will be istantiated to temporize the appearence of the path itself.
        /// and the path to be deleted is set also.
        /// </summary>
        public void DrawPath(CarpetPath path)
        {
            active_path.Add(path);
            int[] x_coord = path.x_cordinates;
            int[] y_coord = path.y_cordinates;
            //Retriving the color name
            int n_color = (int)path.color;
            string color = Enum.GetName(typeof(CarpetColors), n_color);
            Console.WriteLine(color);
            //Opening the port
            serialPort1.Open();
            //Send a string for every coordinate, plus the color
            for(int i=0; i<path.x_cordinates.Length; i++)
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.WriteLine(String.Format("{x}:{y}:{color}",x_coord[i] ,y_coord[i], color));
                }
            }
            //Closing port and comunication
            serialPort1.Close();

             //Timer part, in wich we bind the number of path to send to the handler, setting the time to wait 30 seconds
            var timer = new Timer { Interval = 30000, AutoReset = false };
            timer.Elapsed += (sender, e) => ErasePath(sender, e, (int)path.color);
            timer.Start();
        }

        /// <summary>
        /// This method will be called every time e path's timer finish.
        /// will be passed to it the number of path, with wich the method can create the string to turn off the lights
        /// in the LED Matrix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="path_number"></param>
         void ErasePath(object sender, ElapsedEventArgs e, int path_number)
        {
            //I have to find the path to erase from the list, giving the path_number
            CarpetPath path_to_erase = null;
            foreach(CarpetPath path in active_path)
            {
                if (path_number == (int) path.color)
                {
                    path_to_erase = path;
                    break;
                }
            }
            int[] x_coord = path_to_erase.x_cordinates;
            int[] y_coord = path_to_erase.y_cordinates;
            //Starting to erase the path, forming the right string
            serialPort1.Open();
            for (int i = 0; i < path_to_erase.x_cordinates.Length; i++)
            {
                serialPort1.WriteLine(String.Format("{x}:{y}:black", x_coord[i], y_coord[i]));
            }
            //Remove path from the list
            active_path.Remove(path_to_erase);
        }
    }
}
