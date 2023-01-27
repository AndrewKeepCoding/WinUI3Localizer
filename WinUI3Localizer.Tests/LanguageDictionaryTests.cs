using FluentAssertions;

namespace WinUI3Localizer.Tests;

public class LanguageDictionaryTests
{
    [Fact]
    public void AddItem_GivenNewItem_AddsItemToDictionary()
    {
        // Arrange
        LanguageDictionary sut = new("en-US");
        string uid = "Uid";
        LanguageDictionary.Item inputItem = new(uid, "DependencyPropertyName", "Value", "StringResourceItemName");

        // Act
        sut.AddItem(inputItem);

        // Assert
        sut.TryGetItems(uid, out LanguageDictionary.Items? items).Should().BeTrue();
        items.Should().Contain(inputItem);
    }

    [Fact]
    public void AddItem_GivenExistingItem_AddsItemToExistingCollection()
    {
        // Arrange
        LanguageDictionary sut = new("en-US");
        string uid = "Uid";
        LanguageDictionary.Item item1 = new(uid, "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionary.Item item2 = new(uid, "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);

        // Act
        sut.AddItem(item2);

        // Assert
        sut.TryGetItems(uid, out LanguageDictionary.Items? items);
        items.Should().HaveCount(2);
        items.Should().Contain(item1);
        items.Should().Contain(item2);
    }

    [Fact]
    public void GetItems_ReturnsAllItems()
    {
        // Arrange
        LanguageDictionary sut = new("en-US");
        LanguageDictionary.Item item1 = new("Uid1", "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionary.Item item2 = new("Uid2", "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);
        sut.AddItem(item2);

        // Act
        IEnumerable<LanguageDictionary.Item> items = sut.GetItems();

        // Assert
        items.Should().HaveCount(2);
        items.Should().Contain(item1);
        items.Should().Contain(item2);
    }

    [Fact]
    public void GetItemsCount_ReturnsCountOfAllItems()
    {
        // Arrange
        LanguageDictionary sut = new("en");
        LanguageDictionary.Item item1 = new("Uid1", "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionary.Item item2 = new("Uid2", "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);
        sut.AddItem(item2);

        // Act
        int count = sut.GetItemsCount();

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public void TryGetItem_GivenExistingUid_OutputsItemsAndReturnsTrue()
    {
        // Arrange
        LanguageDictionary sut = new("en-US");
        string uid = "Uid";
        LanguageDictionary.Item item1 = new(uid, "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionary.Item item2 = new(uid, "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);
        sut.AddItem(item2);

        // Act
        bool result = sut.TryGetItems(item1.Uid, out LanguageDictionary.Items? items);

        // Assert
        result.Should().BeTrue();
        items.Should().HaveCount(2);
        items.Should().Contain(item1);
        items.Should().Contain(item2);
    }

    [Fact]
    public void TryGetItem_GivenNonExistentUid_OutputsNullAndReturnsFalse()
    {
        // Arrange
        LanguageDictionary sut = new("en-US");
        LanguageDictionary.Item item = new("Uid", "DependencyPropertyName", "Value", "StringResourceItemName");

        // Act
        bool result = sut.TryGetItems(item.Uid, out LanguageDictionary.Items? items);

        // Assert
        result.Should().BeFalse();
        items.Should().BeNull();
    }
}