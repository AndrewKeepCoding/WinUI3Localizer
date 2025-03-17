using System;

namespace WinUI3Localizer;

public class LanguageChangedEventArgs(string previousLanguage, string currentLanguage) : EventArgs
{
    public string PreviousLanguage { get; } = previousLanguage;

    public string CurrentLanguage { get; } = currentLanguage;
}