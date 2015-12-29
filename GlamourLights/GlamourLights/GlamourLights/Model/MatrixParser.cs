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
    }
}
