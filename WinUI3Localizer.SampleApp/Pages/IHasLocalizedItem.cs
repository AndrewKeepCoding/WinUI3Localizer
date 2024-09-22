using Microsoft.UI.Xaml.Input;

namespace WinUI3Localizer.SampleApp.Pages;

public interface IHasLocalizedItem
{
    event PointerEventHandler? LocalizedItemPointerEntered;
}