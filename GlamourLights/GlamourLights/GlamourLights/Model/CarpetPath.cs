using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlamourLights.Model
{
    public class CarpetPath
    {
        public CarpetColors color { get; set; }

        //arrays of x and y coordinates of the path
        public int[] x_cordinates { get; set; }
        public int[] y_cordinates { get; set; }
        public int cost { get; set; }

        //codes of the lights and of the destination
        public int[] lightsCodes { get; set; } 
        public int destination_light_code { get; set; }

        //list of coordinates of the recommendations
        public int[] x_recommendations { get; set; }
        public int[] y_recommendations { get; set; }

        //max num of recommendations
        public const int NUM_REC_MAX = 2;

        public CarpetPath()
        {

        }
        /// <summary>
        /// function that creates a new path (without lights!!!!!!!!!!)
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
            this.lightsCodes = new int[NUM_REC_MAX];
            this.lightsCodes[0] = -1;
            this.lightsCodes[1] = -1;
            x_recommendations = new int[NUM_REC_MAX];
            y_recommendations = new int[NUM_REC_MAX];
            for(int i=0; i<NUM_REC_MAX; i++)
            {
                x_recommendations[i] = -1;
                y_recommendations[i] = -1;
            }
        }

        /// <summary>
        /// ///OVERLOADING/// function that creates a path with up to 2 light codes
        /// </summary>
        /// <param name="x_cordinates"></param> array of x_cordinates of the path
        /// <param name="y_cordinates"></param> array of y_cordinates of the path
        /// <param name="color"></param> color of the path
        /// <param name="cost"></param> cost of the path
        /// <param name="lightsCodes"></param> codes of the lights to be switched on
        public CarpetPath(int[] x_cordinates, int[] y_cordinates, CarpetColors color, int cost, int[] lightsCodes)
        {
            this.x_cordinates = x_cordinates;
            this.y_cordinates = y_cordinates;
            this.color = color;
            this.cost = cost;
            this.lightsCodes = new int[NUM_REC_MAX];
            this.lightsCodes[0] = lightsCodes[0];
            this.lightsCodes[1] = lightsCodes[1];
        }

        /// <summary>
        /// append the coordinates of a new path to the path already present. The color remains the one of the old path
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
