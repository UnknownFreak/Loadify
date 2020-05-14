using System;
using System.Runtime.Serialization;

namespace Loadify.Mod
{
    [DataContract]
    public class StringInt
    {
        public static readonly String STAR = "*";

        [DataMember(Name = "s")]
        public String S { get; set; }
        [DataMember(Name = "v")]
        public int V { get; set; }

        public StringInt(int i)
        {
            V = i;
        }

        public StringInt(String s) : this(-1)
        {
            S = s;
        }

        public static implicit operator StringInt(int v)
        {
            return new StringInt(v);
        }

        public static implicit operator StringInt(string s)
        {
            return new StringInt(s);
        }

        public override string ToString()
        {
            if (V == -1)
                return S;
            return V.ToString();
        }

        public bool IsSet()
        {
            return (V != -1) || (S == STAR);
        }
        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                StringInt p = (StringInt)obj;
                return V == p.V;
            }
        }
    }
}
