using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;

namespace WinUI3Localizer.SampleApp;

public sealed partial class SettingsPage : Page, IHasLocalizedItem
{
    public SettingsPage()
    {
        InitializeComponent();

        this.StringResourcesFolderHyperLink.Click += StringResourcesFolderHyperLink_Click;
        this.StringResourcesFolderHyperLink.Inlines.Clear();
        this.StringResourcesFolderHyperLink.Inlines.Add(new Run() { Text = App.StringsFolderPath });
    }

    public event PointerEventHandler? LocalizedItemPointerEntered;

    private void StringResourcesFolderHyperLink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        try
        {
            if (App.StringsFolderPath is string path &&
                string.IsNullOrEmpty(path) is false)
            {
                sender.Inlines.Clear();
                sender.Inlines.Add(new Run() { Text = path });
                _ = Process.Start("explorer.exe", path);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        LocalizedItemPointerEntered?.Invoke(sender, e);
    }
}