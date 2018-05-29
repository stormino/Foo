using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// A generic command that takes proper execute/undo <see cref="IAction"/> references
    /// to be executed.
    /// </summary>
    public class DelegateUndoableCommand : UndoableCommandBase
    {
        private readonly Action executeAction;
        private readonly Action undoAction;
        private string header;

        public DelegateUndoableCommand(Action executeAction, Action undoAction) 
            : this(executeAction, undoAction, false, null)
        {
        }

        public DelegateUndoableCommand(Action executeAction, Action undoAction, bool isImplicit)
            : this(executeAction, undoAction, isImplicit, null)
        {
        }

        public DelegateUndoableCommand(Action executeAction, Action undoAction, bool isImplicit, string header)
            : base(isImplicit)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");

            if (undoAction == null)
                throw new ArgumentNullException("undoAction");

            this.executeAction = executeAction;
            this.undoAction = undoAction;
            this.header = header;
        }

        public override void Execute()
        {
            executeAction.Invoke();
        }

        public override void Undo()
        {
            undoAction.Invoke();
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(header))
                return executeAction.Method.Name;
            else
                return header;
        }
    }
}
