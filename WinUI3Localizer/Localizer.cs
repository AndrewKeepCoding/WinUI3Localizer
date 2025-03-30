using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WinUI3Localizer;

public sealed partial class Localizer : ILocalizer
{
    private readonly Options options;

    private readonly DependencyObjectWeakReferences dependencyObjectsReferences = new();

    private readonly HashSet<LanguageDictionary> allLanguageDictionaries = [];

    private readonly List<LanguageDictionary> currentLanguageDictionaries = [];

    private readonly List<LocalizationActions.ActionItem> localizationActions = [];

    internal Localizer(Options options)
    {
        this.options = options;
        CurrentLanguage = options.DefaultLanguage;

        if (this.options.DisableDefaultLocalizationActions is false)
        {
            this.localizationActions = LocalizationActions.DefaultActions;
        }

        Uids.DependencyObjectUidSet += Uids_DependencyObjectUidSet;
    }

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    public event EventHandler<LanguageDictionaryAddedEventArgs>? LanguageDictionaryAdded;

    public event EventHandler<LanguageDictionaryRemovedEventArgs>? LanguageDictionaryRemoved;

    private static ILocalizer Instance { get; set; } = NullLocalizer.Instance;

    private static ILogger Logger { get; set; } = NullLogger.Instance;

    private string CurrentLanguage { get; set; }

    private LanguageDictionary DefaultDictionary { get; set; } = new(language: "", name: "");

    public static ILocalizer Get() => Instance;

    public bool AddLanguageDictionary(LanguageDictionary languageDictionary)
    {
        if (this.allLanguageDictionaries.Add(languageDictionary) is false)
        {
            Logger.LogWarning("LanguageDictionary already exists. [Language: {Language} Name: {Name}]", languageDictionary.Language, languageDictionary.Name);
            return false;
        }

        this.currentLanguageDictionaries.Clear();
        LanguageDictionaryAdded?.Invoke(this, new LanguageDictionaryAddedEventArgs(languageDictionary));
        Logger.LogInformation("Added new dictionary. [Language: {Language} Items: {ItemsCount} Name: {Name}]", languageDictionary.Language, languageDictionary.Count, languageDictionary.Name);
        return true;
    }

    public bool RemoveLanguageDictionary(LanguageDictionary languageDictionary)
    {
        if (this.allLanguageDictionaries.Remove(languageDictionary) is false)
        {
            Logger.LogWarning("LanguageDictionary does not exist. [Language: {Language} / Name: {Name}]", languageDictionary.Language, languageDictionary.Name);
            return false;
        }

        this.currentLanguageDictionaries.Clear();
        LanguageDictionaryRemoved?.Invoke(this, new LanguageDictionaryRemovedEventArgs(languageDictionary));
        Logger.LogInformation("Removed dictionary. [Language: {Language} / Name: {Name}]", languageDictionary.Language, languageDictionary.Name);
        return true;
    }

    public LanguageDictionary[] GetLanguageDictionaries(string language = "")
    {
        IEnumerable<LanguageDictionary> dictionaries = language == string.Empty
            ? this.allLanguageDictionaries
            : this.allLanguageDictionaries.Where(dictionary => dictionary.Language == language);
        return [.. dictionaries.OrderByDescending(dictionary => dictionary.Priority)];
    }

    public string[] GetAvailableLanguages()
    {
        try
        {
            IEnumerable<string> languages = this.allLanguageDictionaries
                .Select(dictionary => dictionary.Language)
                .Distinct();
            return [.. languages];
        }
        catch (Exception exception)
        {
            FailedToGetAvailableLanguagesException localizerException = new(innerException: exception);
            Logger.LogError(localizerException, localizerException.Message);
            throw localizerException;
        }
    }

    public string GetCurrentLanguage() => CurrentLanguage;

    public void SetLanguage(string language)
    {
        string previousLanguage = CurrentLanguage;

        try
        {
            CurrentLanguage = language;
            LocalizeDependencyObjects();
            OnLanguageChanged(previousLanguage, CurrentLanguage);
        }
        catch (LocalizerException)
        {
            throw;
        }
        catch (Exception exception)
        {
            FailedToSetLanguageException localizerException = new(previousLanguage, language, message: string.Empty, innerException: exception);
            Logger.LogError(localizerException, localizerException.Message);
            throw localizerException;
        }
    }

