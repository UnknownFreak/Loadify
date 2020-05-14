using Loadify.DataContract;
using Loadify.Extension;
using Loadify.Mod;
using Loadify.Models;
using Loadify.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Loadify.Pages
{
    /// <summary>
    /// Interaction logic for ModCollectionControl.xaml
    /// </summary>
    public partial class ModCollectionControl : UserControl
    {
        public System.Collections.IEnumerable ItemsSource { get { return TreeView.ItemsSource; } set { TreeView.ItemsSource = value; View = value as ObservableCollection<BaseView>; } }
        ObservableCollection<BaseView> View = new ObservableCollection<BaseView>();
        public Dictionary<string, ModVersionOverride> modIdVersionOverride;
        public ModCollectionControl()
        {
            InitializeComponent();
            TreeView.ItemsSource = View;
            TreeView.DataContext = new BaseView();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ReloadModOrder();
            CollectionReorderHelper.ReOrderIndex(View);
        }

        private void ReloadModOrder()
        {
            var AllMods = View.Where((x) => x.ViewType == Utils.ViewType.Mod ).Select(y => y as ModDataView).ToList();

            var modsInCollection = View.Where(x => x.ViewType == Utils.ViewType.Collection).Select(y => y as CollectionView);

            foreach (var v in modsInCollection)
                AllMods.AddRange(v.GetMods());

            var collection = AllMods.Where(x => x.IsEnabled == true).Select(x => x.Id);
            if (collection.Count() != 0)
            {
                App.modFile.EnabledMods = collection.ToList();
                App.modFile.Save();
            }
        }

        private void TreeListView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is CollectionView)
            {
                RenameCollection.IsEnabled = true;
                DeleteCollection.IsEnabled = true;
                FlagOk.IsEnabled = false;
            }
            else
            {
                RenameCollection.IsEnabled = false;
                DeleteCollection.IsEnabled = false;
                FlagOk.IsEnabled = true;
            }
        }

        private void NewCollection_Click(object sender, RoutedEventArgs e)
        {
            View.Insert(0, new CollectionView() { Name = $"New Collection {new Random().Next()}" });
        }

        private void FlagOk_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                ModDataView view = TreeView.SelectedItem as ModDataView;
                var v = new ModVersionOverride(view.ModVersion, App.stellarisVersion);
                view.ModVersion = v;
                modIdVersionOverride[view.Id] = v;
                FileHelper.SaveModVersionOverride(modIdVersionOverride);
            }
            CollectionReorderHelper.ReOrderIndex(View);
        }

        private void DeleteCollection_Click(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem is CollectionView item)
            {
                View.DispatchedRemove(item);
                foreach (BaseView v in item.Children)
                    View.DispatchedInsert(v.Index, v);
                FileHelper.SaveCollections(View);
            }
        }

        private void EditCollectionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            int? index = (int)(e.Source as TextBox).Tag;
            if (index.HasValue)
                View[index.Value].Name = (e.Source as TextBox).Text;
        }

        private void EditCollectionName_LostFocus(object sender, RoutedEventArgs e)
        {
            FileHelper.SaveCollections(View);
        }
    }
}
