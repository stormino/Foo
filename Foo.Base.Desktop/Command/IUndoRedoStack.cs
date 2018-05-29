using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Foo.Base.Desktop.Command
{
    /// <summary>
    /// Contract for the Undo/Redo stack
    /// </summary>
    /// <typeparam name="TItem">The generic item to manage</typeparam>
    interface IUndoRedoStack<TItem>
    {
        /// <summary>
        /// Adds a new item to the undo stack and empties the redo stack
        /// </summary>
        /// <param name="item">the item to add</param>
        void AddItem(TItem item);

        /// <summary>
        /// Takes the top-level item of the undo stack, returns it and adds that item to the
        /// redo stack.
        /// </summary>
        /// <returns>the top-level item of the undo stack, <c>null</c> if empty</returns>
        TItem Undo();

        /// <summary>
        /// Takes the top-level item of the redo stack, returns it and adds it to the undo stack.
        /// </summary>
        /// <returns>the top-level item of the redo stack, <c>null</c> if empty</returns>
        TItem Redo();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>a <see cref="ReadOnlyCollection<TItem>"/> of available undo items</returns>
        ReadOnlyObservableCollection<TItem> UndoItems();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>a <see cref="ReadOnlyCollection<TItem>"/> of available redo items</returns>
        ReadOnlyObservableCollection<TItem> RedoItems();

        /// <summary>
        /// Specifies whether an undo can be performed based on the number of items in the undo stack
        /// </summary>
        bool CanUndo { get; }
        
        /// <summary>
        /// Specfies whether a redo operation can be performed based on the number of items in the redo stack
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Removes the given set of <paramref name="ExecutedCommands"/> and removes them from
        /// the undo or redo stack.
        /// </summary>
        /// <param name="ExecutedCommands"></param>
        void CleanUp(IEnumerable<TItem> ExecutedCommands);

        /// <summary>
        /// Removes all items from undo and redo stack.
        /// </summary>
        void Reset();
    }
}
