using Loadify.Extension;
using Loadify.Mod;
using Loadify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Loadify.DataContracts
{
    [DataContract]
    public class ModCollection
    {
        [DataMember(Name = "modCollection")]
        public List<Mod> ModsInCollection { get; set; }
        [DataMember(Name = "disabled_dlcs")]
        public string CollectionName { get; set; }
        [DataMember(Name = "is_enabled")]
        public bool Enabled { get; set; }

        [DataMember(Name = "innerCollections")]
        public List<ModCollection> InnerCollection { get; set; }
        [DataMember(Name = "index")]
        public int Index { get; set; }

        public static ModCollection Of(CollectionView view)
        {
            return new ModCollection() {
                Enabled = view.IsEnabled,
                CollectionName = view.Name,
                Index = view.Index,
                ModsInCollection = view.GetMods(false).Select(x => new Mod() { Id = x.Id, Enabled = x.IsEnabled }).ToList(),
                InnerCollection = view.GetCollections().Select(x => Of(x)).ToList()
            };
        }

        internal BaseView ToCollectionView(ObservableCollection<BaseView> view, CollectionView parent=null)
        {
            var modids = ModsInCollection.Select(x => x.Id);
            CollectionView collview = new CollectionView
            {
                Index = Index,
                Name = CollectionName,
                Parent = parent
            };
            collview.Children.DispatchedAddRange(view.Where(x => x.ViewType == Utils.ViewType.Mod).Select(x => { x.Parent = collview; return x as ModDataView; }).Where(x => x.Id.IsIn(modids))) ;
            foreach (BaseView item in collview.Children)
                view.DispatchedRemove(item);
            collview.Children.DispatchedAddRange(InnerCollection.Select(x => x.ToCollectionView(view, collview)));
            collview.SetSenalbedNoRunTroughChildren(Enabled);
            return collview;
        }
    }
    [DataContract]
    public class Mod
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "is_enabled")]
        public bool Enabled { get; set; }
    }
}
