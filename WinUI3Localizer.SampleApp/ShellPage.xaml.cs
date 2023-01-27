using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUI3Localizer.SampleApp;

public sealed partial class ShellPage : Page
{
    public ShellPage()
    {
        InitializeComponent();

        AvailableLanguages = Localizer.Get().GetAvailableLanguages()
            .Select(x => new LanguageItem(Language: x, UidKey: $"{nameof(MainWindow)}_{x}"))
            .ToList();

        this.LanguagesComboBox.SelectedItem = AvailableLanguages
            .FirstOrDefault(x => x.Language == Localizer.Get().GetCurrentLanguage());

        this.NavigationViewControl.Loaded += NavigationViewControl_Loaded;

        LanguageDictionaryItems = Localizer
            .Get()
            .GetCurrentLanguageDictionary()
            .GetItems()
            .ToList();

        this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
    }

    private List<LanguageItem> AvailableLanguages { get; set; }

    private List<LanguageDictionary.Item> LanguageDictionaryItems { get; set; } = new();

    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        if (this.NavigationViewControl.SettingsItem is NavigationViewItem settingsItem)
        {
            Uids.SetUid(settingsItem, "MainWindow_NavigationView_Settings");
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected is true)
        {
            _ = this.ContentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItem is NavigationViewItem item &&
            item.Tag is string pageName &&
            Type.GetType("WinUI3Localizer.SampleApp." + pageName) is Type pageType)
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
        if (sender is DependencyObject dependencyObject)
        {
            string uid = Uids.GetUid(dependencyObject);
            if (LanguageDictionaryItems.Where(x => x.Uid == uid).FirstOrDefault() is LanguageDictionary.Item item)
            {
                this.LanguageDictionaryDataGridControl.SelectedItem = item;
                this.LanguageDictionaryDataGridControl.ScrollIntoView(item, null);
                e.Handled = true;
            }
        }
    }

    private async void LanguagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is LanguageItem languageItem)
        {
            await Localizer.Get().SetLanguage(languageItem.Language);
            LanguageDictionaryItems = Localizer
                .Get()
                .GetCurrentLanguageDictionary()
                .GetItems()
                .ToList();
            this.LanguageDictionaryDataGridControl.ItemsSource = LanguageDictionaryItems;
        }
    }
}
