using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;

namespace WinUI3Localizer.SampleApp.Pages;

public record Person(int ID, string FirstName, string LastName);

public sealed partial class ItemTemplatesPage : Page, IHasLocalizedItem
{
    public ItemTemplatesPage()
    {
        InitializeComponent();

        int id = 1;
        People.Add(new Person(ID: id++, FirstName: "Ted", LastName: "Mosby"));
        People.Add(new Person(ID: id++, FirstName: "Tracy", LastName: "McConnell"));
        People.Add(new Person(ID: id++, FirstName: "Marshall", LastName: "Eriksen"));
        People.Add(new Person(ID: id++, FirstName: "Lilly", LastName: "Aldrin"));
        People.Add(new Person(ID: id++, FirstName: "Barney", LastName: "Stinson"));
        People.Add(new Person(ID: id++, FirstName: "Robin", LastName: "Scherbatsky"));
    }

    public event PointerEventHandler? LocalizedItemPointerEntered;

    private ObservableCollection<Person> People { get; } = new();

    private void LocalizedItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        LocalizedItemPointerEntered?.Invoke(sender, e);
    }
}