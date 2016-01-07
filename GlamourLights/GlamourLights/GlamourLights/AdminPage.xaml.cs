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
using GlamourLights.Model;
using System.Data.Entity;
using GlamourLights.Controller;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private ShopManager shopMan;
        public AdminPage()
        {
            InitializeComponent();
        }
        public AdminPage(ShopManager shopMan)
        {
            this.shopMan = shopMan;
            shopMan.shopState.shopDb.customer.Load();
            InitializeComponent();
            customerGrid.ItemsSource = shopMan.shopState.shopDb.customer.Local;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            customer cust = new customer();
            cust.firstName = firstNameBox.Text;
            cust.lastName = lastNameBox.Text;
            cust.cardNumber = cardNumBox.Text;
            shopMan.shopState.shopDb.customer.Add(cust);
            shopMan.shopState.shopDb.SaveChanges();
            firstNameBox.Clear();
            lastNameBox.Clear();
            cardNumBox.Clear();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            firstNameBox.Clear();
            lastNameBox.Clear();
            cardNumBox.Clear();
            this.NavigationService.GoBack();
        }

        private void matrixButton_Click(object sender, RoutedEventArgs e)
        {
            shopMan.com.InitializeMatrix();
        }

        private void matrixDefButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Customization(shopMan));
        }
    }
}
