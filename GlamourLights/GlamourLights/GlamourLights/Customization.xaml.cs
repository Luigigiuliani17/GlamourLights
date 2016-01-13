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



namespace GlamourLights
{
    /// <summary>
    /// enum of possible modes of personalization
    /// </summary>
    public enum modeList
    {
        shop_layout = 0,
        shelves_position = 1,
        department_position =2,
        light_position = 3
    }

    /// <summary>
    /// Logica di interazione per Customization.xaml
    /// </summary>
    public partial class Customization : Page
    {
        public ShopManager shopManager { get; set; }
        public List<int> shelves_list { get; set; }
    //    public List<shelf> shelves { get; set; }
        public List<int> department_list { get; set; }
        public modeList active_mode { get; set; }
        public int shelf_selected {get; set; }
        public int department_selected { get; set; }
        public matrixButton[,] buttonMatrix;
        public Customization()
        {

        }

        public Customization(ShopManager sm)
        {
            InitializeComponent();
            //give an initial value to shelf_selected and department_selected
            shelf_selected = -1;
            department_selected = -1;
            this.shopManager = sm;
            //build shelves list 
            shelves_list = new List<int>();
            shopManager.shopState.shopDb.shelf.Load();
      //      shelves = shopManager.shopState.shopDb.shelf.Local.ToList<shelf>();
            
            foreach(int k in shopManager.shopState.shelves_position.Keys)
            {
                shelves_list.Add(k);
            }    
            //build department list
            department_list = new List<int>();
            foreach (int k in shopManager.shopState.department_position.Keys)
            {
                department_list.Add(k);
            }

            active_mode = modeList.shop_layout;
            initializeMatrix(sm.shopState);
            Listbox.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// create the matrix of button
        /// </summary>
        /// <param name="st"></param>
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
                    //set the correct initial color
                    updateButtonColor(buttonMatrix[i, j]);
                    //set the correct button position in the grid
                    Grid.SetRow(buttonMatrix[i, j], j);
                    Grid.SetColumn(buttonMatrix[i, j], i);
                }
            }
            showLightsPositions();
            //add listener to the click action for every button
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < numCols; j++)
                    buttonMatrix[i, j].Click += matrixButtonClick;
        }

        /// <summary>
        /// listener of the click of the matrix button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void matrixButtonClick(object sender, RoutedEventArgs e)
        {
            
            if (sender is matrixButton)
            {
                matrixButton b = sender as matrixButton;
                //case 1: change the button from shelf to free or viceversa (and updates its color accordingly)
                if (active_mode == modeList.shop_layout)
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
                    return;
                }
                //case 2: update the position of the selected shelf and the color of the button
                if(active_mode == modeList.shelves_position)
                {
                    //if no shelves seleced, or if b is not free, then return without doing anything
                    if (shelf_selected == -1 || b.kind != 1)
                        return;

                    //retrieve coordinates of active shelf
                    String[] parameters;
                    int x, y;                 
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);
                    //set default color to that button
                    buttonMatrix[x, y].Background = Brushes.White;

                    //set new color to the selected button
                    b.Background = Brushes.ForestGreen;
                    //change shelf position in shop_state.shelves_position
                    shopManager.shopState.shelves_position[shelf_selected] = b.x_cord + ";" + b.y_cord;
                    showLightsPositions();
                }

                //case 3: update the position of the selected department and the color of the button accordingly
                if (active_mode == modeList.department_position)
                {
                    //if no department seleced, or if b is not free, then return without doing anything
                    if (department_selected == -1 || b.kind != 1)
                        return;

                    //retrieve coordinates of active department
                    String[] parameters;
                    int x, y;
                    parameters = shopManager.shopState.department_position[department_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);
                    //set default color to that button
                    buttonMatrix[x, y].Background = Brushes.White;

                    //set new color to the selected button
                    b.Background = Brushes.YellowGreen;
                    //change shelf position in shop_state.shelves_position
                    shopManager.shopState.department_position[department_selected] = b.x_cord + ";" + b.y_cord;
                    showLightsPositions();
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
                    b.Background = Brushes.Red;
                    break;
                case 0:
                    b.Background = Brushes.Violet;
                    break;
                case 1:
                    b.Background = Brushes.White;
                    break;
                case 2:
                    b.Background = Brushes.Yellow;
                    break;                  
            }           
        }

        /// <summary>
        /// to save the changes (invoked by the save button)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            shopManager.saveChanges();
        }

        /// <summary>
        /// called when the user change the selection in the shelves list 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void department_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (active_mode == modeList.shelves_position)
            {
                //delete old color of old_shelf_selected
                String[] parameters;
                int x, y;
                if (shelf_selected != -1)
                {
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = Brushes.White;
                }

                //set new shelf_selected
                shelf_selected = (int)Listbox.SelectedItem;
                parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                x = Int32.Parse(parameters[0]);
                y = Int32.Parse(parameters[1]);

                buttonMatrix[x, y].Background = Brushes.ForestGreen;
                showLightsPositions();
            }
            if(active_mode == modeList.department_position)
            {
                String[] parameters;
                int x, y;
                if (department_selected!= -1)
                {
                    parameters = shopManager.shopState.department_position[department_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = Brushes.White;
                }

                
                try
                {
                    //set new department_selected
                    department_selected = (int)Listbox.SelectedItem;
                    parameters = shopManager.shopState.department_position[department_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);
                    buttonMatrix[x, y].Background = Brushes.GreenYellow;
                    showLightsPositions();
                } catch (Exception e2)
                {
                    return;
                }
               
            }
            
        }

        /// <summary>
        /// functions that changes the mode of update. it is called by the pressing of the change mode button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeMode_Click(object sender, RoutedEventArgs e)
        {
            //shop_layout--->shelves
            if (active_mode == modeList.shop_layout)
            {
                operatingModeTextbox.Text = "Shelves mode     ";
                active_mode = modeList.shelves_position;
                Listbox.Visibility = Visibility.Visible;
                Listbox.ItemsSource = shelves_list;
                showLightsPositions();
                String[] parameters;
                int x, y;
                if (shelf_selected != -1)
                {
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = Brushes.ForestGreen;
                }
                return;
            }
            //shelves---> department
            if (active_mode == modeList.shelves_position)
            {
                operatingModeTextbox.Text = "Department mode";
                active_mode = modeList.department_position;
                Listbox.ItemsSource = department_list;
                //return to correct color
                String[] parameters;
                int x, y;
                if (shelf_selected != -1)
                {
                    parameters = shopManager.shopState.shelves_position[shelf_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = Brushes.White;
                }


                return;
            }
            //department-->shop_layout
            if (active_mode == modeList.department_position)
            {
                operatingModeTextbox.Text = "Shop layout mode";
                active_mode = modeList.shop_layout;
                Listbox.Visibility = Visibility.Hidden;
                //change to correct color
                String[] parameters;
                int x, y;
                if (department_selected != -1)
                {
                    parameters = shopManager.shopState.department_position[department_selected].Split(';');
                    x = Int32.Parse(parameters[0]);
                    y = Int32.Parse(parameters[1]);

                    buttonMatrix[x, y].Background = Brushes.White;
                }
            }

        }

        private void showLightsPositions()
        {
            String[] parameters;
            int x_light, y_light;
            foreach (string light in shopManager.shopState.lights_position.Keys)
            {
                parameters = light.Split(';');
                x_light = Int32.Parse(parameters[0]);
                y_light = Int32.Parse(parameters[1]);
                if(buttonMatrix[x_light,y_light].Background == Brushes.White)
                {
                    buttonMatrix[x_light, y_light].Background = Brushes.LightGreen;
                }
            }
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            shopManager.shopState = new ShopState();
            this.NavigationService.GoBack();
        }
    }
}
