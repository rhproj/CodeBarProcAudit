using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeBarProcAudit.Commands
{
    internal abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged //можно передать вызов этого события WPF:
        {//1:10:51 передаем управление этим событием классу CommandManager
            add => CommandManager.RequerySuggested += value;  //так реализуетс\ событие явно: add/remove
            remove => CommandManager.RequerySuggested -= value;
        }
        //сделаем их абстрактными, их реализацией займется наследник
        public abstract bool CanExecute(object parameter); //если возвр-т false - команду выполнить незя и элемент к кот. привязана команда отключ-ся
        public abstract void Execute(object parameter); //то, что дожно быть выполнено самой командой, основная логика команды

    }
}
