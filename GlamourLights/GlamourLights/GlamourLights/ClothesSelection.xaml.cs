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
using System.Threading;

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

        /// <summary>
        /// this handles the click on the selection button after having selected an item 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!itemDataGrid.SelectedItem.Equals(null))
            {
                item itemSelected = ((item)itemDataGrid.SelectedItem);
                int colInt = shopMan.getAvailableColor();
                CarpetColors col;
                if (colInt != -1)
                {
                    col = (CarpetColors)colInt;
                    shopMan.executePathFinding(itemSelected, Loggedcust.customerId, col);
                    this.NavigationService.Navigate(new EndPage(col, shopMan);
                }
                else
                {
                    //TODO need to pop something on the ui to show that we have run out of available colors
                    MessageBox.Show("All our path are current busy. Please wait a second!", "Oh that's awkard");
                }
            }
            
            


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
