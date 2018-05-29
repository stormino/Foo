using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foo.Base.Desktop.Command;

namespace Foo.Base.Desktop.UnitTests.Command
{
    [TestClass]
    public class DelegateUndoableCommandTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentExceptionIfExecDelegateNotPassed()
        {
            new DelegateUndoableCommand(null, () => { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowArgumentExceptionIfUndoDelegateNotPassed()
        {
            new DelegateUndoableCommand(() => { }, null);
        }

        [TestMethod]
        public void ShouldCorrectlyExecuteAndUndo()
        {
            // Arrange
            var number = 0;
            var command = new DelegateUndoableCommand(() => number++, () => number--);
            
            // Act
            command.Execute();
            command.Execute();
            Assert.AreEqual(2, number);
            
            command.Undo();
            Assert.AreEqual(1, number);
        }
    }
}
