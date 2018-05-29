using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;
using Foo.Base.Desktop.Command;

namespace Foo.Base.Desktop.UnitTests.Command
{
    [TestClass]
    public class UndoRedoStackTest_Performance
    {
        private UndoRedoStack<TestObject> stack;

        [TestInitialize()]
        public void SetUp()
        {
            stack = new UndoRedoStack<TestObject>();
        }

        [TestCleanup()]
        public void TearDown()
        {
            stack = null;
        }

        // Just to have get alerted about potential performance impacts in the implementation
        [TestMethod]
        public void ShouldProcess100000ItemsFastly()
        {
            // Arrange
            var expected = 200; // ObservableCollection makes it slower... but still, shouldn't be an issue
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Act
            for (int i = 0; i < 100000; i++)
            {
                stack.AddItem(new TestObject(i));
            }

            // Assert
            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < expected, "Should be faster than " + expected + " (actual:" + stopwatch.ElapsedMilliseconds + ")");
        }
    }
}
