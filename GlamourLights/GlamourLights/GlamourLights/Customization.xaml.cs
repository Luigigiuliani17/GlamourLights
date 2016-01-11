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
    public enum modeList
    {
        shop_layout = 0,
        shelves_position = 1,
        department_position =2
    }

    /// <summary>
    /// Logica di interazione per Customization.xaml
    /// </summary>
    public partial class Customization : Page
    {
        public ShopManager shopManager { get; set; }
        public List<int> shelves_list { get; set; }
        public modeList active_mode { get; set; }
        public int shelf_selected {get; set; }
        public matrixButton[,] buttonMatrix;
        public Customization()
        {

        }

        public Customization(ShopManager sm)
        {
            InitializeComponent();
            //give an initial value to shelf_selected
            shelf_selected = -1;
            this.shopManager = sm;
            shelves_list = new List<int>();
            foreach(int k in shopManager.shopState.shelves_position.Keys)
            {
                shelves_list.Add(k);
            }
            department_List.ItemsSource = shelves_list;
            active_mode = modeList.shop_layout;
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


            buttonMatrix = new matrixButton[numRows, numCols];

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
                if (active_mode == modeList.shop_layout)
                {
                    executeClickShopLayout(b);
                    return;
                }
                if(active_mode == modeList.shelves_position)
                {
                 
                }
            }
        }


        /// <summary>
        /// it changes the type on the shop layout matrix from shelf to free tor viceversa
        /// </summary>
        /// <param name="b"></param>
        public void executeClickShopLayout (matrixButton b)
        {
            if (b.kind == 1 || b.kind == 0)
            {
                shopManager.changeKind(b.x_cord, b.y_cord);
                if (b.kind == 1)
                {
                    Console.WriteLine("1 diventa 0");
                    b.kind = 0;
                    updateButtonColor(b); 
                }
                else
                {
                    Console.WriteLine("0 diventa 1");
                    b.kind = 1;
                    updateButtonColor(b);
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

        private void department_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if not correct mode, just return
            if (active_mode != modeList.shelves_position)
                return;

            //delete old color of old_shelf_selected
            String[] parameters;
            int x, y;
            if(shelf_selected != -1)
            {
                parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                x = Int32.Parse(parameters[0]);
                y = Int32.Parse(parameters[1]);

                buttonMatrix[x, y].Background = new SolidColorBrush(Colors.White);
            }

            //set new shelf_selected
            shelf_selected = (int)department_List.SelectedItem;
            parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
            x = Int32.Parse(parameters[0]);
            y = Int32.Parse(parameters[1]);
            
            buttonMatrix[x,y].Background = new SolidColorBrush(Colors.ForestGreen);
        }

        private void changeMode_Click(object sender, RoutedEventArgs e)
        {
            if (active_mode == modeList.shop_layout)
            {
                changeMode.Content = "Modalità shelves";
                active_mode = modeList.shelves_position;
                String[] parameters;
                int x, y;
                if (shelf_selected != -1)
                {
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = new SolidColorBrush(Colors.Green);
                }
                return;
            }
            if (active_mode == modeList.shelves_position)
            {
                changeMode.Content = "Modalità shop_layout";
                active_mode = modeList.shop_layout;
                String[] parameters;
                int x, y;
                if (shelf_selected != -1)
                {
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = new SolidColorBrush(Colors.White);
                }
                return;
            }

        }
    }
}
