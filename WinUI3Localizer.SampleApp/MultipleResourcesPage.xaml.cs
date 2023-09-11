using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3Localizer.SampleApp;

public sealed partial class MultipleResourcesPage : Page, IHasLocalizedItem
{
    public MultipleResourcesPage()
    {
        InitializeComponent();
    }

    public event PointerEventHandler? LocalizedItemPointerEntered;

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        LocalizedItemPointerEntered?.Invoke(sender, e);
    }
}
