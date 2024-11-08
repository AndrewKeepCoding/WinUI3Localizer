using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace WinUI3Localizer;

internal sealed class DependencyObjectReferenceAddedEventArgs : EventArgs
{
    public DependencyObjectReferenceAddedEventArgs(Type addedItemType, int itemsTotal)
    {
        AddedItemType = addedItemType;
        ItemsTotal = itemsTotal;
    }

    public Type AddedItemType { get; }

    public int ItemsTotal { get; }
}

internal sealed class DependencyObjectReferenceRemovedEventArgs : EventArgs
{
    public DependencyObjectReferenceRemovedEventArgs(Type removedItemType, int itemsTotal)
    {
        RemovedItemType = removedItemType;
        ItemsTotal = itemsTotal;
    }

    public Type RemovedItemType { get; }

    public int ItemsTotal { get; }
}

internal sealed class DependencyObjectWeakReferences
{
    public readonly List<Item> items = new();

    public event EventHandler<DependencyObjectReferenceAddedEventArgs>? DependencyObjectAdded;

    public event EventHandler<DependencyObjectReferenceRemovedEventArgs>? DependencyObjectRemoved;

    public int Count => this.items.Count;

    public record Item(Type Type, WeakReference<DependencyObject> WeakReference);

    public void Add(DependencyObject dependencyObject)
    {
        WeakReference<DependencyObject> reference = new(dependencyObject);
        Item item = new(dependencyObject.GetType(), reference);
        this.items.Add(item);
        OnDependencyObjectReferenceAdded(item.Type);
    }

    public IReadOnlyCollection<DependencyObject> GetDependencyObjects()
    {
        List<DependencyObject> dependencyObjects = new();

        for (int i = this.items.Count - 1; i >= 0; i--)
        {
            Item targetItem = this.items[i];

            if (targetItem.WeakReference.TryGetTarget(out DependencyObject? aliveObject) is false)
            {
                Type type = targetItem.Type;
                this.items.RemoveAt(i);
                OnDependencyObjectReferenceRemoved(type);
                continue;
            }

            dependencyObjects.Add(aliveObject);
        }

        return dependencyObjects;
    }

    private void OnDependencyObjectReferenceAdded(Type addedItemType)
    {
        DependencyObjectAdded?.Invoke(this, new DependencyObjectReferenceAddedEventArgs(addedItemType, Count));
    }

    private void OnDependencyObjectReferenceRemoved(Type removedItemType)
    {
        DependencyObjectRemoved?.Invoke(this, new DependencyObjectReferenceRemovedEventArgs(removedItemType, Count));
    }
}