    public string GetLocalizedString(string uid)
    {
        try
        {
            return GetLocalizedStrings(uid).FirstOrDefault() ?? string.Empty;
        }
        catch (LocalizerException)
        {
            throw;
        }
        catch (Exception exception)
        {
            FailedToGetLocalizedStringException localizerException = new(uid, innerException: exception);
            Logger.LogError(localizerException, localizerException.Message);
            throw localizerException;
        }
    }

    public string[] GetLocalizedStrings(string uid)
    {
        try
        {
            return [.. GetLanguageDictionaries(CurrentLanguage)
                .SelectMany(dictionary => dictionary.GetItems())
                .Where(item => item.Uid == uid)
                .Select(item => item.Value)];
        }
        catch (LocalizerException)
        {
            throw;
        }
        catch (Exception exception)
        {
            FailedToGetLocalizedStringException localizerException = new(uid, innerException: exception);
            Logger.LogError(localizerException, localizerException.Message);
            throw localizerException;
        }
    }

    internal static void Set(ILocalizer localizer) => Instance = localizer;

    internal void SetLogger(ILogger logger)
    {
        Logger = logger;

        this.dependencyObjectsReferences.DependencyObjectAdded -= DependencyObjectsReferences_DependencyObjectAdded;
        this.dependencyObjectsReferences.DependencyObjectAdded += DependencyObjectsReferences_DependencyObjectAdded;
        this.dependencyObjectsReferences.DependencyObjectRemoved -= DependencyObjectsReferences_DependencyObjectRemoved;
        this.dependencyObjectsReferences.DependencyObjectRemoved += DependencyObjectsReferences_DependencyObjectRemoved;
    }

    internal void SetDefaultLanguageDictionary(LanguageDictionary languageDictionary)
    {
        DefaultDictionary = languageDictionary;
    }

    internal void AddLocalizationAction(LocalizationActions.ActionItem item)
    {
        this.localizationActions.Add(item);
    }

    internal void RegisterDependencyObject(DependencyObject dependencyObject)
    {
        this.dependencyObjectsReferences.Add(dependencyObject);
        LocalizeDependencyObject(dependencyObject, GetLanguageDictionaries(CurrentLanguage));
    }

    private static void Uids_DependencyObjectUidSet(object? sender, DependencyObject dependencyObject)
    {
        (Localizer.Instance as Localizer)?.RegisterDependencyObject(dependencyObject);
    }

    private static void DependencyObjectsReferences_DependencyObjectAdded(object? sender, DependencyObjectReferenceAddedEventArgs e)
    {
        Logger.LogTrace("Added DependencyObject. [Type: {Type} Total: {Count}]",
            e.AddedItemType,
            e.ItemsTotal);
    }

    private static void DependencyObjectsReferences_DependencyObjectRemoved(object? sender, DependencyObjectReferenceRemovedEventArgs e)
    {
        Logger.LogTrace("Removed DependencyObject. [Type: {Type} Total: {Count}]",
            e.RemovedItemType,
            e.ItemsTotal);
    }

    private static DependencyProperty? GetDependencyProperty(DependencyObject dependencyObject, string dependencyPropertyName)
    {
        Type type = dependencyObject.GetType();

        if (type.GetProperty(
            dependencyPropertyName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) is PropertyInfo propertyInfo &&
            propertyInfo.GetValue(null) is DependencyProperty property)
        {
            return property;
        }

        if (type.GetField(
            dependencyPropertyName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) is FieldInfo fieldInfo &&
            fieldInfo.GetValue(null) is DependencyProperty field)
        {
            return field;
        }

        // TODO: This should be done on the building process.
        if (dependencyPropertyName.Split('.') is string[] splitResult &&
            splitResult.Length is 2)
        {
            string attachedPropertyClassName = splitResult[0];
            IEnumerable<Type> types = GetTypesFromName(attachedPropertyClassName);

            string attachedPropertyName = splitResult[1];
            IEnumerable<PropertyInfo> attachedProperties = types
                .Select(type => type.GetProperty(attachedPropertyName))
                .OfType<PropertyInfo>();

            foreach (PropertyInfo attachedProperty in attachedProperties)
            {
                if (attachedProperty.GetValue(null) is DependencyProperty dependencyProperty)
                {
                    return dependencyProperty;
                }
            }
        }

        return null;
    }

