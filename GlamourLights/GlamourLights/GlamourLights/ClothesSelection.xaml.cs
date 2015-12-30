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

        private void selectionButton_Click(object sender, RoutedEventArgs e)
        {
            //block.Text = itemDataGrid.SelectedItem.ToString();
            item itemSelected = ((item)itemDataGrid.SelectedItem);
            String[] parameters;
            int xrec1, yrec1;
            parameters = shopMan.shopState.shelves_position[itemSelected.shelf1.shelfId].Split(';');
            xrec1 = Int32.Parse(parameters[0]);
            yrec1 = Int32.Parse(parameters[1]);
            CarpetColors col = new CarpetColors();
            if (shopMan.shopState.active_colors[(int)CarpetColors.blue])
            {
                col = CarpetColors.green;
            }
            else
            {
                col = CarpetColors.blue;
            }
            CarpetPath path = shopMan.calculateSubPath(2, 1, xrec1, yrec1, col);
            Thread myFred = new Thread(() => shopMan.com.DrawPath(path));
            myFred.Start();
            //shopMan.com.DrawPath(path);


        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
