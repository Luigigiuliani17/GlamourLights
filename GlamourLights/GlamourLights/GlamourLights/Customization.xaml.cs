using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GlamourLights.Controller;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per Customization.xaml
    /// </summary>
    public partial class Customization : Page
    {
        public ShopManager shopManager { get; set; }
        public Customization()
        {
            InitializeComponent();
        }

        public Customization(ShopManager sm)
        {
            this.shopManager = sm;
        }
    }
}
