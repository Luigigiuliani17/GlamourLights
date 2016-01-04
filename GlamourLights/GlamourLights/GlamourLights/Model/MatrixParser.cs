using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace GlamourLights.Model
{
    class MatrixParser
    {
        public int[,]  parseMatrix()
        {
            int[,] shop_layout = null;
            using (StreamReader sr = new StreamReader("../../Resources/shop_layout.txt"))
            {
                int x_pos;
                int y_pos;
                int type;

                string[] parameters;
                string[] firstLine;
                string currentLine;

                firstLine = sr.ReadLine().Split(';');
                shop_layout = new int[Int32.Parse(firstLine[0]),Int32.Parse(firstLine[1])];
      
                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {
                    parameters = currentLine.Split(';');
                    x_pos = Int32.Parse(parameters[0]);
                    y_pos = Int32.Parse(parameters[1]);
                    type = Int32.Parse(parameters[2]);
                    shop_layout[x_pos, y_pos] = type;
                }
            }
            return shop_layout;
        }

        /// <summary>
        /// parse the shelves_position txt. PATTERN: shelfId;x_pos;y_pos
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> parseShelvesPosition()
        {
            Dictionary<int, string> shelves_pos = new Dictionary<int, string>(); 
            using (StreamReader sr = new StreamReader("../../Resources/shelves_position.txt"))
            {
                int x_pos;
                int y_pos;
                int shelfId;

                string[] parameters;
                string currentLine;

                while ((currentLine = sr.ReadLine()) != null)
                {
                    parameters = currentLine.Split(';');
                    shelfId = Int32.Parse(parameters[0]);
                    x_pos = Int32.Parse(parameters[1]);
                    y_pos = Int32.Parse(parameters[2]);
                    shelves_pos.Add(shelfId, x_pos + ";" + y_pos);
                    
                }
            }
            return shelves_pos;
        }

        public Dictionary<int, string> parseDepartmentPosition()
        {
            Dictionary<int, string> department_pos = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader("../../Resources/department_position.txt"))
            {
                int x_pos;
                int y_pos;
                int departmentId;

                string[] parameters;
                string currentLine;

                while ((currentLine = sr.ReadLine()) != null)
                {
                    parameters = currentLine.Split(';');
                    departmentId = Int32.Parse(parameters[0]);
                    x_pos = Int32.Parse(parameters[1]);
                    y_pos = Int32.Parse(parameters[2]);
                    department_pos.Add(departmentId, x_pos + ";" + y_pos);

                }
            }
            return department_pos;
        }

        /// <summary>
        /// parser of the light position-----> pattern   x_pos;y_pos;lighId   (x_pos;y_pos is the string key of the dictionary)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> parseLightsPositions()
        {
            Dictionary<string, int> lights_pos = new Dictionary<string, int>();
            using (StreamReader sr = new StreamReader("../../Resources/lights_position.txt"))
            {
                int x_pos;
                int y_pos;
                int lightId;

                string[] parameters;
                string currentLine;

                while ((currentLine = sr.ReadLine()) != null)
                {
                    parameters = currentLine.Split(';');
                    x_pos = Int32.Parse(parameters[0]);
                    y_pos = Int32.Parse(parameters[1]);
                    lightId = Int32.Parse(parameters[2]);
                    lights_pos.Add(x_pos + ";" + y_pos, lightId);


                }
            }
            return lights_pos;
        }

        /// <summary>
        /// rebuild the txt of the shop layout
        /// </summary>
        /// <param name="lines"></param> 
        internal void uploadTxt(List<string> lines, string path)
        {
            using (StreamWriter sr = new StreamWriter(path))
            {
                foreach(string s in lines)
                {
                    sr.WriteLine(s);
                }
            }
        }
    }
}
