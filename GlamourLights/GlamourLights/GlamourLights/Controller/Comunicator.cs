using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    rapresent the shelves of the shop
    */
    class Comunicator
    {

    }
}
