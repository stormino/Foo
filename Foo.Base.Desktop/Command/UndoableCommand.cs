using System;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    public class UndoableCommand : ICommand
    {
        private readonly ICommandExecutionContext commandHandler;
        private readonly Func<object, UndoableCommandBase> executeFunction;
        private readonly Func<object, bool> canExecuteFunction;

        public UndoableCommand(ICommandExecutionContext commandHandler,
            Func<object, UndoableCommandBase> executeFunction,
            Func<object, bool> canExecute = null)
        {
            this.commandHandler = commandHandler;
            this.executeFunction = executeFunction;
            this.canExecuteFunction = canExecute;

            this.CanExecuteChanged += UndoableCommand_CanExecuteChanged;
        }

        void UndoableCommand_CanExecuteChanged(object sender, EventArgs e)
        {
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecuteFunction == null || this.canExecuteFunction.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var commandToExecute = executeFunction.Invoke(parameter);
            commandHandler.Execute(commandToExecute);
        }
    }
}
