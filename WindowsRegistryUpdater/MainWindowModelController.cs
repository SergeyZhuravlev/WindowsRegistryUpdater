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
using WindowsRegistryUpdater.Operations;
using Tasks;


//todo: Rewrite main operational structures with Propertyfull
namespace WindowsRegistryUpdater
{
    public class MainWindowModelController
    {
        public HashSet<TraversedType> TraverseTypesSet { get; set; }
            = new HashSet<TraversedType>(
                Enum
                .GetValues(typeof(TraversedType))
                .Cast<TraversedType>());

        public void UpdateTraverseTypesSet(TraversedType type, bool value)
        {
            if (value)
                TraverseTypesSet.Add(type);
            else
                TraverseTypesSet.Remove(type);
        }

        public Task DoTraverse(PatternOperationInfo pattern)
        {
            return
                TaskManager.Instance.Value.RegisterTask(
                Task.Factory.StartNew(() =>
            {
                var mc = this;
                var token = mc.InetrruptSource.Token;
                DoClear();
                var registryTraverser = new DoTraverseRegistry(pattern, TraverseTypesSet, token);
                registryTraverser.Execute(OperationalInfo, TraverseErrors);
            }, TaskCreationOptions.LongRunning));
        }

        public void DoPreparation(ReplacerOperationInfo replacer)
        {
            var mc = this;
            var token = mc.InetrruptSource.Token;
            var preparation = new DoPrepareRegistryUpdate(replacer, TraverseTypesSet, token);
            preparation.Execute(OperationalInfo);
        }

        public void DoApply()
        {
            var mc = this;
            var token = mc.InetrruptSource.Token;
            var apply = new DoApplyRegistryUpdate(TraverseTypesSet, token);
            apply.Execute(OperationalInfo);
        }

        public void DoSelection()
        {
            var invertEnabled = !OperationalInfo.Enumerate.Any(_ => _.Enabled);
            OperationalInfo.ForEach(_ => _.Enabled = invertEnabled);
            UpdateAllCollections();
        }

        private void UpdateAllCollections()
        {
            OperationalInfo.Visit(_=> { var saved = _.ToList(); _.Clear(); saved.ToList().ForEach(item => _.Add(item)); });
        }

        public void DoClear()
        {
           UI.Invoke(() =>
           {
               OperationalInfo.Visit(changing => changing.Clear());
               TraverseErrors.Clear();
           });
        }

        public OperationalInfo OperationalInfo { get { return new OperationalInfo(ValuesToChangings, NamesToChangings, PathesToChangings); } }
        public ObservableCollection<string> TraverseErrors { get; private set; } = new ObservableCollection<string>();

        public ObservableCollection<OperationalElement> ValuesToChangings { get; private set; } = new ObservableCollection<OperationalElement>();
        public ObservableCollection<OperationalElement> NamesToChangings { get; private set; } = new ObservableCollection<OperationalElement>();
        public ObservableCollection<OperationalElement> PathesToChangings { get; private set; } = new ObservableCollection<OperationalElement>();

        public CancellationTokenSource InetrruptSource = new CancellationTokenSource();

        public Assignment Assignment { get; set; } = new Assignment();
    }
}
