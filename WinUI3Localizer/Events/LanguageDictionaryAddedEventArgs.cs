using System;

namespace WinUI3Localizer;

public class LanguageDictionaryAddedEventArgs(LanguageDictionary languageDictionary) : EventArgs
{
    public LanguageDictionary LanguageDictionary { get; } = languageDictionary;
}