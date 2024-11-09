using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUI3Localizer.SampleApp.Pages;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        InitializeComponent();

        AvailableLanguages = Localizer.Get().GetAvailableLanguages()
            .Select(x => new LanguageItem(Language: x, UidKey: $"{nameof(MainWindow)}_{x}"))
            .ToList();

        this.LanguagesGridView.SelectedItem = AvailableLanguages
            .FirstOrDefault(x => x.Language == Localizer.Get().GetCurrentLanguage());

        this.NavigationViewControl.Loaded += NavigationViewControl_Loaded;

        LanguageDictionaryItems = Localizer
            .Get()
            .GetCurrentLanguageDictionary()
            .GetItems()
            .ToList();

        this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
    }

    private string Namespace { get; } = typeof(ShellPage).Namespace ?? string.Empty;

    private List<LanguageItem> AvailableLanguages { get; set; }

    private List<LanguageDictionaryItem> LanguageDictionaryItems { get; set; } = [];

    private void LanguagesSplitButton_Click(SplitButton sender, SplitButtonClickEventArgs args)
    {
        Localizer.Get().SetLanguage(Localizer.Get().GetCurrentLanguage());
    }

    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.NavigationViewControl.SettingsItem is not NavigationViewItem settingsItem)
        {
            return;
        }

        Uids.SetUid(settingsItem, "MainWindow_NavigationView_Settings");
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected is true)
        {
            _ = this.ContentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItem is NavigationViewItem item &&
            item.Tag is string pageName &&
            Type.GetType($"{Namespace}.{pageName}") is Type pageType)
        {
            _ = this.ContentFrame.Navigate(pageType);
        }

        if (this.ContentFrame.Content is IHasLocalizedItem page)
        {
            page.LocalizedItemPointerEntered -= LocalizedItem_PointerEntered;
            page.LocalizedItemPointerEntered += LocalizedItem_PointerEntered;
        }
    }

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not DependencyObject dependencyObject ||
            Uids.GetUid(dependencyObject) is not string uid ||
            LanguageDictionaryItems.Where(x => x.Uid == uid).FirstOrDefault() is not LanguageDictionaryItem item)
        {
            return;
        }

        this.LanguageDictionaryDataGridControl.SelectedItem = item;
        this.LanguageDictionaryDataGridControl.ScrollIntoView(item, null);
        e.Handled = true;
    }

    private void LanguagesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is not LanguageItem languageItem)
        {
            return;
        }

        Localizer.Get().SetLanguage(languageItem.Language);
        LanguageDictionaryItems = Localizer
            .Get()
            .GetCurrentLanguageDictionary()
            .GetItems()
            .ToList();
        this.LanguagesSplitButton.Content = Localizer.Get().GetLocalizedString(languageItem.Language);
        this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
        this.LanguagesSplitButton.Flyout.Hide();
    }
}