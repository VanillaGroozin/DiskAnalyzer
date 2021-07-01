using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using DiskAnalyzer.ViewModels;
using MaterialDesignThemes.Wpf;
using DiskAnalyzer.Helpers;

namespace DiskAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Session session = Session.Instance;
        
        public MainWindow()
        {
            InitializeComponent();
        }


        private void FolderView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new DirectoryViewModel();
            HomeInfo.Visibility = Visibility.Hidden;
        }

        private void DiscView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new DiscViewModel();
            HomeInfo.Visibility = Visibility.Hidden;
        }

        private void ProgramsView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = new ProgramsViewModel();
            HomeInfo.Visibility = Visibility.Hidden;
        }
        private void HomeView_Clicked(object sender, RoutedEventArgs e)
        {
            DataContext = null;
            HomeInfo.Visibility = Visibility.Visible;
        }
        

    }
}
