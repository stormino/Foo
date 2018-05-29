using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Foo.Base.Desktop.Command;

namespace Foo.Base.Desktop.UnitTests.Command
{
    [TestClass]
    public class CommandHandlerTestAcceptance
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
        public void ShouldCorrectlyUndoAndRedoActions()
        {
            var obj = new ValueObject() { Number = 1 };

            handler.Execute(new AddOneCommand(obj));
            Assert.AreEqual(2, obj.Number);

            handler.Undo();
            Assert.AreEqual(1, obj.Number);

            handler.Redo();
            Assert.AreEqual(2, obj.Number);
        }

        [TestMethod]
        public void ShouldProperlyLimitTheAmountOfOperationsThatCanBeUndone()
        {
            // Execute more than 10 operations
            for (int i = 0; i < stack.MaxStackSize + 10; i++)
            {
                handler.Execute(new AddOneCommand(new ValueObject()));    
            }

            Assert.AreEqual(stack.MaxStackSize, stack.UndoItems().Count(), "There should be <= 10 items in the undo stack");
        }

        [TestMethod]
        public void ShouldFailSilentlyWhenUndoingMultipleTimes()
        {
            // Arrange
            var valueObj = new ValueObject();
            handler.Execute(new AddOneCommand(valueObj));

            // Act
            handler.Undo();

            // Assert
            Assert.AreEqual(0, valueObj.Number);
            Assert.ThrowsException<ApplicationException>(() => { handler.Undo(); });
        }

        [TestMethod]
        public void ShouldUndoAndRedoMultipleLevels()
        {
            // Arrange
            var valueObj = new ValueObject();

            for (int i = 0; i < 5; i++)
            {
                handler.Execute(new AddOneCommand(valueObj));
            }
            Assert.AreEqual(5, valueObj.Number);

            // Act
            handler.Undo(3);

            // Assert
            Assert.AreEqual(2, valueObj.Number);

            // Redo
            handler.Redo(2);
            Assert.AreEqual(4, valueObj.Number);
        }

        [TestMethod]
        public void ShouldReturn0WhenNoMoreUndoItemsAreThere()
        {
            Assert.AreEqual(0, handler.UndoItems().Count);
        }

        [TestMethod]
        public void ShouldReturnTheNumberOfAvailableUndoItems()
        {
            handler.Execute(new MyTestCommand());
            handler.Execute(new MyTestCommand());

            Assert.AreEqual(2, handler.UndoItems().Count);
        }

        [TestMethod]
        public void ShouldReturn0WhenNoMoreRedoItemsAreThere()
        {
            Assert.AreEqual(0, handler.RedoItems().Count);
        }

        [TestMethod]
        public void ShouldReturnTheNumberofAvailableRedoItems()
        {
            handler.Execute(new MyTestCommand());
            handler.Execute(new MyTestCommand());
            handler.Execute(new MyTestCommand());
            handler.Undo();
            handler.Undo();

            Assert.AreEqual(2, handler.RedoItems().Count);
        }
    }

    // Nothing more than an object holding a number
    class ValueObject
    {
        public int Number { get; set; }
    }

    // Very simple demo implementations of commands
    class AddOneCommand : UndoableCommandBase
    {
        private ValueObject valObj;

        public AddOneCommand(ValueObject valObj)
        {
            this.valObj = valObj;
        }

        public override void Execute()
        {
            valObj.Number++;
        }

        public override void Undo()
        {
            valObj.Number--;
        }
    }

    class AddTwoCommand : UndoableCommandBase
    {
        private ValueObject valObj;

        public AddTwoCommand(ValueObject valObj)
        {
            this.valObj = valObj;
        }

        public override void Execute()
        {
            valObj.Number = valObj.Number + 2;
        }

        public override void Undo()
        {
            valObj.Number = valObj.Number - 2;
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanUndo
        {
            get { return true; }
        }
    }
}
