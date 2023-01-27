using Microsoft.UI.Xaml;

namespace WinUI3Localizer.SampleApp;

public record LanguageItem(string Language, string? UidKey);

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(this.AppTitleBar);
    }
}