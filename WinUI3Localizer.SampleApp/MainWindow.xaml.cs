using Microsoft.UI.Xaml;

namespace WinUI3Localizer.SampleApp;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(this.AppTitleBar);
    }
}
