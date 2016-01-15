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
            Itms = new List<ItemImage>();
            foreach (item itm in shopMan.shopState.shopDb.item.Local.OrderBy(it => it.name))
            {
                Itms.Add(new ItemImage(itm.itemId, itm.name, itm.gender));
            }
            itms.ItemsSource = Itms;
            //Done here and not in XAML, beacause otherwise it will pop out NullReferenceException
            AllRd.IsChecked = true;
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

        /// <summary>
        /// Handle the event of a Selection on the RadioButton Gruop
        /// Change the items showed in ListBox, default behavior is to show all the items, then if MaleRd or FemaleRd
        /// are pressed, then it changes the item showing only male items and female items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            if (AllRd.IsChecked == true) //not able to do btn.IsChecked 'coz the property is bool?
            {
                //if the default one is checked all the clothes are shown
                itms.ItemsSource = Itms;
            }
            else
            {
                if(MaleRd.IsChecked == true)
                {
                    //if the selected one is male clothes
                    itms.ItemsSource = Itms.Where<ItemImage>(it => it.ItemGender.ToLower() == "male").ToList<ItemImage>();
                }
                else
                {
                    //if none the above then the one pressed is female
                    itms.ItemsSource = Itms.Where<ItemImage>(it => it.ItemGender.ToLower() == "female").ToList<ItemImage>();
                }
            }
        }
    }

    /// <summary>
    /// This class containd the useful info about an item, the one needed to be displayed and the one useful in the
    /// page, such as the name, the id, the gender and finally it loads up the image associated to an item
    /// </summary>
    public class ItemImage
    {
        
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public BitmapImage Image { get; set; }
        public string ItemGender { get; set; }


        public ItemImage(int id, string name, string gender)
        {
            ItemId = id;
            Uri uri = new Uri(@"Resources\Images\Clothes\img_" + id + ".jpg", UriKind.Relative);
            Image = new BitmapImage(uri);
            ItemName = name;
            ItemGender = gender;
        }
    }
}
