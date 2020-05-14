using Loadify.Utils;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Loadify.DataContract
{
    [DataContract]
    public class ModFile
    {
        [DataMember(Name = "enabled_mods")]
        public List<string> EnabledMods { get; set; }
        [DataMember(Name = "disabled_dlcs")]
        public List<string> DisabledDlcs { get; set; }

        public static ModFile Load()
        {

            ModFile file = new ModFile();
            FileHelper.Load(ref file, Properties.Settings.Default.StellarisDir + "/dlc_load.json");

            List<string> patched = new List<string>();

            foreach (string ss in file.EnabledMods)
            {
                patched.Add(ss.Split('/')[1].Replace("ugc_", "").Replace(".mod", ""));
            }
            file.EnabledMods = patched;
            return file;
        }

        public void Save()
        {
            List<string> patched = new List<string>();
            foreach (string ss in EnabledMods)
            {
                patched.Add($"mod/ugc_{ss}.mod");
            }
            EnabledMods = patched;

            FileHelper.Save(this, Properties.Settings.Default.StellarisDir + "/dlc_load.json");
        }
    }
}
