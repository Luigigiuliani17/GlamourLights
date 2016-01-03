﻿using System;
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
    public class Comunicator
    {
        ShopState state;
        List<CarpetPath> active_path;
        SerialPort serial = new SerialPort();
        Blinker blink;
        DateTime start, stop;
        
        public Comunicator(ShopState shop)
        {
            this.state = shop;
            this.blink = new Blinker(shop);
            serial.PortName = "COM4";
            serial.BaudRate = 9600;
            serial.Open();
            Console.WriteLine("inizializzato la porta");

        }

        /// <summary>
        /// This function will take the matrix structure in the ShopState, looping on it recognizing the walls = 4
        /// (identified with -1 in txt file) and the shelves = 5 (identified with 0 in txt file), put them in string form and comunicate them
        /// to the arduino board via serial communication.
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
                            serial.WriteLine(i + ":" + j + ":" + "5"); //walls
                        }
                        if (matrix[i, j] == 0)
                        {
                            serial.WriteLine(i + ":" + j + ":" + "6"); //shelves 
                        }
                    }
                }
            }
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
            int[] x_coord = path.x_cordinates;
            int[] y_coord = path.y_cordinates;
            //Retriving the color name
            int path_id = (int)path.color;
            int color = path_id + 1;
            //update of vertex colors
            this.UpdateColorVertex(path);
            //Setting the correct things in the shop state
            state.active_colors[path_id] = true;
            state.active_path.Add(path);
            //Adding to the path cost 5
            for(int i=0; i<x_coord.Length; i++)
            {
                state.shop_graph[x_coord[i] + ";" + y_coord[i]].cost += 5;
            }
            //Send a string for every coordinate, plus the color
            for(int i=0; i<path.x_cordinates.Length; i++)
            {
                if (serial.IsOpen)
                {
                    serial.WriteLine(x_coord[i] + ":" + y_coord[i] + ":" + color);
                    Thread.Sleep(200);
                }
            }
            //Check if there is overlapping, if yes the method to blink the matrix is fired in another thread
            //But only of is not fired yet
            if (blink.CheckOverlapping(path) && blink.blink == false)
            {
                new Thread(delegate ()
                {
                    blink.StartBlink(serial);
                }).Start();
            }
            //Timer part, in wich we bind the number of path to send to the handler, setting the time to wait 30 seconds
            var timer = new System.Timers.Timer { Interval = 20000, AutoReset = false };
            timer.Elapsed += (sender, e) => CallErasePath(sender, e, path_id);
            timer.Start();
            start = DateTime.Now;
            Console.WriteLine("starting at: " + start);
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
            new Thread(delegate () {
                this.ErasePath(path_id);
            }).Start();
        }

        /// <summary>
        /// This method is fired by the event occurring when the timer elapse
        /// This method will form the right string to be passed to the serial port to switch off the lights
        /// </summary>
        /// <param name="path_id"></param>
        private void ErasePath(int path_id)
        {
            stop = DateTime.Now;
            Console.WriteLine("Finishing at: " + stop);
            TimeSpan time = stop - start;
            Console.WriteLine("total elapse time: " + time);
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
            if (serial.IsOpen)
            {
                for (int i = 0; i < path_to_erase.x_cordinates.Length; i++)
                    serial.WriteLine(x_coord[i] + ":" + y_coord[i] + ":" + "-1");

            }
            //Set the accupation of the specific color to false again
            //Erasing a path from the list 
            state.active_colors[path_id] = false;
            state.active_path.Remove(path_to_erase);
            //Update blinker: cheking if the path is overlapping with another
            blink.UpdateBlinker(path_to_erase);
            //Lowering the path cost by 5
            for (int i = 0; i < x_coord.Length; i++)
            {
                state.shop_graph[x_coord[i] + ";" + y_coord[i]].cost -= 5;
            }
        }

        /// <summary>
        /// This function will update the colors present in a specific vertex of the carpet
        /// </summary>
        /// <param name="path"></param>
        private void UpdateColorVertex(CarpetPath path)
        {
            Dictionary<string, Graphvertex> graph = state.shop_graph;
            int[] x = path.x_cordinates;
            int[] y = path.y_cordinates;
            string[] coord = new string[x.Length];
            int color_code = (int)path.color;

            //this will create the array with the keys for the dictionary
            for(int i=0; i<x.Length; i++)
                coord[i] = x[i] + ";" + y[i];

            //Loop through the dictionary to update colors of vertex and the number of active colors
            for (int i = 0; i < coord.Length; i++)
            {
                graph[coord[i]].active_colors[color_code] = true;
                graph[coord[i]].n_activeColors += 1;
            }
        }

    }
}
