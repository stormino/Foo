using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Foo.Base.Desktop.Command
{
    public abstract class UndoableCommandBase : ICommand
    {
        public bool isImplicit { get; private set; }
        private readonly DelegateCommand wrappedDelegateCmd;

        protected UndoableCommandBase() : this(false)
        {
        }

        protected UndoableCommandBase(bool isImplicit)
        {
            this.isImplicit = isImplicit;
            wrappedDelegateCmd = new DelegateCommand(() => Execute());
        }

        /// <summary>
        /// Raises <see cref="DelegateCommandBase.CanExecuteChanged"/> on the UI thread so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommandBase.CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.wrappedDelegateCmd.RaiseCanExecuteChanged();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the <see cref="Action{Object}"/> supplied during construction.
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute();

        protected bool CanExecute(object parameter)
        {
            return this.wrappedDelegateCmd.CanExecute();
        }

        public abstract void Undo();

        public event EventHandler CanExecuteChanged
        {
            add
            {
                this.wrappedDelegateCmd.CanExecuteChanged += value;
            }
            remove
            {
                this.wrappedDelegateCmd.CanExecuteChanged -= value;
            }
        }
    }
}