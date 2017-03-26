using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public class NothingDoing: Operate
    {
        protected override void DoApply(OperationalElement element)
        {
            State = OperateState.Failed;
            ErrorMessageWhileApply = "Unsuported for apply";
        }

        protected override void DoPreview(OperationalElement element)
        {
            State = OperateState.Failed;
            ErrorMessageWhileApply = "Unsuported for apply";
        }
    }
}
