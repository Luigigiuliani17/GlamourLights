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
        Comunicator com;
        ShopManager sm;
        public Home()
        {
            InitializeComponent();
        }
        public Home(ShopManager shopM)
        {
            InitializeComponent();
            //this.shop = shop;
            textBox.Focus();
            textBox.Clear();
            sm = shopM;
            com = sm.com;
            // st = new ShopState();
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (textBox.Text == "admin")
                {
                    textBox.Clear();
                    this.NavigationService.Navigate(new AdminPage(sm.shopState.shopDb));
                }
                else
                {
                    Console.WriteLine(textBox.Text);
                    sm.shopState.shopDb.customer.Load();
                    String cardNum = textBox.Text;
                    //customer cust = shop.customer.SqlQuery("SELECT * FROM customer WHERE cardNumber = @p0", cardNum).First<customer>();
                    var L2EQuery = sm.shopState.shopDb.customer.Where(c => c.cardNumber.Equals(cardNum));
                    var cust = L2EQuery.FirstOrDefault<customer>();
                    textBox.Clear();
                    this.NavigationService.Navigate(new SelectionPage(sm, cust));
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            com.InitializeMatrix();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //int[] x = { 10, 10, 10, 10, 10, 10, 11, 12, 13, 14, 15, 14, 13 };
            //int[] y = { 0, 1, 2, 3, 4, 5, 5, 5, 5, 5, 5, 6, 6 };
            CarpetColors col = CarpetColors.blue;
            //CarpetPath path = new CarpetPath(x,y,col,1);
            CarpetPath path = sm.calculateSubPath(3, 1, 62, 30, col);
            com.DrawPath(path);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
