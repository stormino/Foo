using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Foo.Base.Desktop.Command
{

    [Export(typeof(IUndoRedoStack<>))]
    class UndoRedoStack<TItem> : IUndoRedoStack<TItem>
    {
        public int _MaxStackSize = 20;
        public int MaxStackSize
        {
            get { return _MaxStackSize; }
            set { _MaxStackSize = value; }
        }

        private readonly ObservableCollection<TItem> UndoStack;
        private readonly ReadOnlyObservableCollection<TItem> ReadOnlyUndoStack;
        private readonly ObservableCollection<TItem> RedoStack;
        private readonly ReadOnlyObservableCollection<TItem> ReadOnlyRedoStack;

        public UndoRedoStack()
        {
            UndoStack = new ObservableCollection<TItem>();
            RedoStack = new ObservableCollection<TItem>();

            ReadOnlyUndoStack = new ReadOnlyObservableCollection<TItem>(UndoStack);
            ReadOnlyRedoStack = new ReadOnlyObservableCollection<TItem>(RedoStack);
        }

        public void AddItem(TItem item)
        {
            if (UndoStack.Count() >= MaxStackSize)
            {
                UndoStack.RemoveAt(0);
            }

            UndoStack.Add(item);
            RedoStack.Clear();
        }

        public TItem Undo()
        {
            TItem item = default(TItem);
            if (CanUndo)
            {
                item = UndoStack.Pop();
                RedoStack.Add(item);
            }

            return item;
        }

        public TItem Redo()
        {
            TItem item = default(TItem);
            if (CanRedo)
            {
                item = RedoStack.Pop();
                UndoStack.Add(item);
            }

            return item;
        }

        public ReadOnlyObservableCollection<TItem> UndoItems()
        {
            return ReadOnlyUndoStack;
        }

        public ReadOnlyObservableCollection<TItem> RedoItems()
        {
            return ReadOnlyRedoStack;
        }

        public bool CanUndo
        {
            get { return UndoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return RedoStack.Count > 0; }
        }

        public void CleanUp(IEnumerable<TItem> ExecutedCommands)
        {
            foreach (var cmd in ExecutedCommands)
            {
                UndoStack.Remove(cmd);
                RedoStack.Remove(cmd);
            }
        }

        public void Reset()
        {
            UndoStack.Clear();
            RedoStack.Clear();
        }
    }

    static class ListStackExtension
    {
        public static TItem Pop<TItem>(this IList<TItem> list)
        {
            var lastElement = list.Last();
            list.Remove(lastElement);
            return lastElement;
        }
    }
}
