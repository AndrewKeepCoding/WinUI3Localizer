using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUI3Localizer.Tests;

public class PriResourceReaderTest
{
    [Fact]
    public void GetItems_ReturnsAllItems()
    {
        LanguageDictionary.Item[]? resourcesItems = null;
        LanguageDictionary.Item[]? errorMessagesItems = null;

        Thread? thread = new Thread(() =>
        {
            Microsoft.Windows.ApplicationModel.DynamicDependency.Bootstrap.Initialize(0x00010004);

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

        LanguageDictionary.Item item1 = new LanguageDictionary.Item("ControlsPage_Button", "ContentProperty", "Click", "ControlsPage_Button.Content");
        LanguageDictionary.Item item2 = new LanguageDictionary.Item("StylesPage_Top", "TextProperty", "Top", "StylesPage_Top.Text");
        LanguageDictionary.Item item3 = new LanguageDictionary.Item("/ErrorMessages/ErrorMessageExample", "TextProperty", "Ejemplo de mensajes de error", "/ErrorMessages/ErrorMessageExample.Text");

        resourcesItems.Should().HaveCount(55);
        resourcesItems.Should().Contain(item1);
        resourcesItems.Should().Contain(item2);
        resourcesItems.Should().NotContain(item3);

        errorMessagesItems.Should().HaveCount(1);
        errorMessagesItems.Should().NotContain(item1);
        errorMessagesItems.Should().NotContain(item2);
        errorMessagesItems.Should().Contain(item3);
    }

}
