using System;

namespace WinUI3Localizer;

public class NullLocalizer : ILocalizer
{
    private NullLocalizer() { }

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged { add { } remove { } }

    public event EventHandler<LanguageDictionaryAddedEventArgs>? LanguageDictionaryAdded { add { } remove { } }

    public event EventHandler<LanguageDictionaryRemovedEventArgs>? LanguageDictionaryRemoved { add { } remove { } }

    public static ILocalizer Instance { get; } = new NullLocalizer();

    public bool AddLanguageDictionary(LanguageDictionary languageDictionary) => false;

    public bool RemoveLanguageDictionary(LanguageDictionary languageDictionary) => false;

    public LanguageDictionary[] GetLanguageDictionaries(string _ = "") => [];

    public string[] GetAvailableLanguages() => [];

    public string GetCurrentLanguage() => string.Empty;

    public void SetLanguage(string language) { }

    public string GetLocalizedString(string uid) => uid;

    public string[] GetLocalizedStrings(string uid) => [uid];
}