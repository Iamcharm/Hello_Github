using System.Windows;
using System;
using System.Collections.ObjectModel;

namespace WpfApplication3
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            //this.Topmost = true;//设置为最顶层
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //this.Close();
            Environment.Exit(0);
        }

        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Menu_MMF_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_MMF.Visibility = System.Windows.Visibility.Visible;
            this.Tabitem_MMF.IsSelected = true;
        }

        private void Menu_Budget_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_Budget.Visibility = System.Windows.Visibility.Visible;
        }

        private void Menu_Bond_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_Bond.Visibility = System.Windows.Visibility.Visible;
        }

        private void Menu_Analysis_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_Analysis.Visibility = System.Windows.Visibility.Visible;
        }

        private void Menu_yieldcurve_Click(object sender, RoutedEventArgs e)
        {
            this.Tabtiem_yieldcurve.Visibility = System.Windows.Visibility.Visible;
        }

        private void Menu_RealTime_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_RealTime.Visibility = System.Windows.Visibility.Visible;
        }

        private void Menu_Fund_Click(object sender, RoutedEventArgs e)
        {
            this.Tabitem_Fund.Visibility = System.Windows.Visibility.Visible;
        }
        
    }
}


