using Loadify.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Loadify.Mod
{
    [DataContract]
    public class ModVersion
    {
        [DataMember(Name = "major")]
        StringInt Major { get; set; } = string.Empty;
        [DataMember(Name = "minor")]
        StringInt Minor { get; set; } = string.Empty;
        [DataMember(Name = "build")]
        StringInt Build { get; set; } = string.Empty;
        [DataMember(Name = "revision")]
        StringInt Revision { get; set; } = string.Empty;


        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                ModVersion p = (ModVersion)obj;
                bool IsBuildCompatible;
                if (Build.V == -1)
                    IsBuildCompatible = true;
                else
                    IsBuildCompatible = Build.V == p.Build.V;
                return (Major.Equals(p.Major)) && (Minor.Equals(p.Minor)) && IsBuildCompatible;
            }
        }

        public override string ToString()
        {
            string s = String.Empty;
            if (Major.IsSet())
            {
                s += Major;
                if (Minor.IsSet())
                {
                    s += "." + Minor;
                    if (Build.IsSet())
                    {
                        s += "." + Build;
                        if (Revision.IsSet())
                            s += "." + Revision;
                    }
                }
            }
            return s;
        }

        public ModVersion(string version)
        {
            List<String> ch = version.Split('.').ToList();
            foreach ((int Index, String Value) in ch.Enumerate())
            {
                if (Index == 0)
                    Major = Set(Value);
                if (Index == 1)
                    Minor = Set(Value);
                if (Index == 2)
                    Build = Set(Value);
                if (Index == 3)
                    Revision = Set(Value);
            }
        }

        private StringInt Set(String Value)
        {
            if (Value == StringInt.STAR)
                return Value;
            if (int.TryParse(Value, out int i))
            {
                return i;
            }
            return string.Empty;
        }

        public ModVersion(ModVersion copy)
        {
            Major = copy.Major;
            Minor = copy.Minor;
            Build = copy.Build;
            Revision = copy.Revision;
        }

        public ModVersion(int Major, int Minor, int Build, int Revision)
        {
            this.Major = Major;
            this.Minor = Minor;
            this.Build = Build;
            this.Revision = Revision;
        }

        public ModVersion(int Major, int Minor, int Build) : this(Major, Minor, Build, 0) {}

        public ModVersion(int Major, int Minor) : this(Major, Minor, 0) {}

    }
}
