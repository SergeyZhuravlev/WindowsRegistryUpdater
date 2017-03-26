using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WindowsRegistryUpdater.Operations;

namespace WindowsRegistryUpdater.Info
{
    public class OperationalInfo
    {
        public OperationalInfo (Collection<OperationalElement> valuesToChangings, Collection<OperationalElement> namesToChangings, Collection<OperationalElement> pathesToChangings)
        {
            ValuesToChangings = valuesToChangings;
            NamesToChangings = namesToChangings;
            PathesToChangings = pathesToChangings;
        }
        public void Visit(Action<Collection<OperationalElement>> visitor)
        {
            visitor(ValuesToChangings);
            visitor(NamesToChangings);
            visitor(PathesToChangings);
        }
        public IEnumerable<OperationalElement> Enumerate
        {
            get
            {
                return ValuesToChangings.Concat(NamesToChangings).Concat(PathesToChangings);
            }
        }
        public void ForEach(Action<OperationalElement> visitor)
        {
            Enumerate.ToList().ForEach(visitor);
        }
        public Collection<OperationalElement> ValuesToChangings { get; private set; }
        public Collection<OperationalElement> NamesToChangings { get; private set; }
        public Collection<OperationalElement> PathesToChangings { get; private set; }
    }
}
