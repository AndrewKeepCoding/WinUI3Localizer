using System;
using System.Collections.Generic;

namespace WinUI3Localizer;

public interface ILocalizer
{
    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    IEnumerable<string> GetAvailableLanguages();

    string GetCurrentLanguage();

    void SetLanguage(string language);

    string GetLocalizedString(string uid);

    IEnumerable<string> GetLocalizedStrings(string uid);

    LanguageDictionary GetCurrentLanguageDictionary();

    IEnumerable<LanguageDictionary> GetLanguageDictionaries();
}