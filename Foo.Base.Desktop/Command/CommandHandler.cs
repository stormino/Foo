using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    [Export(typeof(ICommandHandler))]
    class CommandHandler : ICommandHandler
    {
        private IUndoRedoStack<UndoableCommandBase> stack;

        [ImportingConstructor]
        public CommandHandler(
            [Import(typeof(IUndoRedoStack<UndoableCommandBase>))]
            IUndoRedoStack<UndoableCommandBase> undoRedoHandler)
        {
            stack = undoRedoHandler;
        }

        public void Execute(ICommand command)
        {
            try
            {
                if (typeof(UndoableCommandBase).IsAssignableFrom(command.GetType()))
                {
                    stack.AddItem(command as UndoableCommandBase);
                }
                command.Execute(null);
                RaiseOperationExecuted(command, CommandOperation.Execute);
            }
            catch
            {
                throw;
            }
        }

        public void Redo(int numberOfRedos = 1)
        {
            for (int i = 0; i < numberOfRedos; i++)
            {
                var command = stack.Redo();
                if (command == null)
                {
                    throw new ApplicationException();
                }

                command.Execute();
                RaiseOperationExecuted(command, CommandOperation.Redo);

                if (stack.RedoItems().Count > 0 && stack.RedoItems().First().isImplicit)
                    i--; // Do not increase redo counter if next command is implicit
            }
        }

        public bool CanRedo()
        {
            ReadOnlyObservableCollection<UndoableCommandBase> redoItems = stack.RedoItems();
            for (int i = redoItems.Count - 1; i >= 0; i--)
            {
                if (!redoItems.ElementAt(i).isImplicit)
                    return true;
            }
            return false;
        }

        public void Undo(int numberOfUndos = 1)
        {
            for (int i = 0; i < numberOfUndos; i++)
            {
                var command = stack.Undo();
                if (command == null)
                {
                    throw new ApplicationException();
                }

                command.Undo();
                RaiseOperationExecuted(command, CommandOperation.Undo);

                if (command.isImplicit)
                    i--; // Do not increase undo counter if command is implicit
            }
        }

        public bool CanUndo()
        {
            ReadOnlyObservableCollection<UndoableCommandBase> undoItems = stack.UndoItems();
            for (int i = undoItems.Count - 1; i >= 0; i--)
            {
                if (!undoItems.ElementAt(i).isImplicit)
                    return true;
            }
            return false;
        }

        public ReadOnlyObservableCollection<UndoableCommandBase> UndoItems()
        {
            return stack.UndoItems();
        }

        public ReadOnlyObservableCollection<UndoableCommandBase> RedoItems()
        {
            return stack.RedoItems();
        }

        public event EventHandler<OperationExecutionEventArgs> OperationExecuted;
        protected void RaiseOperationExecuted(ICommand item, CommandOperation operation)
        {
            if (OperationExecuted != null)
                OperationExecuted(this, new OperationExecutionEventArgs(item, operation, stack.CanUndo, stack.CanRedo));
        }

        public void CleanUp(IEnumerable<ICommand> ExecutedCommands)
        {
            var undoableCommands = ExecutedCommands
                                        .Where(x => x is UndoableCommandBase)
                                        .Select(x => x as UndoableCommandBase);

            stack.CleanUp(undoableCommands);
            RaiseOperationExecuted(null, CommandOperation.Cleanup);
        }

        public void Reset()
        {
            stack.Reset();
            RaiseOperationExecuted(null, CommandOperation.Cleanup);
        }
    }

}
