using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistryUpdater.Info
{
    public class PatternOperationInfoGeneric<CaseSensitiveType>
    {
        public PatternOperationInfoGeneric(CaseSensitiveType caseSensitive, string pattern)
        {
            CaseSensitive = caseSensitive;
            Pattern = pattern;
        }

        public CaseSensitiveType CaseSensitive { get; private set; }
        public string Pattern { get; private set; }
    }

    public class PatternOperationInfo: PatternOperationInfoGeneric<bool>
    {
        public PatternOperationInfo (bool caseSensitive, string pattern)
            : base(caseSensitive, pattern)
        { }
    }

    public class ReplacerOperationInfo : PatternOperationInfo
    {
        public ReplacerOperationInfo(bool caseSensitive, string pattern, string replacer)
            : base(caseSensitive, pattern)
        {
            Replacer = replacer;
        }

        public string Replacer { get; private set; }
    }
}
