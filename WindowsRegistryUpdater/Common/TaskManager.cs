using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace Tasks
{
    public class TaskManager
    {
        public static readonly Lazy<TaskManager> Instance = new Lazy<TaskManager>();

        public readonly SynchronizedCollection<Task> _tasks = new SynchronizedCollection<Task>();
        private bool _isClosing = false;

        public Task RegisterTask(Task task)
        {
            _tasks.Add(task);
            return task.ContinueWith(_ =>
            {
                _tasks.Remove(task);
                _.Wait();
            }, TaskContinuationOptions.AttachedToParent);
        }

        public void WaitAll()
        {
            while (_tasks.Any())
            {
                Task task = null;
                lock (_tasks.SyncRoot)
                {
                    task = _tasks.FirstOrDefault();
                    _tasks.Remove(task);
                }
                if(task != null)
                    task.Wait();
            }
        }

        public async Task AWaitAll(Action interruptor = null)
        {
            await Task.Run(() =>
            {
                if(interruptor != null)
                    interruptor();
                WaitAll(); }
            );
        }

        public Action Interruptor { get; set; }

        public async void OnClosing(object obj, CancelEventArgs arg)
        {
            if (_isClosing)
            {
                arg.Cancel = false;
                return;
            }
            _isClosing = true;
            arg.Cancel = true;
            (obj as Window).Hide();
            await AWaitAll(Interruptor);
            arg.Cancel = false;
            Application.Current.Shutdown();            
        }
    }
}
