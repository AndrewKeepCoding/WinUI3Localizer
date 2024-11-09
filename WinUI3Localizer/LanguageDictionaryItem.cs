namespace WinUI3Localizer;

public class LanguageDictionaryItem(string uid, string dependencyPropertyName, string stringResourceItemName, string value)
{
    public string Uid { get; } = uid;

    public string DependencyPropertyName { get; } = dependencyPropertyName;

    public string StringResourceItemName { get; } = stringResourceItemName;

    public string Value { get; set; } = value;
}