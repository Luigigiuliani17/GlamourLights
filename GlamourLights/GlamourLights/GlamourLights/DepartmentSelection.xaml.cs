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
    /// Logica di interazione per DepartmentSelection.xaml
    /// </summary>
    public partial class DepartmentSelection : Page
    {
        ShopManager shopMan;
        string gender;
        private DepImage depSelected;
        public List<DepImage> Dep { get; set; }
        public DepartmentSelection(ShopManager shop, string gender)
        {
            this.shopMan = shop;
            this.gender = gender;
            InitializeComponent();
            shopMan.shopState.shopDb.department.Load();
            //itemDataGrid.ItemsSource = shopMan.shopState.shopDb.item.Local;
            Dep = new List<DepImage>();
            foreach (department d in shopMan.shopState.shopDb.department.Where<department>(it => it.genderType == gender))
            {
                Dep.Add(new DepImage(d.departmentId, d.departmentName));
            }
            itms.ItemsSource = Dep;
        }

        /// <summary>
        /// Go back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        /// <summary>
        /// This will select the right department and will set it globally
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itms_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DepImage depSelected = (DepImage)itms.SelectedItem;
        }

        /// <summary>
        /// This will fire the method to arrive to the right department
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guideButton_Click(object sender, RoutedEventArgs e)
        {
            int colInt = shopMan.getAvailableColor();
            CarpetColors col;
            if (colInt != -1)
            {
                col = (CarpetColors)colInt;
                this.NavigationService.Navigate(new EndPage(col, shopMan, 1, depSelected.DepId));
            }
            else
            {
                Thread myThread = new Thread(() => MessageBox.Show("All our paths are currently busy, Please try in a while", "Ooops...."));
                myThread.Start();
            }
            //Chiama EndPage con un parametro
        }

        /// <summary>
        /// Support class to retrieve all the information to display the department info
        /// </summary>
        public class DepImage
        {
            public int DepId { get; set; }
            public string DepName { get; set; }
            public BitmapImage Image { get; set; }


            public DepImage(int id, string name)
            {
                DepId = id;
                Uri uri = new Uri(@"Resources\Images\Departments\img_" + id + ".jpg", UriKind.Relative);
                Image = new BitmapImage(uri);
                DepName = name;
            }
        }
    }
}
