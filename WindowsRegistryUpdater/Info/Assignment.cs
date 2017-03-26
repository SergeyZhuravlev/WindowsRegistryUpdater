using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistryUpdater.Info
{
    public class Assignment
    {
        public bool CaseSensitive { get; set; } = true;
        public string Pattern { get; set; } = "";
        public string Replacer { get; set; } = "";

        public Assignment()
        { }

        public Assignment(Assignment assignment)
        {
            CaseSensitive = assignment.CaseSensitive;
            Pattern = assignment.Pattern;
            Replacer = assignment.Replacer;
        }

        public override int GetHashCode() 
        {
            return CaseSensitive.GetHashCode() ^ Pattern.GetHashCode() << 1 ^ Replacer.GetHashCode() << 4;
        }

        public override bool Equals(Object obj)
        {
            var Obj = obj as Assignment;
            if (Obj == null)
                return false;
            return 
                CaseSensitive.Equals(Obj.CaseSensitive) &&
                Pattern.Equals(Obj.Pattern) &&
                Replacer.Equals(Obj.Replacer);
        }

        public static bool operator ==(Assignment a, Assignment b)
        {
            if (System.Object.ReferenceEquals(a, b))
                return true;

            return a.CaseSensitive == b.CaseSensitive && a.Pattern == b.Pattern && a.Replacer == b.Replacer;
        }

        public static bool operator !=(Assignment a, Assignment b)
        {
            return !(a == b);
        }
    }
}
