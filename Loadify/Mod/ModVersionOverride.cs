using System.Runtime.Serialization;

namespace Loadify.Mod
{
    [DataContract]
    public class ModVersionOverride : ModVersion
    {
        [DataMember(Name = "overriden_version")]
        public ModVersion Overriden { get; set; } = null;

        public ModVersionOverride(ModVersion def, ModVersion overriden) : base(def)
        {
            Overriden = overriden;
        }
        public override string ToString()
        {
            return base.ToString() + $" ({Overriden})";
        }
    }
}
