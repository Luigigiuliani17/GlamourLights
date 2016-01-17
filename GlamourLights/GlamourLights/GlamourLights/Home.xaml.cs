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
using System.Data.Entity;
using GlamourLights.Model;
using GlamourLights.Controller;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        ShopManager sm;
        public Home()
        {
            InitializeComponent();
        }
        public Home(ShopManager shopM)
        {
            InitializeComponent();
            cardNumBox.Focus();
            cardNumBox.Clear();
            sm = shopM;
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (cardNumBox.Text.ToLower() == "admin")
                {
                    cardNumBox.Clear();
                    this.NavigationService.Navigate(new AdminPage(sm));
                }
                else
                {
                    Console.WriteLine(cardNumBox.Text);
                    sm.shopState.shopDb.customer.Load();
                    String cardNum = cardNumBox.Text;
                    var Query = sm.shopState.shopDb.customer.Where(c => c.cardNumber.Equals(cardNum));
                    var cust = Query.FirstOrDefault<customer>();
                    cardNumBox.Clear();
                    if (cust != null)
                    {
                        this.NavigationService.Navigate(new SelectionPage(sm, cust));
                    }
                }
            }
            
        }

        /// <summary>
        /// TODO Decide how to handle this. the most likely situation is to pass to selection page a mock customer 
        /// with special id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noCardButton_Click(object sender, RoutedEventArgs e)
        {
            customer mockCust = new customer();
            mockCust.customerId = -1;
            mockCust.firstName = "Mock";
            mockCust.lastName = "Customer";
            mockCust.cardNumber = "0000";
            cardNumBox.Clear();
            this.NavigationService.Navigate(new SelectionPage(sm, mockCust));
        }
    }
}
