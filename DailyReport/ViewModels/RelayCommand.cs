using System;
using System.Windows.Input;

namespace DailyReport.ViewModels
{
    /// <summary>簡易ICommand実装です。<</summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;
        /// <summary>コマンドを作成します。<</summary>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null) { this.execute = execute ?? throw new ArgumentNullException(nameof(execute)); this.canExecute = canExecute; }
        /// <summary>実行可否が変化したときに発生します。<</summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>実行可能か判定します。<</summary>
        public bool CanExecute(object parameter) { return canExecute == null || canExecute(parameter); }
        /// <summary>実行します。<</summary>
        public void Execute(object parameter) { execute(parameter); }
    }
}
