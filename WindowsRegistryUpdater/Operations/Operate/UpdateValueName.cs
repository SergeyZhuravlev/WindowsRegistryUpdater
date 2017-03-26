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
    public class UpdateValueName : NodeReplacer
    {
        public UpdateValueName(ReplacerOperationInfo replacer) : base(replacer, _ => _.ValueName)
        {}

        protected override void DoApply(OperationalElement element)
        {
            var source = element.Source;
            using (var path = source.Path.OpenKey(true))
            {
                var oldKey = source.ValueName;
                var valueAtRegistry = path.GetValue(oldKey);
                /*var valueAtRegistry = path.GetValue(oldKey).ToString();
                var oldValue = source.Value;
                if (string.Compare(oldValue, valueAtRegistry) != 0)
                    throw new Exception("Obsolete new value for '" + valueAtRegistry + "'");
                var newValue = element.Node.Value;*/
                var newKey = element.Node.ValueName;
                if (path.GetValueNames().Contains(newKey))
                    throw new Exception("Already exists key '" + newKey + "'");
                path.SetValue(newKey, /*newValue*/valueAtRegistry);
                path.DeleteValue(oldKey);
                path.Flush();
            }
        }
    }
}
