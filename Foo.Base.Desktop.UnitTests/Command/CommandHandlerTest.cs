using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Foo.Base.Desktop.Command;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Foo.Base.Desktop.UnitTests.Command
{
    class MyNonUndoableTestCommand : ICommand
    {
        public bool HasUndone { get; set; }
        public bool HasExecuted { get; set; }
        public bool ThrowExceptionOnExecuting { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            HasExecuted = true;
            if (ThrowExceptionOnExecuting)
            {
                throw new ApplicationException("Something went wrong");
            }
        }
    }

    class MyTestCommand : UndoableCommandBase
    {
        public MyTestCommand()
        {
        }

        public bool HasUndone { get; set; }
        public bool HasExecuted { get; set; }
        public bool ThrowExceptionOnExecuting { get; set; }

        public override void Execute()
        {
            HasExecuted = true;
            if (ThrowExceptionOnExecuting)
            {
                throw new ApplicationException("Something went wrong");
            }
        }

        public override void Undo()
        {
            HasUndone = true;
        }
    }

    public class CommandHandlerTest
    {
        private CommandHandler handler;
        private Mock<IUndoRedoStack<UndoableCommandBase>> mockUndoRedo;

        [TestInitialize()]
        public void SetUp()
        {
            mockUndoRedo = new Mock<IUndoRedoStack<UndoableCommandBase>>();
            mockUndoRedo.Setup(x => x.RedoItems()).Returns(new ReadOnlyObservableCollection<UndoableCommandBase>(new ObservableCollection<UndoableCommandBase>()));
            mockUndoRedo.Setup(x => x.UndoItems()).Returns(new ReadOnlyObservableCollection<UndoableCommandBase>(new ObservableCollection<UndoableCommandBase>()));
            handler = new CommandHandler(mockUndoRedo.Object);
        }

        [TestCleanup()]
        public void TearDown()
        {
            handler = null;
        }

        [TestClass]
        public class TheExecuteCommandMethod : CommandHandlerTest
        {

            [TestMethod]
            public void ShouldInvokeTheCommand()
            {
                // Arrange
                var myCommand = new MyTestCommand();

                // Act
                handler.Execute(myCommand);

                // Assert
                Assert.IsTrue(myCommand.HasExecuted, "should have been executed");
            }

            [TestMethod]
            public void ShouldAddTheCommandToTheUndoStack()
            {
                // Arrange
                var myCommand = new MyTestCommand();

                // Act
                handler.Execute(myCommand);

                // Assert
                mockUndoRedo.Verify(x => x.AddItem(myCommand), Times.Once(), "The command should have been added to the undo stack");
            }

            [TestMethod]
            public void ShouldRethrowAnyExceptionFiredBy()
            {
                // Arrange
                var myCommand = new MyTestCommand()
                {
                    ThrowExceptionOnExecuting = true
                };

                // Act / assert
                Assert.ThrowsException<ApplicationException>(() => { handler.Execute(myCommand); });
            }

            [TestMethod]
            public void ShouldFireACorrespondingAddEvent()
            {
                // Arrange
                var wasCalled = false;
                handler.OperationExecuted += (object s, OperationExecutionEventArgs e) =>
                                            {
                                                Assert.AreEqual(CommandOperation.Execute, e.Action);
                                                wasCalled = true;
                                            };

                // Act
                handler.Execute(new MyTestCommand());

                // Assert
                Assert.AreEqual(true, wasCalled, "The event should have been fired");
            }

            [TestMethod]
            public void ShouldNotAddACommandToTheUndoStackIfItIsNotUndoable()
            {
                // Arrange
                var myTestCmd = new MyNonUndoableTestCommand();

                // Act
                handler.Execute(myTestCmd);

                // Assert
                mockUndoRedo.Verify(x => x.AddItem(It.IsAny<UndoableCommandBase>()), Times.Never(), "Should not have been added to the collection of undo/redos");
            }
        }

        [TestClass]
        public class TheUndoMethod : CommandHandlerTest
        {
            MyTestCommand myCommand;

            [TestInitialize()]
            public new void SetUp()
            {
                base.SetUp();
                myCommand = new MyTestCommand();
                handler.Execute(myCommand);

                mockUndoRedo.Setup(x => x.Undo()).Returns(myCommand);
            }

            [TestCleanup()]
            public new void TearDown()
            {
                base.TearDown();
                myCommand = null;
            }

            [TestMethod]
            public void ShouldUndoTheCommandByInvokingTheProperMethodOnTheCommandObject()
            {
                // Act
                handler.Undo();

                // Assert
                Assert.IsTrue(myCommand.HasUndone, "The command should have been undone");
            }

            [TestMethod]
            public void ShouldThrowExceptionInCaseAnUndoIsMadeWhenNothingCanBeUndone()
            {
                mockUndoRedo.Setup(x => x.Undo()).Returns((UndoableCommandBase)null);

                // Act / assert
                Assert.ThrowsException<ApplicationException>(() => handler.Undo());
            }

            [TestMethod]
            public void ShouldFireACorrespondingUndoEvent()
            {
                // Arrange
                var wasCalled = false;
                handler.OperationExecuted += (object s, OperationExecutionEventArgs e) =>
                                        {
                                            if (e.Action == CommandOperation.Undo)
                                                wasCalled = true;
                                        };

                // Act
                handler.Undo();

                // Assert
                Assert.AreEqual(true, wasCalled, "The event should have been fired");
            }

        }

        [TestClass]
        public class TheRedoMethod : CommandHandlerTest
        {
            MyTestCommand myCommand;

            [TestInitialize()]
            public new void SetUp()
            {
                base.SetUp();
                myCommand = new MyTestCommand();

                mockUndoRedo.Setup(x => x.Undo()).Returns(myCommand);
                mockUndoRedo.Setup(x => x.Redo()).Returns(myCommand);

                handler.Execute(myCommand);
                handler.Undo();

                myCommand.HasExecuted = false; // Reset it
            }

            [TestCleanup()]
            public new void TearDown()
            {
                base.TearDown();
                myCommand = null;
            }

            [TestMethod]
            public void ShouldReExecuteTheLastCommand()
            {
                // Act
                handler.Redo();

                // Assert
                Assert.IsTrue(myCommand.HasExecuted, "The command should have been re-executed");
            }

            [TestMethod]
            public void ShouldThrowExceptionInCaseARedoIsMadeWhenNothingCanBeRedone()
            {
                mockUndoRedo.Setup(x => x.Redo()).Returns((UndoableCommandBase)null);

                // Act / assert
                Assert.ThrowsException<ApplicationException>(() => handler.Redo());
            }

            [TestMethod]
            public void ShouldFireACorrespondingRedoEvent()
            {
                // Arrange
                var wasCalled = false;
                handler.OperationExecuted += (object s, OperationExecutionEventArgs e) =>
                                        {
                                            if (e.Action == CommandOperation.Redo)
                                                wasCalled = true;
                                        };

                // Act
                handler.Redo();

                // Assert
                Assert.AreEqual(true, wasCalled, "The event should have been fired");
            }

        }
    }
}
