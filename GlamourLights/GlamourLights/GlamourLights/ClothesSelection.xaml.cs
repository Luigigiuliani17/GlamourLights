﻿using System;
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
                Itms.Add(new ItemImage(itm.itemId));
            }
            itms.ItemsSource = Itms;
        }

        /// <summary>
        /// this handles the click on the selection button after having selected an item 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!itms.SelectedItem.Equals(null))
            {
                ItemImage itemSelected = ((ItemImage)itms.SelectedItem);
                var Query = shopMan.shopState.shopDb.item.Where<item>(it => it.itemId == itemSelected.ItemId);
                item itemFound = (item)Query.FirstOrDefault<item>();
                int colInt = shopMan.getAvailableColor();
                CarpetColors col;
                if (colInt != -1)
                {
                    col = (CarpetColors)colInt;
                    //shopMan.executePathFinding(itemSelected, Loggedcust.customerId, col);
                    this.NavigationService.Navigate(new EndPage(col, shopMan, Loggedcust.customerId, itemFound));
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

    public class ItemImage
    {
        public int ItemId { get; set; }
        public BitmapImage Image { get; set; }

        public ItemImage(int id)
        {
            ItemId = id;
            Image = new BitmapImage(new Uri("https://yt3.ggpht.com/-UXFADeUgtQY/AAAAAAAAAAI/AAAAAAAAAAA/huGrZbZYaUM/s100-c-k-no/photo.jpg"));
        }
    }
}
