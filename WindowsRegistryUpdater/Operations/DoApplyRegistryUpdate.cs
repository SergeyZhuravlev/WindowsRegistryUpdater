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
    public class DoApplyRegistryUpdate
    {
        private HashSet<TraversedType> TraverseTypesSet { get; set; }

        private CancellationToken Token { get; set; }

        public DoApplyRegistryUpdate(HashSet<TraversedType> traverseTypesSet, CancellationToken token)
        {
            Token = token;
            TraverseTypesSet = traverseTypesSet;
        }

        public void Execute(OperationalInfo info)
        {
            info.Visit((Collection<OperationalElement> changing) =>
            {
                if (Token.IsCancellationRequested)
                    return;
                foreach (var operational in changing.Where(_ => TraverseTypesSet.Contains(_.Type)).Select((job, index) => new { job, index}).ToList())
                {
                    if (Token.IsCancellationRequested)
                        return;
                    if (operational.job.Enabled && operational.job.Operate.State == OperateState.NothingWas)
                        operational.job.Operate.Apply(operational.job, (OperationalElement newJob) =>
                            UI.Invoke(() => changing[operational.index] = new OperationalElement(newJob)));
                }
            });
        }
    }
}
