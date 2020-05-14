using Loadify.DataContract;
using Loadify.Extension;
using Loadify.Mod;
using Loadify.Models;
using Loadify.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Loadify.Pages
{
    /// <summary>
    /// Interaction logic for LoadOrderPage.xaml
    /// </summary>
    public partial class LoadOrderPage : Page
    {
        ObservableCollection<BaseView> collection = new ObservableCollection<BaseView>();
        Dictionary<string, ModDataView> modIdDataMapper = new Dictionary<string, ModDataView>();
        private ModWatcher Watcher { get; set; }
        Dictionary<string, ModVersionOverride> modIdVersionOverride = new Dictionary<string, ModVersionOverride>();

        public LoadOrderPage()
        {
            InitializeComponent();
            CollectionView.ItemsSource = collection;
            if (Properties.Settings.Default.Steamapps == string.Empty)
            {
                if (GetMessageBoxResult("No Steamapps folder set, want to set one now?", "Setup"))
                {
                    string s = string.Empty;
                    SetProperty(ref s, "Steamapps");
                    Properties.Settings.Default.Steamapps = s;
                }
            }

            if (Properties.Settings.Default.StellarisDir == string.Empty)
            {
                if (GetMessageBoxResult("No Stellaris folder set, want to set one now?", "Setup"))
                {
                    string s = string.Empty;
                    SetProperty(ref s, "(MyDocuments) Stellaris (e.g. C:/Users/<USER>/Documents/Paradox Interactive/Stellaris)");
                    Properties.Settings.Default.StellarisDir = s;
                }
            }
            Watcher = new Mod.ModWatcher(ref collection, ref modIdDataMapper);
            ReadStellarisInfo();
            ReadMods();
            ReadOverrides();
            CollectionView.modIdVersionOverride = modIdVersionOverride;
        }

        bool GetMessageBoxResult(string Caption, string Title)
        {
            return MessageBox.Show(Caption, Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void SetProperty(ref string steamapps, string folderToSelect)
        {
            var dialog = new CommonOpenFileDialog($"Please select the {folderToSelect} Folder");
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            steamapps = dialog.FileName;
        }

        public void ReadMods()
        {
            App.modFile = ModFile.Load();
            FileHelper.LoadView(ref collection, ref modIdDataMapper, ref App.modFile);
        }

        public void ReadOverrides()
        {
            modIdVersionOverride = FileHelper.LoadModVersionOverrides();
            foreach(var v in collection.Flatten(x => { if (x.ViewType == ViewType.Collection) return (x as CollectionView).Children; return new ObservableCollection<BaseView>(); }))
            {
                if (v.ViewType == ViewType.Mod)
                    if (modIdVersionOverride.TryGetValue(v.Id, out ModVersionOverride mv))
                        (v as ModDataView).ModVersion = mv;
            }
        }

        public void ReadStellarisInfo()
        {
            if (Properties.Settings.Default.Steamapps == string.Empty)
                return;
            StellarisLauncherInfo info = FileHelper.LoadLauncherInfo();
            StellarisVersion.Content = new Mod.ModVersion(info.RawVersion);
            App.stellarisVersion = StellarisVersion.Content as ModVersion;
            StellarisChecksum.Content = info.Version.Split("(".ToArray())[1].Replace(")", "");
        }
    }
}
