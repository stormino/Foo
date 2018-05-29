using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// Identifies the type of operation that has been performed
    /// </summary>
    public enum CommandOperation
    {
        Execute,
        Undo,
        Redo,
        Cleanup
    }
}
