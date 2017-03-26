using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UIHelpers
{
    public static class UI
    {
        public static void Invoke(Action forExecute)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                forExecute();
            else
                Application.Current.Dispatcher.Invoke(forExecute);
        }

        public static Result Invoke<Result>(Func<Result> forExecute)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                return forExecute();
            else
                return Application.Current.Dispatcher.Invoke(forExecute);
        }
    }
}
