using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace Loadify.Window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this._NavigationFrame.Navigate(new Uri("Pages/LoadOrderPage.xaml", UriKind.Relative));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this._NavigationFrame.Navigate(new Uri("Pages/SettingsFrame/SettingsPage.xaml", UriKind.Relative));
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void Save()
        {
            Properties.Settings.Default.Save();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }
    }
}
