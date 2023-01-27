using Microsoft.UI.Xaml.Input;

namespace WinUI3Localizer.SampleApp;

public interface IHasLocalizedItem
{
    event PointerEventHandler? LocalizedItemPointerEntered;
}