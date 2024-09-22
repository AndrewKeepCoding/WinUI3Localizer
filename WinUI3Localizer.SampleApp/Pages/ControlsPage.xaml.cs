using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;

namespace WinUI3Localizer.SampleApp.Pages;

public sealed partial class ControlsPage : Page, IHasLocalizedItem
{
    public ControlsPage()
    {
        InitializeComponent();
    }

    public event PointerEventHandler? LocalizedItemPointerEntered;

    private List<string> ComboBoxItems { get; } = new()
    {
        $"{nameof(ControlsPage)}_ComboBox_OptionA",
        $"{nameof(ControlsPage)}_ComboBox_OptionB",
        $"{nameof(ControlsPage)}_ComboBox_OptionC",
    };

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        LocalizedItemPointerEntered?.Invoke(sender, e);
    }
}