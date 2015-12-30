﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    class CarpetPath
    {
        public CarpetColors color { get; set; }
        public int[] x_cordinates { get; set; }
        public int[] y_cordinates { get; set; }
        public int cost { get; set; }

        public CarpetPath()
        {

        }
        /// <summary>
        /// function that creates a new path 
        /// </summary>
        /// <param name="x_cordinates"></param> array of x_cordinates of the path
        /// <param name="y_cordinates"></param> array of y_cordinates of the path
        /// <param name="color"></param> color of the path
        public CarpetPath(int[] x_cordinates, int[] y_cordinates, CarpetColors color, int cost)
        {
            this.x_cordinates = x_cordinates;
            this.y_cordinates = y_cordinates;
            this.color = color;
            this.cost = cost;
        }

        /// <summary>
        /// append the coordinates of a new path to the path already present
        /// </summary>
        /// <param name="newPath"></param> path coordinates to append
        public void appendPath(CarpetPath newPath )
        {
            List<int> x_list = new List<int>();
            List<int> y_list = new List<int>();
            for(int i=0; i<x_cordinates.Length; i++)
            {
                x_list.Add(x_cordinates[i]);
                y_list.Add(y_cordinates[i]);
            }

            //this for starts from 1 to avoid the repetition of the termination point
            for(int i=1; i<newPath.x_cordinates.Length; i++)
            {
                x_list.Add(newPath.x_cordinates[i]);
                y_list.Add(newPath.y_cordinates[i]);
            }

            
            cost = cost + newPath.cost;
            x_cordinates = x_list.ToArray();
            y_cordinates = y_list.ToArray();
        }
    }
}
