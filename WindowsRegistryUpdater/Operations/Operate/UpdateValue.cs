using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistry;
using WindowsRegistryUpdater.Info;
using StringExtensions;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public class UpdateValue : NodeReplacer
    {
        public UpdateValue(ReplacerOperationInfo replacer) : base(replacer, _=> _.Value)
        {}

        protected override void DoApply(OperationalElement element)
        {
            var source = element.Source;
            using(var path = source.Path.OpenKey(true))
            {
                var oldKey = source.ValueName;
                var valueAtRegistry = path.GetValue(oldKey).ToString();
                var oldValue = source.Value;
                if (string.Compare(oldValue, valueAtRegistry) != 0)
                    throw new Exception("Obsolete new value for '" + valueAtRegistry + "'");
                var newValue = element.Node.Value;
                var currentKey = element.Node.ValueName;
                path.SetValue(currentKey, newValue);
                path.Flush();
            }
        }
    }
}
