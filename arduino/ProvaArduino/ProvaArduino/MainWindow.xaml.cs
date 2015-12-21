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

namespace ProvaArduino
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.IO.Ports.SerialPort serialPort1 = new System.IO.Ports.SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 9600;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("1:13:4:9"); //draw a sure starting from 10,2 lenght = 10
            }
            serialPort1.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("2:2:0:0");
            }
            serialPort1.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("3:3:0:0");
            }
            serialPort1.Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            serialPort1.Open();
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("4:3:0:0");
            }
            serialPort1.Close();
        }

    }
    }

