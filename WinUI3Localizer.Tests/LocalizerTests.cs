using FluentAssertions;
using WinUI3Localizer;

public class LocalizerTests
{
    [Fact]
    public void Get_ReturnsInstanceOfLocalizer()
    {
        // Arrange & Act
        ILocalizer result = Localizer.Get();

        // Assert
        result.Should().NotBeNull().And.BeAssignableTo<ILocalizer>();
    }
}
