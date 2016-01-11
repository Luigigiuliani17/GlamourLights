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



namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per Customization.xaml
    /// </summary>
    public partial class Customization : Page
    {
        public ShopManager shopManager { get; set; }
        public Customization()
        {

        }

        public Customization(ShopManager sm)
        {
            InitializeComponent();
            this.shopManager = sm;
            initializeMatrix(sm.shopState);
        }

        private void initializeMatrix(ShopState st)
        {
            //set num rows and column of button matrix
            int numRows = st.shop_layout_matrix.GetLength(0);
            int numCols = st.shop_layout_matrix.GetLength(1);

            //add correct number of rows and columns
            for (int i = 0; i < numCols; i++)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < numRows; j++)
            {
                buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }


            matrixButton[,] buttonMatrix = new matrixButton[numRows, numCols];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    buttonMatrix[i, j] = new matrixButton(i, j, st.shop_layout_matrix[i, j]);



                    buttonGrid.Children.Add(buttonMatrix[i, j]);

                    updateButtonColor(buttonMatrix[i, j]);

                    Grid.SetRow(buttonMatrix[i, j], j);
                    Grid.SetColumn(buttonMatrix[i, j], i);
                }
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    buttonMatrix[i, j].Click += matrixButtonClick;
                }
            }
        }

        private void matrixButtonClick(object sender, RoutedEventArgs e)
        {
            
            if (sender is matrixButton)
            {
                
                matrixButton b = sender as matrixButton;
                //call function to change it
                if(b.kind == 1 || b.kind == 0)
                {
                    shopManager.changeKind(b.x_cord, b.y_cord);
                    if (b.kind == 1)
                    {
                        Console.WriteLine("1 diventa 0");
                        b.kind = 0;
                        b.Background = new SolidColorBrush(Colors.Violet);
                        return;

                    }
                    else
                    {
                        Console.WriteLine("0 diventa 1");
                        b.kind = 1;
                        b.Background = new SolidColorBrush(Colors.White);
                        return;
                    }
                   
                }

            }
        }

        /// <summary>
        /// updates the color of a single button (after update or at inizialization)
        /// </summary>
        /// <param name="b"></param>
        private void updateButtonColor(matrixButton b)
        {
            
            switch ((int)b.kind)
            {
                
                case -1:
                    b.Background = new SolidColorBrush(Colors.Red);
                    break;
                case 0:
                    b.Background = new SolidColorBrush(Colors.Violet);
                    break;
                case 1:
                    b.Background = new SolidColorBrush(Colors.White);
                    break;
                case 2:
                    b.Background = new SolidColorBrush(Colors.PapayaWhip);
                    break;
                    
            }
            
        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            shopManager.saveChanges();
        }
    }
}
