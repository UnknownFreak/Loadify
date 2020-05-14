using Ionic.Zip;
using Loadify.Extension;
using Loadify.Models;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media;

namespace Loadify.Mod
{
    public class ModDataView : BaseView
    {
        private ModVersion modVersion = new ModVersion("");

        public Brush IconBrush { get; set; } = Brushes.LightGray;

        public PackIconModernKind IsCompatible { get; set; } = PackIconModernKind.Question;
        public ModVersion ModVersion { get => modVersion; set
            {
                modVersion = value;
                if (value is ModVersionOverride)
                    CompatCheck((value as ModVersionOverride).Overriden.Equals(App.stellarisVersion));
                else
                    CompatCheck(modVersion.Equals(App.stellarisVersion));
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsCompatible");
                NotifyPropertyChanged("IconBrush");
            }
        }

        void CompatCheck(bool Compatible)
        {
            if(Compatible)
            {
                IsCompatible = PackIconModernKind.Check;
                IconBrush = Brushes.DarkGreen;
            }
            else
            {
                IsCompatible = PackIconModernKind.Warning;
                IconBrush = Brushes.Yellow;
            }
        }

        public string SteamLink { get; set; }
        public ObservableCollection<string> RequirementModColl { get; set; } = new ObservableCollection<string>();
        public ModDataView()
        {
            Name = "asd";
        }

        public ModDataView(int index, string filename)
        {
            ViewType = Utils.ViewType.Mod;
            Index = index;
            bool isLocalMod = true;
            string[] lines = new string[0];
            if (filename.Contains("ugc_") || filename.Contains("descriptor.mod") ||filename.Contains(".zip"))
            {
                isLocalMod = false;
            }
            if (filename.Contains(".zip"))
            {
                using (ZipFile zip = ZipFile.Read(filename))
                {
                    foreach (ZipEntry e in zip)
                    {
                        if (e.FileName == "descriptor.mod")
                        {
                            e.Extract("tmp", ExtractExistingFileAction.OverwriteSilently);
                            lines = File.ReadAllLines("tmp/descriptor.mod");
                        }
                    }
                }
            }
            else
                lines = File.ReadAllLines(filename);

            foreach (string s in lines)
            {
                string[] keyValue = s.Split('=');
                if (keyValue.Length >= 2)
                {
                    string Key = keyValue[0];
                    string Value = keyValue[1].Replace("\"", "");

                    if (Key.ToLower() == "name")
                    {
                        Name = Value;
                        if (Id == string.Empty)
                            Id = Name;
                    }
                    else if (Key.ToLower() == "supported_version")
                    {
                        ModVersion = new ModVersion(Value);
                    }
                    else if (Key.ToLower() == "version")
                    {
                        //ModVersion = new ModVersion(Value);
                    }
                    else if (Key.ToLower() == "remote_file_id" && !isLocalMod)
                    {
                        Id = Value;
                        SteamLink = $"https://steamcommunity.com/sharedfiles/filedetails/?id={Value}";
                    }
                }
            }
        }

        public static ModDataView TryRead(int index, string filename, int retries, int delay_ms)
        {
            ModDataView v = null;
            while (retries > 0 && v == null)
            {
                try
                {
                    v = new ModDataView(index, filename);
                }
                catch (System.IO.IOException)
                {
                    retries--;
                    System.Threading.Thread.Sleep(delay_ms);
                }
            }
            return v;

        }

        public void GetRequirementModsFromSteamWorkshop()
        {
            if (string.IsNullOrEmpty(SteamLink))
                return;
            HttpWebRequest request = WebRequest.CreateHttp(SteamLink);
            try
            {
                var lines = ReadLines(() => request.GetResponse().GetResponseStream(), Encoding.UTF8).ToList();
                bool requiredItemsFound = false;
                string requirementsLines = string.Empty;
                int foundA = 0;
                foreach (string line in lines)
                {
                    if (requiredItemsFound)
                    {
                        requirementsLines += line;
                        if (line.Contains("<a href"))
                        {
                            foundA++;
                        }
                        if (line.Contains("</a>"))
                        {
                            requirementsLines = requirementsLines.Replace("\t", "").Split(new string[] { "requiredItem\">" }, StringSplitOptions.None)[1].Split(new string[] { "</div" }, StringSplitOptions.None)[0];
                            RequirementModColl.DispatchedAdd(requirementsLines);
                            requirementsLines = string.Empty;
                            foundA--;
                        }
                    }

                    if (line.Contains("RequiredItems"))
                    {
                        requiredItemsFound = true;
                    }

                    if (requiredItemsFound && foundA == 0 && line.Contains("</div>"))
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        public IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
