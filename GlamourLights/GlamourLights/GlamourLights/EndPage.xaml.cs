using GlamourLights.Controller;
using GlamourLights.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per EndPage.xaml
    /// </summary>
    public partial class EndPage : Page
    {
        public EndPage()
        {
            InitializeComponent();
        }

        public EndPage(CarpetColors col, ShopManager sm)
        {
            InitializeComponent();
            switch (col)
            {
                case CarpetColors.blue:
                    textBlock.Text = "Your color is BLUE," + "\n" + "Follow it!";
                    break;
                case CarpetColors.red:
                    textBlock.Text = "Your color is RED," + "\n" + "Follow it!";
                    break;
                case CarpetColors.green:
                    textBlock.Text = "Your color is GREEN," + "\n" + "Follow it!";
                    break;
                case CarpetColors.yellow:
                    textBlock.Text = "Your color is GREEN," + "\n" + "Follow it!";
                    break;
                default:
                    textBlock.Text = "something's is wrong! UHUHUHUHUH";
                    break;
            }
            Thread.Sleep(5000);
            this.NavigationService.Navigate(new Home(sm));
        }
    }
}
