using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// Handles the execution of <see cref="ICommand"/> objects and according
    /// undo/redo functionality.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Execute a given <see cref="ICommand"/>
        /// </summary>
        /// <param name="command">the <see cref="ICommand"/> to execute</param>
        void Execute(ICommand command);

        /// <summary>
        /// Performs a redo operation of the <see cref="ICommand"/> that has been
        /// undone previously
        /// </summary>
        /// <param name="numberOfRedos">an integer specifying the number of redoes to perform</param>
        void Redo(int numberOfRedos = 1);

        bool CanRedo();

        /// <summary>
        /// Performs an undo operation of the <see cref="ICommand"/> that has been executed before.
        /// </summary>
        /// <param name="numberOfUndoes">an integer specifying the number of undoes to perform</param>
        void Undo(int numberOfUndoes = 1);

        bool CanUndo();

        /// <summary>
        /// A collection of <see cref="ICommand"/> objects that currently sit in the undo item list
        /// </summary>
        /// <returns>a <see cref="ReadOnlyCollection<T>"/> of commands</returns>
        ReadOnlyObservableCollection<UndoableCommandBase> UndoItems();

        /// <summary>
        /// A collection of <see cref="ICommand"/> objects that currently sit in the redo item list
        /// </summary>
        /// <returns>a <see cref="ReadOnlyCollection<T>"/> of commands</returns>
        ReadOnlyObservableCollection<UndoableCommandBase> RedoItems();

        /// <summary>
        /// An event that gets fired when a execute/redo/undo operation is being performed
        /// </summary>
        event EventHandler<OperationExecutionEventArgs> OperationExecuted;

        /// <summary>
        /// Called to cleanup a given set of <see cref="ICommand"/> objects.
        /// </summary>
        /// <param name="ExecutedCommands">an <see cref="IEnumerable<T>"/> of commands to remove</param>
        void CleanUp(IEnumerable<ICommand> ExecutedCommands);

        /// <summary>
        /// Resets the command handler status.
        /// </summary>
        void Reset();
    }
}
