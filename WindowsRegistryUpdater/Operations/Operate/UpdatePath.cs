using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsRegistryUpdater.Info;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public class UpdatePath: NothingDoing
    {
        public UpdatePath(ReplacerOperationInfo replacer)
        {
            State = OperateState.Failed;
            ErrorMessageWhileApply = "Unsuported for apply";
        }
    }
}
