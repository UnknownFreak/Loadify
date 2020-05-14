using Loadify.Extension;
using Loadify.Mod;
using Loadify.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Loadify.Models
{
    public class CollectionView : BaseView
    {
        private bool is_enabled = false;
        public new bool IsEnabled
        {
            get { return is_enabled; }
            set
            {
                is_enabled = value;
                foreach (BaseView view in Children)
                {
                    view.IsEnabled = is_enabled;
                }
                NotifyPropertyChanged();
            }
        }

        public new bool AllowRename { get; set; } = true;

        public void SetSenalbedNoRunTroughChildren(bool b) { is_enabled = b; NotifyPropertyChanged("IsEnabled"); }

    public ObservableCollection<BaseView> Children { get; set; } = new ObservableCollection<BaseView>();

        public CollectionView()
        {
            Visibility = Visibility.Collapsed;
            ViewType = ViewType.Collection;
            AllowDrop = true;
        }

        public ICollection<ModDataView> GetMods(bool iteratve=true)
        {
            ICollection<ModDataView> collection = new Collection<ModDataView>();
            foreach (BaseView view in Children)
            {
                if (view.ViewType == ViewType.Mod)
                {
                    collection.Add(view as ModDataView);
                }
                else if (view.ViewType == ViewType.Collection && iteratve)
                {
                    collection.AddRange((view as CollectionView).GetMods(iteratve));
                }
            }
            return collection;
        }

        public ICollection<CollectionView> GetCollections()
        {
            ICollection<CollectionView> collection = new Collection<CollectionView>();
            foreach (BaseView view in Children)
            {
                if (view.ViewType == ViewType.Collection)
                {
                    collection.Add(view as CollectionView);
                }
            }
            return collection;
        }
    }
}
