using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;
using Foo.Base.Desktop.Command;

namespace Foo.Base.Desktop.UnitTests.Command
{
    class TestObject
    {
        public int Index { get; private set; }

        public TestObject(int index = 0)
        {
            Index = index;
        }
    }

    public class UndoRedoStackTest
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

        [TestClass]
        public class TheAddItemMethod : UndoRedoStackTest
        {
            [TestMethod]
            public void ShouldCorrectlyAddANewItemAndHaveItInTheRedoList()
            {
                // Arrange
                var item = new TestObject();

                // Act
                stack.AddItem(item);
                var resultItem = stack.UndoItems().ElementAt(0);

                // Assert
                Assert.AreEqual(item, resultItem, "The item should be equal");
            }

           
        }

        [TestClass]
        public class TheUndoMethod : UndoRedoStackTest
        {
            private TestObject addedObject;

            [TestInitialize()]
            public new void SetUp()
            {
                base.SetUp();
            }

            [TestCleanup()]
            public new void TearDown()
            {
                base.TearDown();

                addedObject = null;
                stack = null;
            }

            private void PrepareStackToPerformUndo()
            {
                addedObject = new TestObject();
                stack.AddItem(addedObject);
            }

            [TestMethod]
            public void ShouldCorrectlyReturnAPreviouslyAddedUndoItem()
            {
                // Arrange
                PrepareStackToPerformUndo();

                // Act
                var returnItem = stack.Undo();

                // Assert
                Assert.AreEqual(addedObject, returnItem, "The item should be equal");
            }

            [TestMethod]
            public void ShouldHaveAnItemInTheRedoListWhenPerformingAnUndo()
            {
                // Arrange
                PrepareStackToPerformUndo();

                // Act
                var returnItem = stack.Undo();

                // Assert
                Assert.AreEqual(addedObject, stack.RedoItems().ElementAt(0), "The object should be in the redo list");
            }

            [TestMethod]
            public void ShouldReturnNullWhenPerformingAnUndoAndTheStackIsEmpty()
            {
                // Act
                var item = stack.Undo();

                // Assert
                Assert.IsNull(item, "The returned item should be null as there is nothing to be undone");
            }
        }

        [TestClass]
        public class TheRedoMethod : UndoRedoStackTest
        {
            private TestObject undoneObject;

            [TestInitialize()]
            public new void SetUp()
            {
                base.SetUp();
            }

            [TestCleanup()]
            public new void TearDown()
            {
                base.TearDown();

                undoneObject = null;
                stack = null;
            }

            private void PrepareStackWithItemToPerformRedo()
            {
                undoneObject = new TestObject();
                stack.AddItem(undoneObject);
                stack.Undo(); // So we now should have it in the redo list
            }

            [TestMethod]
            public void ShouldReturnTheObjectToRedo()
            {
                // Arrange
                PrepareStackWithItemToPerformRedo();

                // Act
                var item = stack.Redo();

                // Assert
                Assert.AreEqual(undoneObject, item);
            }

            [TestMethod]
            public void ShouldHaveTheItemInTheUndoListWhenPerformingAgainARedo()
            {
                // Arrange
                PrepareStackWithItemToPerformRedo();

                // Act
                stack.Redo();

                // Assert
                Assert.AreEqual(undoneObject, stack.UndoItems().ElementAt(0));
            }

            [TestMethod]
            public void ShouldReturnNullWhenPerformingAnUndoAndTheStackIsEmpty()
            {
                // Act
                var item = stack.Redo();

                // Assert
                Assert.IsNull(item, "The returned item should be null as there is nothing to be redone");
            }

        }

        [TestClass]
        public class TheStackSizeLimit : UndoRedoStackTest
        {
            [TestMethod]
            public void ShouldLimitTheStackSizeAccordingly()
            {
                // Arrange / act
                for (int i = 0; i < stack.MaxStackSize + 10; i++)
                {
                    stack.AddItem(new TestObject(i));
                }

                // Assert
                Assert.AreEqual(stack.MaxStackSize, stack.UndoItems().Count(), "There should be 10 undo objects in the stack");
                Assert.AreEqual(stack.MaxStackSize - 10, stack.UndoItems().ElementAt(0).Index, "Id should match");

                var objToUndo = stack.Undo();
                Assert.AreEqual(stack.MaxStackSize + 10 - 1, objToUndo.Index, "Should be the last added object");
            }

        }
    }
}
