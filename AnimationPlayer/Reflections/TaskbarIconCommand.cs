using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AnimationPlayer.Reflections
{
    public class TaskbarIconCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.MainWindow.Show();  // 顯示視窗
            Application.Current.MainWindow.WindowState = WindowState.Normal;    // 還原視窗狀態
        }
    }
}
