using System;

namespace WindowsRegistryUpdater.Operations.Operate
{
    public enum OperateState
    {
        NothingWas,
        Success,
        Failed
    }
    public interface IOperate
    {
        void Preview(OperationalElement element, Action<OperationalElement> updater/* = null*/);
        void Apply(OperationalElement element, Action<OperationalElement> updater/* = null*/);
        OperateState State { get; }
        string ErrorMessageWhileApply { get; }
    }
}