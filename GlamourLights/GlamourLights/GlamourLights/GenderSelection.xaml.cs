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
    /// Logica di interazione per GenderSelection.xaml
    /// </summary>
    public partial class GenderSelection : Page
    {
        private ShopManager shopMan;
        private string gender;
        public GenderSelection(ShopManager shop)
        {
            this.shopMan = shop;
            InitializeComponent();
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void menButton_Click(object sender, RoutedEventArgs e)
        {
            gender = "male";
            this.NavigationService.Navigate(new DepartmentSelection(shopMan, gender));
        }

        private void womenButton_Click(object sender, RoutedEventArgs e)
        {
            gender = "female";
            this.NavigationService.Navigate(new DepartmentSelection(shopMan, gender));
        }
    }
}
