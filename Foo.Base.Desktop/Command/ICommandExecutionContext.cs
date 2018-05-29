using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// Allows to execute <see cref="ICommand"/> objects, keeping track of the context
    /// in which the action is being executed for later being able to free up resources
    /// when needed.
    /// Call the <code>Dispose()</code> to instruct freeing up resources and according executed
    /// commands
    /// </summary>
    public interface ICommandExecutionContext : IDisposable
    {
        /// <summary>
        /// Used to execute a given <see cref="ICommand"/> object
        /// </summary>
        /// <param name="command">the command object to execute</param>
        void Execute(ICommand command);
    }
}
