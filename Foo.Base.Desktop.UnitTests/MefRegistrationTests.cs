using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using Foo.Base.Desktop.Command;
using System.ComponentModel.Composition.Hosting;

namespace Foo.Base.Desktop.IntegrationTests
{
    [TestClass]
    public class MefRegistrationTests
    {
        private CompositionContainer container;

        [TestInitialize]
        public void SetUp()
        {
            this.container = SetupMefContainer();            
        }

        [TestCleanup]
        public void TearDown()
        {
            this.container.Dispose();
        }

        private CompositionContainer SetupMefContainer()
        {
            var mainCatalog = new AggregateCatalog(new AssemblyCatalog(typeof(BaseDesktopModule).Assembly));

            var compositionContainer = new CompositionContainer(mainCatalog);
            compositionContainer.ComposeParts(this);
            return compositionContainer;
        }

        [TestMethod]
        public void ShouldCorrectlySetupUndoRedoStack()
        {
            Assert.IsNotNull(container.GetExportedValue<IUndoRedoStack<UndoableCommandBase>>());
        }

        [TestMethod]
        public void ShouldCorrectlySetupCommandHandler()
        {
            Assert.IsNotNull(container.GetExportedValue<IUndoRedoStack<ICommandHandler>>());
        }

        [TestMethod]
        public void ShouldCorrectlySetupCommandExecutionContext()
        {
            Assert.IsNotNull(container.GetExportedValue<IUndoRedoStack<ICommandExecutionContext>>());
        }
    }
}
