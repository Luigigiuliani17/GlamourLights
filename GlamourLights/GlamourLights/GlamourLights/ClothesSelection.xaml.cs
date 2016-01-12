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
        public List <ItemImage> Itms { get; set; }
        public ClothesSelection(ShopManager sm, customer cust)
        {
            InitializeComponent();
            shopMan = sm;
            Loggedcust = cust;
            shopMan.shopState.shopDb.item.Load();
            //itemDataGrid.ItemsSource = shopMan.shopState.shopDb.item.Local;
            Itms = new List<ItemImage>();
            foreach (item itm in shopMan.shopState.shopDb.item.Local)
            {
                Itms.Add(new ItemImage(itm.itemId, itm.name, itm.fabric));
            }
            itms.ItemsSource = Itms;
        }
               

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// Simulate the mouse click. When the mouse button (the left one) is released, it'll pick the item selected
        /// and pass the control to ItemPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itms_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ItemImage itemSelected = (ItemImage)itms.SelectedItem;
            this.NavigationService.Navigate(new ItemPage(itemSelected, Loggedcust, shopMan));
        }
    }

    /// <summary>
    /// This is kinda helper Class to have the stuff I need most in the listBox
    /// </summary>
    public class ItemImage
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemFabric { get; set; }
        public BitmapImage Image { get; set; }


        public ItemImage(int id, string name, string fabric)
        {
            ItemId = id;
            Uri uri = new Uri(@"Resources\Images\Clothes\img_" + id + ".jpg", UriKind.Relative);
            Image = new BitmapImage(uri);
            ItemName = name;
            ItemFabric = fabric;
        }
    }
}
