using FluentAssertions;

namespace WinUI3Localizer.Tests;

public class PriResourceReaderTest
{
    [Fact]
    public void GetItems_ReturnsAllItems()
    {
        LanguageDictionaryItem[]? resourcesItems = null;
        LanguageDictionaryItem[]? errorMessagesItems = null;

        Thread? thread = new Thread(() =>
        {
            Microsoft.Windows.ApplicationModel.DynamicDependency.Bootstrap.Initialize(0x00010005);

            string? priFile = Path.Combine(AppContext.BaseDirectory, "resources.pri");
            PriResourceReaderFactory? factory = new();

            PriResourceReader? reader = factory.GetPriResourceReader(priFile);

            resourcesItems = reader.GetItems("en-US").ToArray();
            errorMessagesItems = reader.GetItems("es-ES", "ErrorMessages").ToArray();

            Microsoft.Windows.ApplicationModel.DynamicDependency.Bootstrap.Shutdown();
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        LanguageDictionaryItem item1 = new LanguageDictionaryItem("ControlsPage_Button", "ContentProperty", "ControlsPage_Button.Content", "Click");
        LanguageDictionaryItem item2 = new LanguageDictionaryItem("StylesPage_Top", "TextProperty", "StylesPage_Top.Text", "Top");
        LanguageDictionaryItem item3 = new LanguageDictionaryItem("/ErrorMessages/ErrorMessageExample", "TextProperty", "/ErrorMessages/ErrorMessageExample.Text", "Ejemplo de mensajes de error");

        resourcesItems.Should().HaveCount(55);
        resourcesItems.Should().ContainEquivalentOf(item1);
        resourcesItems.Should().ContainEquivalentOf(item2);
        resourcesItems.Should().NotContainEquivalentOf(item3);

        errorMessagesItems.Should().HaveCount(1);
        errorMessagesItems.Should().NotContainEquivalentOf(item1);
        errorMessagesItems.Should().NotContainEquivalentOf(item2);
        errorMessagesItems.Should().ContainEquivalentOf(item3);
    }

}
