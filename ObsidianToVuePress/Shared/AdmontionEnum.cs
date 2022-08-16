using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsidianToVuePress.Shared
{
    public static class AdmontionMap
    {
        public static readonly Dictionary<string, string> AdmontionPairs 
            = new Dictionary<string, string>
        {
            {"theorem", "info" },
            {"lemma", "info" },
            {"remark", "note" },
            {"note", "note" },
            {"definition", "tip" },
            {"assumption", "tip" },
            {"problem", "warning" },
            {"corollary", "danger" },
            {"example", "details" },
        };
    }
}
