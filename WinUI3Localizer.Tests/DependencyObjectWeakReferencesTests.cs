using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Moq;
using Xunit;
using FluentAssertions;

namespace WinUI3Localizer.Tests;

public class DependencyObjectWeakReferencesTests
{
    [Fact]
    public void Add_InvokesDependencyObjectReferenceAddedEvent()
    {
        // Arrange
        DependencyObjectWeakReferences sut = new();
        DependencyObject dependencyObject = new Mock<DependencyObject>().Object;
        Type? addedType = null;
        int itemsTotal = 0;
        sut.DependencyObjectAdded += (sender, args) =>
        {
            addedType = args.AddedItemType;
            itemsTotal = args.ItemsTotal;
        };

        // Act
        sut.Add(dependencyObject);

        // Assert
        addedType.Should().NotBeNull();
        addedType.Should().Be(dependencyObject.GetType());
        itemsTotal.Should().Be(1);
    }

    //[Fact]
    //public async Task GetDependencyObjects_Should_Remove_Dead_References_And_Invoke_DependencyObjectReferenceRemoved_Event()
    //{
    //    // Arrange
    //    var sut = new DependencyObjectWeakReferences();
    //    var dependencyObject1 = new Mock<DependencyObject>().Object;
    //    var dependencyObject2 = new Mock<DependencyObject>().Object;
    //    var dependencyObject3 = new Mock<DependencyObject>().Object;
    //    sut.Add(dependencyObject1);
    //    sut.Add(dependencyObject2);
    //    sut.Add(dependencyObject3);
    //    Type removedType = null;
    //    int itemsTotal = 0;
    //    sut.DependencyObjectRemoved += (sender, args) =>
    //    {
    //        removedType = args.RemovedItemType;
    //        itemsTotal = args.ItemsTotal;
    //    };

    //    // Act
    //    sut.items[1].WeakReference.SetTarget(null); // set the second item's reference to null to simulate a dead reference
    //    var result = await sut.GetDependencyObjects();

    //    // Assert
    //    result.Should().BeEquivalentTo(new List<DependencyObject> { dependencyObject1, dependencyObject3 });
    //    removedType.Should().Be(dependencyObject2.GetType());
    //    itemsTotal.Should().Be(2);
    //}

    //[Fact]
    //public void Dispose_Should_Dispose_Semaphore()
    //{
    //    // Arrange
    //    var sut = new DependencyObjectWeakReferences();
    //    var semaphoreMock = new Mock<SemaphoreSlim>();
    //    sut.semaphore = semaphoreMock.Object;

    //    // Act
    //    sut.Dispose();

    //    // Assert
    //    semaphoreMock.Verify(x => x.Dispose(), Times.Once);
    //}
}