using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsRegistry;
using Tasks;

namespace WindowsRegistryUpdater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var vc = new MainWindowViewController();
            var taskManager = TaskManager.Instance.Value;
            taskManager.Interruptor = vc.Interrupter;
            Closing += taskManager.OnClosing;
            DataContext = vc;
            InitializeComponent();
        }
    }
}
