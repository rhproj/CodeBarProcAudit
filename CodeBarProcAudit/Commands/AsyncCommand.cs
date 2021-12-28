using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeBarProcAudit.Commands
{
    internal class AsyncCommand : ICommand
    {
        private readonly Func<Task> _Execute;
        private readonly Func<object, bool> _CanExecute;

        public AsyncCommand(Func<Task> Execute, Func<object, bool> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }

        public bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;

        public async void Execute(object parameter) => await ExecuteAsync(parameter); //gotta be void to keep ICommand interface implementation

        public async Task ExecuteAsync(object parameter) => await _Execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
