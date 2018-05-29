using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Foo.Base.Desktop.Command
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ICommandExecutionContext))]
    class CommandExecutionContext : ICommandExecutionContext
    {
        private ICommandHandler CommandHandler { get; set; }
        private IList<ICommand> ExecutedCommands { get; set; }

        [ImportingConstructor]
        public CommandExecutionContext(ICommandHandler commandHandler)
        {
            this.CommandHandler = commandHandler;
            ExecutedCommands = new List<ICommand>();
        }

        public void Execute(ICommand command)
        {
            try
            {
                ExecutedCommands.Add(command);
                CommandHandler.Execute(command);
            }
            catch (Exception ex)
            {
                // Log?
                Console.Error.WriteLine("CommandExecutionContext: " + ex.Message);
            }
        }

        public void Dispose()
        {
            CommandHandler.CleanUp(ExecutedCommands);
        }
    }
}
