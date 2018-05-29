using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using Foo.Base.Desktop.Command;
using System.Windows.Input;

namespace Foo.Base.Desktop.UnitTests.Command
{
    [TestClass]
    public class CommandExecutionContextTest_Acceptance
    {
        private CommandHandler handler;
        private UndoRedoStack<UndoableCommandBase> stack;

        [TestInitialize()]
        public void SetUp()
        {
            stack = new UndoRedoStack<UndoableCommandBase>();
            handler = new CommandHandler(stack);
        }

        [TestCleanup()]
        public void TearDown()
        {
            handler = null;
        }

        [TestMethod]
        public void ShouldCorrectlyExecuteTheCommands()
        {
            // Arrange
            var obj = new ValueObject() { Number = 1 };
            var context = new CommandExecutionContext(handler);

            // Act
            context.Execute(new AddOneCommand(obj));

            // Assert
            Assert.AreEqual(2, obj.Number);
        }


        [TestMethod]
        public void ShouldProperlyRemoveCommandsBoundToAGivenExecutionContext()
        {
            // Arrange
            var cmd1 = new AddOneCommand(new ValueObject() { Number = 1 });
            var cmd2 = new AddOneCommand(new ValueObject() { Number = 1 });
            var context1 = new CommandExecutionContext(handler);
            var context2 = new CommandExecutionContext(handler);

            // Act
            context1.Execute(cmd1);
            context2.Execute(cmd2);

            // Assert
            Assert.AreEqual(2, stack.UndoItems().Count(), "There should be 2 undo entries");
            context1.Dispose();
            Assert.AreEqual(1, stack.UndoItems().Count(), "There should be 1 undo entry left");
            Assert.AreEqual(cmd2, stack.UndoItems().ElementAt(0), "Should be the correct command obj");
        }
    }
}
