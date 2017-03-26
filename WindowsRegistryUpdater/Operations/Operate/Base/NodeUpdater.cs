using ReferenceWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistry;
using WindowsRegistryUpdater.Info;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public abstract class NodeUpdater : Operate
    {
        protected ReplacerOperationInfo Replacer { get; private set; }
        
        public NodeUpdater(ReplacerOperationInfo replacer)
        {
            Replacer = replacer;
        }

        public override void Apply(OperationalElement element, Action<OperationalElement> updater)
        {
            try
            {
                DoApply(element);
                State = OperateState.Success;
                ErrorMessageWhileApply = "Success";
            }
            catch (Exception ex)
            {
                State = OperateState.Failed;
                ErrorMessageWhileApply = ex.Message;
            }
            updater?.Invoke(element);
        }

        protected override void DoPreview(OperationalElement element)
        {
            var newNode = (RegistryInfo)(element.Source).Clone();
            element.OldNode = element.Source;
            PreviewNode(element, element.OldNode, newNode);
            element.Node = newNode;
        }

        protected abstract void PreviewNode(OperationalElement element, RegistryInfo oldNode, RegistryInfo newNode);
    }
}
