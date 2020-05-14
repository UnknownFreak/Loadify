using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Loadify.DataContract
{
    [DataContract]
    public class StellarisLauncherInfo
    {
        [DataMember(Name = "browserDlcUrl")]
        public string BrowserDlcUrl { get; set; }

        [DataMember(Name = "browserModUrl")]
        public string BrowserModUrl { get; set; }
        [DataMember(Name = "distPlatform")]
        public string DistPlatform { get; set; }
        [DataMember(Name = "exeArgs")]
        public List<string> ExeArgs { get; set; }
        [DataMember(Name = "exePath")]
        public string ExePath { get; set; }
        [DataMember(Name = "gameDataPath")]
        public string GameDataPath { get; set; }
        [DataMember(Name = "gameEngine")]
        public string GameEngine { get; set; }
        [DataMember(Name = "gameId")]
        public string GameId { get; set; }
        [DataMember(Name = "ingameSettingsLayoutPath")]
        public string InGameSettingsLauoutPath { get; set; }
        [DataMember(Name = "rawVersion")]
        public string RawVersion { get; set; }
        [DataMember(Name = "themeFile")]
        public string ThemeFile { get; set; }
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}
