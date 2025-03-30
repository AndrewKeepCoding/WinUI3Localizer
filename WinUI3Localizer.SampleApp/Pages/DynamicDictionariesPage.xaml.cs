using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace WinUI3Localizer.SampleApp.Pages;

public sealed partial class DynamicDictionariesPage : Page
{
    private readonly ILocalizer localizer;

    public DynamicDictionariesPage()
    {
        InitializeComponent();
        this.localizer = Localizer.Get();
        Loaded += DynamicDictionariesPage_Loaded;

        ILocalizer localizer = Localizer.Get();
        string currentLanguage = localizer.GetCurrentLanguage();

        LanguageDictionary currentDictionary = localizer.GetLanguageDictionaries(currentLanguage).First();

        LanguageDictionaryItem newItem = new(
            uid: "TestPage_Button",
            dependencyPropertyName: "Content",
            stringResourceItemName: "TestPage_Button.Content",
            value: "Test Value");

        LanguageDictionaryItem targetItem = currentDictionary
            .GetItems()
            .First(item => item.Uid == "TestPage_Button");
        targetItem.Value = "New Test Value";
        currentDictionary.AddItem(newItem);
    }

    private void DynamicDictionariesPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.LanguageTextBox.Text = this.localizer.GetCurrentLanguage();
        this.NameTextBox.Text = "DynamicDictionary";
        this.UidTextBox.Text = "MainWindow_AppTitleBar";
        this.DependencyPropertyNameTextBox.Text = "Text";
        this.ValueTextBox.Text = "Dynamic WinUI3 Localizer Sample App🤩";

        LanguageDictionary[] dictionaries = this.localizer.GetLanguageDictionaries();
        this.LanguageDictionaryDataGrid.ItemsSource = dictionaries;
    }

    private void AddDictionaryButton_Click(object sender, RoutedEventArgs e)
    {
        LanguageDictionary newDictionary = new(this.LanguageTextBox.Text, this.NameTextBox.Text)
        {
            Priority = (int)this.PriorityNumberBox.Value,
        };

        _ = this.localizer.AddLanguageDictionary(newDictionary);
        LanguageDictionary[] dictionaries = this.localizer.GetLanguageDictionaries();
        this.LanguageDictionaryDataGrid.ItemsSource = dictionaries;
    }

    private void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.LanguageDictionaryDataGrid.SelectedItem is not LanguageDictionary dictionary)
        {
            return;
        }

        LanguageDictionaryItem item = new(
            this.UidTextBox.Text,
            this.DependencyPropertyNameTextBox.Text,
            stringResourceItemName: $"{this.UidTextBox.Text}.{this.DependencyPropertyNameTextBox.Text}",
            this.ValueTextBox.Text);
        dictionary.AddItem(item);
        LanguageDictionary[] dictionaries = this.localizer.GetLanguageDictionaries();
        this.LanguageDictionaryDataGrid.ItemsSource = dictionaries;

        this.localizer.SetLanguage(this.localizer.GetCurrentLanguage());
    }
}