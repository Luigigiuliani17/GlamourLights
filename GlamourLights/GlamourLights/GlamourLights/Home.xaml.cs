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

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        ShopDb shop;
        public Home()
        {
            InitializeComponent();
        }
        public Home(ShopDb shop)
        {
            InitializeComponent();
            this.shop = shop;
            textBox.Focus();
            textBox.Clear();
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (textBox.Text == "admin")
                {
                    textBox.Clear();
                    this.NavigationService.Navigate(new AdminPage(shop));
                }
                else
                {
                    Console.WriteLine(textBox.Text);
                    shop.customer.Load();
                    String cardNum = textBox.Text;
                    //customer cust = shop.customer.SqlQuery("SELECT * FROM customer WHERE cardNumber = @p0", cardNum).First<customer>();
                    var L2EQuery = shop.customer.Where(c => c.cardNumber.Equals(cardNum));
                    var cust = L2EQuery.FirstOrDefault<customer>();
                    textBox.Clear();
                    this.NavigationService.Navigate(new SelectionPage(shop, cust));
                }
            }
        }
    }
}
