using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3Localizer.SampleApp.Pages;

public sealed partial class NonStringValuesPage : Page, IHasLocalizedItem
{
    public NonStringValuesPage()
    {
        InitializeComponent();
    }

    public event PointerEventHandler? LocalizedItemPointerEntered;

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        LocalizedItemPointerEntered?.Invoke(sender, e);
    }
}
