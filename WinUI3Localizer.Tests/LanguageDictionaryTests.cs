using FluentAssertions;

namespace WinUI3Localizer.Tests;

public class LanguageDictionaryTests
{
    [Fact]
    public void GetItems_ReturnsAllItems()
    {
        // Arrange
        LanguageDictionary sut = new("en-US", "test");
        LanguageDictionaryItem item1 = new("Uid1", "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionaryItem item2 = new("Uid2", "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);
        sut.AddItem(item2);

        // Act
        IEnumerable<LanguageDictionaryItem> items = sut.GetItems();

        // Assert
        items.Should().HaveCount(2);
        items.Should().Contain(item1);
        items.Should().Contain(item2);
    }

    [Fact]
    public void Count_ReturnsCountOfAllItems()
    {
        // Arrange
        LanguageDictionary sut = new("en", "test");
        LanguageDictionaryItem item1 = new("Uid1", "DependencyPropertyName1", "Value1", "StringResourceItemName1");
        LanguageDictionaryItem item2 = new("Uid2", "DependencyPropertyName2", "Value2", "StringResourceItemName2");
        sut.AddItem(item1);
        sut.AddItem(item2);

        // Act
        int count = sut.Count;

        // Assert
        count.Should().Be(2);
    }
}