    private static IEnumerable<Type> GetTypesFromName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Name == name);
    }

    private static void LocalizeDependencyObjectsWithDependencyProperty(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string value)
    {
        Type propertyType = dependencyObject
            .GetValue(dependencyProperty)?
            .GetType()
            ?? typeof(object);

        if (propertyType.IsEnum is true &&
            Enum.TryParse(propertyType, value, out object? enumValue) is true)
        {
            dependencyObject.SetValue(dependencyProperty, enumValue);
            return;
        }

        if (propertyType == typeof(string))
        {
            dependencyObject.SetValue(dependencyProperty, value);
            return;
        }

        try
        {
            object convertedValue = XamlBindingHelper.ConvertValue(propertyType, value);
            dependencyObject.SetValue(dependencyProperty, convertedValue);
        }
        catch (Exception exception)
        {
            throw new FailedToConvertValueException(
                Uids.GetUid(dependencyObject),
                propertyType,
                value,
                innerException: exception);
        }
    }

    private void LocalizeDependencyObjects()
    {
        IEnumerable<LanguageDictionary> languageDictionaries = GetLanguageDictionaries(CurrentLanguage);

        foreach (DependencyObject dependencyObject in this.dependencyObjectsReferences.GetDependencyObjects())
        {
            LocalizeDependencyObject(dependencyObject, languageDictionaries);
        }
    }

    private void LocalizeDependencyObject(DependencyObject dependencyObject, IEnumerable<LanguageDictionary> languageDictionaries)
    {
        if (Uids.GetUid(dependencyObject) is not string uidSource ||
            string.IsNullOrEmpty(uidSource) is true)
        {
            Logger.LogWarning("DependencyObject does not have Uid. [Type: {Type}]", dependencyObject.GetType());
            return;
        }

        string uid = uidSource;
        string? uidDependencyPropertyName = null;

        if (uidSource.Split('.') is { Length: 2 } splitResult)
        {
            uid = splitResult[0];
            uidDependencyPropertyName = splitResult[1] + "Property";
        }

        IEnumerable<LanguageDictionaryItem> items = languageDictionaries
            .SelectMany(dictionary => dictionary
                .GetItems()
                .Where(item => item.Uid == uid));

        if (items.Any() is false)
        {
            items = DefaultDictionary.GetItems().Where(item => item.Uid == uid);
        }

        foreach (LanguageDictionaryItem item in items)
        {
            LocalizeDependencyObject(dependencyObject, uidDependencyPropertyName ?? item.DependencyPropertyName, item.Value);
        }

        if (items.Any() is false)
        {
            Logger.LogWarning("DependencyObject does not have Uid in the dictionary. [Type: {Type} Uid: {Uid}]", dependencyObject.GetType(), uid);
        }
    }

    private void LocalizeDependencyObject(DependencyObject dependencyObject, string dependencyPropertyName, string value)
    {
        if (GetDependencyProperty(
            dependencyObject,
            dependencyPropertyName) is DependencyProperty dependencyProperty)
        {
            Localizer.LocalizeDependencyObjectsWithDependencyProperty(dependencyObject, dependencyProperty, value);
            return;
        }

        LocalizeDependencyObjectsWithoutDependencyProperty(dependencyObject, value);
    }

    private void LocalizeDependencyObjectsWithoutDependencyProperty(DependencyObject dependencyObject, string value)
    {
        foreach (LocalizationActions.ActionItem item in this.localizationActions
            .Where(action => action.TargetType == dependencyObject.GetType()))
        {
            item.Action(new LocalizationActions.ActionArguments(dependencyObject, value));
        }
    }

    private void OnLanguageChanged(string previousLanguage, string currentLanguage)
    {
        LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(previousLanguage, currentLanguage));
        Logger.LogInformation("Changed language. [{PreviousLanguage} -> {CurrentLanguage}]", previousLanguage, currentLanguage);
    }
}