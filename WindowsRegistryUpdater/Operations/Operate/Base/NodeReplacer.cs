using ReferenceWrapper;
using StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistry;
using WindowsRegistryUpdater.Info;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public abstract class NodeReplacer: NodeUpdater
    {
        protected Func<RegistryInfo, Ref<string>> UpdatableSelector { get; private set; }
        public StringComparison StringComparison { get; private set; }
        public NodeReplacer(ReplacerOperationInfo replacer, Func<RegistryInfo, Ref<string>> updatableSelector) : base(replacer)
        {
            UpdatableSelector = updatableSelector;
            StringComparison = Replacer.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        protected override void PreviewNode(OperationalElement element, RegistryInfo oldNode, RegistryInfo newNode)
        {
            var value = UpdatableSelector(newNode).Value.Replace(Replacer.Pattern, Replacer.Replacer, StringComparison);
            UpdatableSelector(newNode).Assign(value);
        }

        protected override abstract void DoApply(OperationalElement element);
    }
}
