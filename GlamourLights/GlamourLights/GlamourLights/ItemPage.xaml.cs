using GlamourLights.Controller;
using GlamourLights.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per ItemPage.xaml
    /// </summary>
    public partial class ItemPage : Page
    {
        public customer LoggeedCust { get; set; }
        public ShopManager ShopMan { get; set; }
        public item itemSelected { get; set; }

        public ItemPage()
        {
            InitializeComponent();
        }
        
        public ItemPage(ItemImage itm, customer cust, ShopManager shopMan) : this()
        {
            img.Source = itm.Image;
            LoggeedCust = cust;
            var Query = shopMan.shopState.shopDb.item.Where<item>(it => it.itemId == itm.ItemId);
            itemSelected = Query.FirstOrDefault<item>();
            nameBlock.Text = itemSelected.name.ToUpper();
            fabricBlock.Text = "FABRIC: " + itemSelected.fabric ;
            descriptionBlock.Text = itemSelected.description;
            ShopMan = shopMan;
        }

        /// <summary>
        /// Pretty self explained
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bacKButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// check wheter there is a color available and if so, it send all the stuff needed to EndPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            int colInt = ShopMan.getAvailableColor();
            CarpetColors col;
            if (colInt != -1)
            {
                col = (CarpetColors)colInt;
                this.NavigationService.Navigate(new EndPage(col, ShopMan, LoggeedCust.customerId, itemSelected));
            }
            else
            {
                Thread myThread = new Thread(() => MessageBox.Show("All our paths are currently busy, Please try in a while", "Ooops...."));
                myThread.Start();
            }
        }
    }
}
