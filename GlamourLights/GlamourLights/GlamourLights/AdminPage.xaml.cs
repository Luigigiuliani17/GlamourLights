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

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private ShopDb shop;
        public AdminPage()
        {
            InitializeComponent();
        }
        public AdminPage(ShopDb shop)
        {
            this.shop = shop;
            shop.customer.Load();
            InitializeComponent();
            customerGrid.ItemsSource = shop.customer.Local;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            customer cust = new customer();
            cust.firstName = firstNameBox.Text;
            cust.lastName = lastNameBox.Text;
            cust.cardNumber = cardNumBox.Text;
            shop.customer.Add(cust);
            shop.SaveChanges();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            firstNameBox.Clear();
            lastNameBox.Clear();
            cardNumBox.Clear();
            this.NavigationService.GoBack();
        }
    }
}
