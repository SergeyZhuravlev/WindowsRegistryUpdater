using Common.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsRegistry;
using WindowsRegistryUpdater.Info;
using Tasks;
using WindowsRegistryUpdater.Operations.Operate;

namespace WindowsRegistryUpdater
{
    //todo: Add changeability roots to search 
    public class MainWindowViewController: Propertyfull
    {
        public ICommand Traverse { get; private set; }
        public ICommand Preview { get; private set; }
        public ICommand Apply { get; private set; }
        public ICommand Clear { get; private set; }
        public ICommand Selection { get; private set; }
        public ICommand Interrupt { get; private set; }

        public Action Interrupter;

        public MainWindowViewController()
        {
            Interrupter = () => { Model.InetrruptSource.Cancel(); };

            Traverse = new AsyncCommand(async () =>
            {
                SetProgressState();
                await Model.DoTraverse(new PatternOperationInfo(CaseSensitive, Pattern));
                SetTraversedState();
            },
            () => !InProgress && !string.IsNullOrWhiteSpace(Pattern));

            Preview = new RelayCommand(
                SetProgressState,
                //()=> _initialAssignment = new Assignment(Model.Assignment),
                () => Model.DoPreparation(new ReplacerOperationInfo(CaseSensitive, Pattern, Replacer)),
                SetPreviewedState
                ) { CanExecutePredicate = _ => !InProgress /*&& _initialAssignment != Model.Assignment*/ && Model.OperationalInfo.Enumerate.Any(/*job => job.Enabled*/) };

            Apply = new RelayCommand(
                SetProgressState,
                Model.DoApply,
                SetApplyedState
                ) { CanExecutePredicate = _ => !InProgress  && State == OperationalState.Previewed && Model.OperationalInfo.Enumerate.Any(/*job => job.Enabled*/) };

            Clear = new RelayCommand(
                SetProgressState,
                Model.DoClear,
                SetApplyedState
                ) { CanExecutePredicate = _ => !InProgress && State > OperationalState.Virgin };

            Selection = new RelayCommand(
                SetProgressState,
                Model.DoSelection,
                SetUnprogressState
                ) { CanExecutePredicate = _ => !InProgress && Model.OperationalInfo.Enumerate.Any() };

            Interrupt = new RelayCommand(
                Interrupter
                ) { CanExecutePredicate = _ => InProgress };
        }

        private void SetProgressState()
        {
            Model.InetrruptSource.Dispose();
            Model.InetrruptSource = new CancellationTokenSource();
            InProgress = true;
        }

        private void SetUnprogressState()
        {
            InProgress = false;
        }

        private void SetApplyedState()
        {
            if (!Model.InetrruptSource.IsCancellationRequested)
                State = OperationalState.Virgin;
            else
                InProgress = false;
        }

        private void SetPreviewedState()
        {
            if (!Model.InetrruptSource.IsCancellationRequested)
                State = OperationalState.Previewed;
            else
                InProgress = false;
        }

        private void SetTraversedState()
        {
            if (!Model.InetrruptSource.IsCancellationRequested)
                State = OperationalState.Traversed;
            else
                InProgress = false;
        }

        private void SetUnTraversedState()
        {
            State = OperationalState.Virgin;
        }

        public enum OperationalState
        {
            Virgin,
            Traversed,
            Previewed
        }

        private OperationalState _state = OperationalState.Virgin;
        public OperationalState State
        {
            get
            {
                return _state;
            }
            private set
            {
                _state = value;
                InProgress = false;
                UpdateCommands();
            }
        }

        private bool _inProgress;
        public bool InProgress
        {
            get
            {
                return _inProgress;
            }
            private set
            {
                SetField(ref _inProgress, value);
                UpdateCommands();
            }
        }

        public MainWindowModelController Model { get; set; } = new MainWindowModelController();

        public bool CaseSensitive
        {
            get
            {
                return Model.Assignment.CaseSensitive;
            }
            set
            {
                Model.Assignment.CaseSensitive = value;
                //SetUnTraversedState();
            }
        }

        public string Pattern
        {
            get
            {
                return Model.Assignment.Pattern;
            }
            set
            {
                Model.Assignment.Pattern = value;
                //SetUnTraversedState();
            }
        }

        public string Replacer
        {
            get
            {
                return Model.Assignment.Replacer;
            }
            set
            {
                Model.Assignment.Replacer = value;
                /*if (State > OperationalState.Traversed)
                    State = OperationalState.Traversed;*/
            }
        }

        public bool AllowFindInValue
        {
            get
            {
                return Model.TraverseTypesSet.Contains(TraversedType.InValue);
            }
            set
            {
                Model.UpdateTraverseTypesSet(TraversedType.InValue, value);
                SetUnTraversedState();
            }
        }

        public bool AllowFindInKeyName
        {
            get
            {
                return Model.TraverseTypesSet.Contains(TraversedType.InKeyName);
            }
            set
            {
                Model.UpdateTraverseTypesSet(TraversedType.InKeyName, value);
                SetUnTraversedState();
            }
        }

        public bool AllowFindInPath
        {
            get
            {
                return Model.TraverseTypesSet.Contains(TraversedType.InPath);
            }
            set
            {
                Model.UpdateTraverseTypesSet(TraversedType.InPath, value);
                SetUnTraversedState();
            }
        }

        public Assignment _initialAssignment = new Assignment();

        private void UpdateCommands()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
