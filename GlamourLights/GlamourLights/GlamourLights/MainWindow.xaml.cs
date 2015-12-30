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
using System.Data.Entity;
using GlamourLights.Controller;

namespace GlamourLights
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// Non fa praticamente nulla se non istanziare il db + creare la window dove mostrare le pagine
    /// </summary>
    public partial class MainWindow : Window
    {

        ShopManager shopMan = new ShopManager(new Model.ShopState());
        public MainWindow()
        {
            InitializeComponent();
            shopMan.shopState.shopDb.customer.Load();
            // dataGrid1.ItemsSource = myDb.customer.Local;
            mainFrame.Navigate(new Home(shopMan));
        }



    }
}
