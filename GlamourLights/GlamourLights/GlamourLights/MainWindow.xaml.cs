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
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ShopDb myDb = new ShopDb();
        public MainWindow()
        {
            InitializeComponent();
            myDb.customer.Load();
            dataGrid1.ItemsSource = myDb.customer.Local;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            customer cust = new customer();
            cust.firstName = textBox0.Text;
            cust.lastName = textBox1.Text;
            cust.cardNumber = textBox2.Text;
            myDb.customer.Add(cust);
            myDb.SaveChanges();
        }

    }
}
