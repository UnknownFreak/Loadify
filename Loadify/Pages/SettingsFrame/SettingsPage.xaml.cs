using System;
using System.Windows;
using System.Windows.Controls;

namespace Loadify.Pages.SettingsFrame
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            StellarisDir.Text = Properties.Settings.Default.StellarisDir;
            Steamapps.Text = Properties.Settings.Default.Steamapps;
            Backup.IsChecked = Properties.Settings.Default.Backup;
            BackupStellarisDir.IsChecked = Properties.Settings.Default.SaveUnderStellarisDir;
            ReopenGame.IsChecked = Properties.Settings.Default.ReopenGameAfterExit;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("LoadOrderPage.xaml", UriKind.Relative));
        }
    }
}
