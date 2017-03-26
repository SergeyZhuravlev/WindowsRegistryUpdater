using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIHelpers;
using WindowsRegistry;
using WindowsRegistryUpdater.Info;
using WindowsRegistryUpdater.Operations.Operate;

namespace WindowsRegistryUpdater.Operations
{
    public class DoPrepareRegistryUpdate
    {
        private HashSet<TraversedType> TraverseTypesSet { get; set; }
        private CancellationToken Token { get; set; }
        private ReplacerOperationInfo Replacer { get; set; }

        public DoPrepareRegistryUpdate(ReplacerOperationInfo replacer, HashSet<TraversedType> traverseTypesSet, CancellationToken token)
        {
            Replacer = replacer;
            Token = token;
            TraverseTypesSet = traverseTypesSet;
        }
        
        private void DoPrepareCollection(Collection<OperationalElement> OperationalElements, Func<IOperate> operateFactory)
        {
            foreach (var operational in OperationalElements.Where(_=>TraverseTypesSet.Contains(_.Type)).Select((job, index) => new { job, index }).ToList())
            {
                if (Token.IsCancellationRequested)
                    return;
                /*if (!(operational.job.Operate is NothingDoing))
                    continue;*/
                if (operational.job.Enabled && operational.job.Operate.State == OperateState.NothingWas)
                {
                    operational.job.Operate = operateFactory();
                    operational.job.Operate.Preview(operational.job, newJob =>
                                UI.Invoke(() => OperationalElements[operational.index] = new OperationalElement(newJob)));
                }
            }
        }

        public void Execute(OperationalInfo info)
        {
            DoPrepareCollection(info.ValuesToChangings, () => new UpdateValue(Replacer));
            DoPrepareCollection(info.NamesToChangings, () => new UpdateValueName(Replacer));
            DoPrepareCollection(info.PathesToChangings, () => new UpdatePath(Replacer));
        }
    }
}
