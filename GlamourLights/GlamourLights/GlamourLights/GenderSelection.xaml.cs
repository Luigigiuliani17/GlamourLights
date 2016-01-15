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
    /// Logica di interazione per GenderSelection.xaml
    /// </summary>
    public partial class GenderSelection : Page
    {
        private ShopManager shopMan;
        public customer LoggedCust { get; set; }

        public GenderSelection(ShopManager shop, customer cust)
        {
            LoggedCust = cust;
            this.shopMan = shop;
            InitializeComponent();
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "womenButton")
            {
                //if women is pressed i go to women page 
                this.NavigationService.Navigate(new DepartmentSelection(shopMan, "female", LoggedCust));
            }
            else
            {
                //if men is pressed i go to men page
                this.NavigationService.Navigate(new DepartmentSelection(shopMan, "male", LoggedCust));
            }
        }

    }
}
