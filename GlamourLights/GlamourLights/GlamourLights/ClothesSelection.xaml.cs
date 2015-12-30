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
using System.Data.Entity;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per ClothesSelection.xaml
    /// </summary>
    public partial class ClothesSelection : Page
    {
        private ShopManager shopMan;
        private customer Loggedcust;
        public ClothesSelection(ShopManager sm, customer cust)
        {
            InitializeComponent();
            shopMan = sm;
            Loggedcust = cust;
            shopMan.shopState.shopDb.item.Load();
            itemDataGrid.ItemsSource = shopMan.shopState.shopDb.item.Local;
        }
    }
}
