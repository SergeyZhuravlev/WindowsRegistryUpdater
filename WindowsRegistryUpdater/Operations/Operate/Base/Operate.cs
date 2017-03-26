using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public abstract class Operate: IOperate
    {
        private string _errorMessageWhileApply = String.Empty;
        private OperateState _state;

        public string ErrorMessageWhileApply
        {
            get
            {
                return _errorMessageWhileApply;
            }
            protected set
            {
                _errorMessageWhileApply = value;
            }
        }

        public OperateState State
        {
            get
            {
                return _state;
            }
            protected set
            {
                _state = value;
            }
        }
        protected abstract void DoApply(OperationalElement element);
        protected abstract void DoPreview(OperationalElement element);

        public virtual void Apply(OperationalElement element, Action<OperationalElement> updater)
        {
            DoApply(element);
            updater?.Invoke(element);
        }

        public virtual void Preview(OperationalElement element, Action<OperationalElement> updater)
        {
            DoPreview(element);
            updater?.Invoke(element);
        }
    }
}
