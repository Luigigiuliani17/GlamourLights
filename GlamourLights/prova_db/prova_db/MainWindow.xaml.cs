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

namespace prova_db
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private System.Windows.Data.CollectionViewSource customersViewSource;
        private prova_db.Database1Entities adventureWorksLTEntities;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            adventureWorksLTEntities = new prova_db.Database1Entities();
            // Load data into Customers. You can modify this code as needed.
            customersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customersViewSource")));
            System.Data.Objects.ObjectQuery<prova_db.Customers> customersQuery = this.GetCustomersQuery(adventureWorksLTEntities);
            customersViewSource.Source = customersQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }
            }
}
