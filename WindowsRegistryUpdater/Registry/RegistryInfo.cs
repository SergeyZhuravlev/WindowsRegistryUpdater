using ReferenceWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistry
{
    public class RegistryInfo: ICloneable
    {
        public RegistryInfo(RegistryPath path, string valueName = null, string value = null)
        {
            Path = path;
            ValueName.Assign(valueName);
            Value.Assign(value);
        }
        public override string ToString()
        {
            return $"'{Path.ToString()}' '{(ValueName.Value ?? String.Empty)}' '{(Value.Value ?? String.Empty)}'";
        }

        public object Clone()
        {
            return new RegistryInfo((RegistryPath)Path.Clone(), ValueName.Value?.ToString(), Value.Value?.ToString());
        }

        public RegistryPath Path { get; set; } = null;
        public Ref<string> ValueName { get; set; } = new Ref<string>();//null Nullable
        public Ref<string> Value { get; set; } = new Ref<string>();//null Nullable
    }
}
