using System;

namespace WinUI3Localizer;

public class LanguageDictionaryRemovedEventArgs(LanguageDictionary languageDictionary) : EventArgs
{
    public LanguageDictionary LanguageDictionary { get; } = languageDictionary;
}