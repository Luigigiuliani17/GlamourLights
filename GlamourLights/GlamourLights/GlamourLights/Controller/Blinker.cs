using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlamourLights.Model;

namespace GlamourLights.Controller

{
    class Blinker
    {
        List<string> red = new List<string>();
        List<string> green = new List<string>();
        List<string> blue = new List<string>();
        List<string> yellow = new List<string>();
        bool allEmpty = true;
        int n_overlapping = 0;
        ShopState state;

        public Blinker(ShopState state)
        {
            this.state = state;
        }

        public bool CheckOverlapping()
        {
            return false;
        }

        private void StartBlink()
        {

        }
    }
}
