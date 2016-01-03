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
using GlamourLights.Model;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per SelectionPage.xaml
    /// </summary>
    public partial class SelectionPage : Page
    {
        private ShopManager shopMan;
        private customer LoggedCust { set; get; }
        public SelectionPage()
        {
            InitializeComponent();
        }

        public SelectionPage(ShopManager shopMan, customer loggedCust)
        {
            this.shopMan = shopMan;
            LoggedCust = loggedCust;
            InitializeComponent();
            if (loggedCust.customerId == -1)
            {
                welcomeBlock.Text = "Hello Customer!";
            }
            else
            {
                welcomeBlock.Text = "Welcome Back, " + LoggedCust.firstName.ToUpper() + " " + LoggedCust.lastName.ToUpper() + "!";
            }
        }

        private void ClothesButton_Click(object sender, RoutedEventArgs e)
        {
            clothesButton.Content = "hello";
            this.NavigationService.Navigate(new ClothesSelection(shopMan, LoggedCust));
        }

        private void DepButton_Click(object sender, RoutedEventArgs e)
        {
            depButton.Content = "hi!";
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
