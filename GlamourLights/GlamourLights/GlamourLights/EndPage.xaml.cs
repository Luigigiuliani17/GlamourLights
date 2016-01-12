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
using System.Timers;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per EndPage.xaml ==> this one doesn't do to much, it only excutes the pathfinding
    /// and nothing more
    /// </summary>
    public partial class EndPage : Page
    {
        private int customerId;
        private item itemSelected;
        private ShopManager shopMan;
        private CarpetColors col;

        public EndPage()
        {
            InitializeComponent();
        }
        public EndPage(CarpetColors col, ShopManager sm, int customerId, item itemSelected)
        {
            InitializeComponent();
            this.customerId = customerId;
            this.itemSelected = itemSelected;
            this.col = col;
            this.shopMan = sm;
            switch (col)
            {
                case CarpetColors.blue:
                    endBlock.Text = "Your color is. BLUE Follow it!";
                    break;
                case CarpetColors.green:
                    endBlock.Text = "Your color is. GREEN Follow it!";
                    break;
                case CarpetColors.red:
                    endBlock.Text = "Your color is RED. Follow it!";
                    break;
                case CarpetColors.yellow:
                    endBlock.Text = "Your color is YELLOW. Follow it!";
                    break;
                default:
                    endBlock.Text = "Something went wrong, Retry!";
                    break;
            }

        }

        /// <summary>
        /// redirect to the home page, probably not the best code, but it works, who cares!
        /// </summary>
        private void Redirect()
        {
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.GoBack();

        }

        /// <summary>
        /// This one calls the excute pathfinding and then return to the homePage.
        /// i can't instantiate a new Home because i need to have the the right shopMAnager
        /// that exctues the paths on the carpet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            shopMan.executePathFinding(itemSelected, customerId, col);
            Redirect();
        }
    }
}

