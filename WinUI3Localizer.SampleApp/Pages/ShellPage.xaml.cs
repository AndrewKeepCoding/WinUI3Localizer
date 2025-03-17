using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUI3Localizer.SampleApp.Pages;

public sealed partial class ShellPage : Page
{
    private readonly ILocalizer localizer;

    public ShellPage()
    {
        InitializeComponent();

        this.localizer = Localizer.Get();
        this.localizer.LanguageDictionaryAdded += Localizer_LanguageDictionariesUpdated;
        this.localizer.LanguageDictionaryRemoved += Localizer_LanguageDictionariesUpdated;

        AvailableLanguages = [.. this.localizer
            .GetAvailableLanguages()
            .Select(language => new LanguageItem(language, UidKey: $"{nameof(MainWindow)}_{language}"))];

        this.LanguagesGridView.SelectedItem = AvailableLanguages
            .FirstOrDefault(item => item.Language == this.localizer.GetCurrentLanguage());

        this.NavigationViewControl.Loaded += NavigationViewControl_Loaded;

        RefreshLanguageDictionaryItems();
    }

    private void Localizer_LanguageDictionariesUpdated(object? sender, object e)
    {
        RefreshLanguageDictionaryItems();
    }

    private void RefreshLanguageDictionaryItems()
    {
        LanguageDictionaryItems = [.. this.localizer
            .GetLanguageDictionaries(this.localizer.GetCurrentLanguage())
            .SelectMany(dictionary => dictionary.GetItems())];
        this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
    }

    private string Namespace { get; } = typeof(ShellPage).Namespace ?? string.Empty;

    private List<LanguageItem> AvailableLanguages { get; set; }

    private List<LanguageDictionaryItem> LanguageDictionaryItems { get; set; } = [];

    private void LanguagesSplitButton_Click(SplitButton sender, SplitButtonClickEventArgs args)
    {
        Localizer.Get().SetLanguage(this.localizer.GetCurrentLanguage());
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
            LanguageDictionaryItems
                .Where(item => item.Uid == uid)
                .FirstOrDefault() is not LanguageDictionaryItem item)
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

        this.localizer.SetLanguage(languageItem.Language);
        LanguageDictionaryItems = [.. this.localizer
            .GetLanguageDictionaries(this.localizer.GetCurrentLanguage())
            .SelectMany(dictionary => dictionary.GetItems())];
        this.LanguagesSplitButton.Content = this.localizer.GetLocalizedString(languageItem.Language);
        this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
        this.LanguagesSplitButton.Flyout.Hide();
    }
}