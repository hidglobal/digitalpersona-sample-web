using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json.Linq;

namespace DPWebDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Trace.Listeners.Add(new Logger(LogTextBox));
            if( Properties.Settings.Default.BaseURL != null)
                WebApiAddress.Text = Properties.Settings.Default.BaseURL;
        }

       
        private void CleatLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.Clear();
        }

        private void SetAddressButton(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if( WebApiAddress.Text != null)
            {
                Properties.Settings.Default.BaseURL = WebApiAddress.Text;
                Properties.Settings.Default.Save();
            }
        }


    }
}
