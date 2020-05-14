using Loadify.Extension;
using Loadify.Models;
using System.Collections.ObjectModel;

namespace Loadify.Utils
{
    public static class CollectionReorderHelper
    {
        public static void ReOrderIndex(ObservableCollection<BaseView> observableCollection)
        {
            int idx = 0;
            int previous_collection_idx = 0;
            foreach ((int index, BaseView s) in observableCollection.Enumerate())
            {
                s.Index = idx;
                if (s.ViewType == ViewType.Collection)
                    idx += ReorderCollection(s as CollectionView);
                else
                    idx++;
            }
            bool no_coll = true;
            foreach ((int index, BaseView s) in observableCollection.Enumerate())
            {
                if (s.ViewType == ViewType.Collection)
                {
                    s.Index = previous_collection_idx;
                    previous_collection_idx++;
                    no_coll = false;
                }
                else
                    if (no_coll)
                        previous_collection_idx++;
            }
            FileHelper.SaveCollections(observableCollection);
        }

        private static int ReorderCollection(CollectionView collectionView)
        {
            int idx = 0;
            foreach ((int index, BaseView s) in collectionView.Children.Enumerate())
            {
                s.Index = collectionView.Index + idx;
                if (s.ViewType == ViewType.Collection)
                    idx += ReorderCollection(s as CollectionView);
                else
                    idx++;
            }
            return idx;
        }
    }
}
