using System.Collections.Generic;
using System.Linq;

namespace WinUI3Localizer;

public partial class LanguageDictionary(string language, string name)
{
    // Key: Uid
    private readonly Dictionary<string, Items> dictionary = [];

    public string Language { get; } = language;

    public string Name { get; } = name;

    public int Count => this.dictionary.Values.Sum(items => items.Count);

    // The priority of the dictionary. The higher the value, the higher the priority. The default value is 0.
    public int Priority { get; set; } = 0;

    public void AddItem(LanguageDictionaryItem item)
    {
        if (this.dictionary.ContainsKey(item.Uid) is true)
        {
            this.dictionary[item.Uid].Add(item);
        }
        else
        {
            this.dictionary[item.Uid] = [item];
        }
    }

    public IEnumerable<LanguageDictionaryItem> GetItems()
    {
        IEnumerable<LanguageDictionaryItem> items = this.dictionary.Values.SelectMany(item => item);
        return [.. items];
    }

    public partial class Items : List<LanguageDictionaryItem>, IEnumerable<LanguageDictionaryItem> { }
}