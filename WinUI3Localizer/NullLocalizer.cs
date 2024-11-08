using System;
using System.Collections.Generic;
using System.Linq;

namespace WinUI3Localizer;

public class NullLocalizer : ILocalizer
{
    private NullLocalizer()
    {
    }

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged { add { } remove { } }

    public static ILocalizer Instance { get; } = new NullLocalizer();

    public IEnumerable<string> GetAvailableLanguages() => Array.Empty<string>();

    public string GetCurrentLanguage() => string.Empty;

    public void SetLanguage(string language) { }

    public string GetLocalizedString(string uid) => uid;

    public IEnumerable<string> GetLocalizedStrings(string uid) => new string[] { uid };

    public LanguageDictionary GetCurrentLanguageDictionary() => new("");

    public IEnumerable<LanguageDictionary> GetLanguageDictionaries() => Enumerable.Empty<LanguageDictionary>();
}