using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// Holds information about the executed operation
    /// </summary>
    public class OperationExecutionEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="ICommand"/> that has been used in the operation
        /// </summary>
        public ICommand CurrentItem { get; private set; }

        /// <summary>
        /// Indicates whether there are remaining undo items
        /// </summary>
        public bool HasUndoItems { get; private set; }

        /// <summary>
        /// Indicates whether there are remaining redo items
        /// </summary>
        public bool HasRedoItems { get; private set; }

        /// <summary>
        /// The kind of operation, <see cref="CommandOperation"/>
        /// </summary>
        public CommandOperation Action { get; private set; }

        public OperationExecutionEventArgs(ICommand item, CommandOperation action, bool hasUndoItems, bool hasRedoItems)
            : base()
        {
            CurrentItem = item;
            Action = action;
            HasUndoItems = hasUndoItems;
            HasRedoItems = hasRedoItems;
        }
    }
}
