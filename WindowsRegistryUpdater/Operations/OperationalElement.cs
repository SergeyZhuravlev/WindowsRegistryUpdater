using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistry;
using WindowsRegistryUpdater.Operations.Operate;

namespace WindowsRegistryUpdater.Operations
{
    public class OperationalElement
    {
        public TraversedType Type { get; private set; }
        public RegistryInfo Node { get; set; }
        public RegistryInfo OldNode { get; set; } = null;
        public bool Enabled { get; set; } = true;

        public RegistryInfo Source { get { return OldNode ?? Node; } }

        public IOperate Operate { get; set; } = new NothingDoing();

        public OperationalElement(OperationalElement operationalElement)
        {
            Type = operationalElement.Type;
            Node = operationalElement.Node;
            OldNode = operationalElement.OldNode;
            Enabled = operationalElement.Enabled;
            Operate = operationalElement.Operate;
        }

        public OperationalElement(TraversedType type, RegistryInfo node)
        {
            Type = type;
            Node = node;
            if (node == null)
                throw new Exception("Node can't be empty in OperationalElement");
        }
    }
}
