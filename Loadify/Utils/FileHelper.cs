using Loadify.DataContract;
using Loadify.DataContracts;
using Loadify.Extension;
using Loadify.Mod;
using Loadify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace Loadify.Utils
{
    static class FileHelper
    {

        public static void Load<T>(ref T tOut, string file)
        {
            try
            {
                FileStream s = new FileStream(file, FileMode.Open);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T loaded = (T)serializer.ReadObject(s);
                tOut = loaded;
            }
            catch
            {
            }
        }

        public static void Save<T>(T toSave, string file)
        {
            StreamWriter tmp = File.CreateText(file);
            tmp.Close();

            FileStream s = new FileStream(file, FileMode.OpenOrCreate);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(s, toSave);

            s.Flush();
            s.Close();
        }
        public static Dictionary<string, ModVersionOverride> LoadModVersionOverrides()
        {
            Dictionary<string, ModVersionOverride> loaded = new Dictionary<string, ModVersionOverride>();
            Load(ref loaded, "modIdVersionOverrides.json");
            return loaded;
        }
        public static void SaveModVersionOverride(Dictionary<string, ModVersionOverride> toSave)
        {
            Save(toSave, "modIdVersionOverrides.json");
        }

        public static StellarisLauncherInfo LoadLauncherInfo()
        {
            StellarisLauncherInfo info = new StellarisLauncherInfo();
            Load(ref info, Properties.Settings.Default.Steamapps + "/common/Stellaris/launcher-settings.json");
            return info;
        }

        public static List<ModCollection> LoadCollections()
        {
            List<ModCollection> l = new List<ModCollection>();
            Load(ref l, "collections.json");
            return l;
        }
        public static void SaveCollections(ObservableCollection<BaseView> view)
        {
            var collection = view.Where(x => x.ViewType == ViewType.Collection).Select(y => ModCollection.Of(y as CollectionView)).ToList();
            Save(collection, "collections.json");
        }

        public static void LoadView(ref ObservableCollection<BaseView> view, ref Dictionary<string, ModDataView> modIdDataMapper, ref ModFile theModLoadOrder)
        {
            var collections = LoadCollections();

            if (Properties.Settings.Default.StellarisDir == string.Empty)
                return ;

            //List<string> files = Directory.GetFileSystemEntries(Properties.Settings.Default.StellarisDir + "/mod", "*.mod").ToList();
            List<string> files = Directory.GetFileSystemEntries(Properties.Settings.Default.Steamapps + "/workshop/content/281990/", "*.*", SearchOption.AllDirectories).Where(x => x.Contains(".mod") ||x.Contains(".zip")).ToList();
            foreach ((int index, string s) in files.Enumerate())
            {
                var v = new ModDataView(index, s);
                view.DispatchedAdd(v);
                modIdDataMapper.Add(v.Id, v);
            }
            view.ChainTask((x) => (x as ModDataView).GetRequirementModsFromSteamWorkshop()).Start();
            foreach ((int index, string s) in theModLoadOrder.EnabledMods.Enumerate())
            {
                view[index].Index = modIdDataMapper[s].Index;
                modIdDataMapper[s].IsEnabled = true;
                modIdDataMapper[s].Index = index;

                view.Swap(view[index], modIdDataMapper[s], (a, b) =>
                {
                    return new Tuple<int, int>(a.Index, b.Index);
                });
            }

            foreach(var v in collections)
            {
                if (v.Index < view.Count)
                    view.DispatchedInsert(v.Index, v.ToCollectionView(view));
                else
                    view.DispatchedAdd(v.ToCollectionView(view));
            }
        }
    }
}
