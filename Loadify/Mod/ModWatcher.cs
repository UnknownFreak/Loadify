using Loadify.Extension;
using Loadify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Loadify.Mod
{
    public class ModWatcher
    {
        private readonly FileSystemWatcher Watcher;
        readonly ObservableCollection<BaseView> collection;
        readonly Dictionary<string, ModDataView> modIdDataMapper;

        public ModWatcher(ref ObservableCollection<BaseView> modView, ref Dictionary<string, ModDataView> idMapper)
        {
            collection = modView;
            modIdDataMapper = idMapper;
            Watcher = new FileSystemWatcher(Properties.Settings.Default.StellarisDir + "/mod", "*.mod");
            Watcher.Deleted += Watcher_Deleted;
            Watcher.Changed += Watcher_Changed;
            Watcher.Created += Watcher_Created;

            Watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            var v = ModDataView.TryRead(collection.Count, e.FullPath, 5, 500);
            collection.DispatchedAdd(v);
            modIdDataMapper.Add(v.Id, v);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //string modId = e.Name.Replace("ugc_", "").Replace(".mod", "");
            //collection.DispatchedRemove(modIdDataMapper[modId]);
            //
            //var v = ModDataView.TryRead(-1, e.FullPath, 5, 500);
            //
            //v.Index = modIdDataMapper[modId].Index;
            //collection.DispatchedInsert(v.Index, v);
            //modIdDataMapper[modId] = v;
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string modId = e.Name.Replace("ugc_", "").Replace(".mod", "");
            ModDataView view = modIdDataMapper[modId];
            int index = view.Index;
            for (int i = view.Index; i < collection.Count; i++ )
            {
                collection[i].Index--;
            }
            collection.DispatchedRemove(view);
            modIdDataMapper.Remove(modId);
        }
    }
}
