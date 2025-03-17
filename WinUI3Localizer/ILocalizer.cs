using System;

namespace WinUI3Localizer;

public interface ILocalizer
{
    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    event EventHandler<LanguageDictionaryAddedEventArgs>? LanguageDictionaryAdded;

    event EventHandler<LanguageDictionaryRemovedEventArgs>? LanguageDictionaryRemoved;

    bool AddLanguageDictionary(LanguageDictionary languageDictionary);

    bool RemoveLanguageDictionary(LanguageDictionary languageDictionary);

    LanguageDictionary[] GetLanguageDictionaries(string language = "");

    string[] GetAvailableLanguages();

    string GetCurrentLanguage();

    void SetLanguage(string language);

    string GetLocalizedString(string uid);

    string[] GetLocalizedStrings(string uid);
}