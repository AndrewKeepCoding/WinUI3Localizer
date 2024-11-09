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

    private readonly Dictionary<string, LanguageDictionary> languageDictionaries = new();

    private readonly List<LocalizationActions.ActionItem> localizationActions = new();

    internal Localizer(Options options)
    {
        this.options = options;

        if (this.options.DisableDefaultLocalizationActions is false)
        {
            this.localizationActions = LocalizationActions.DefaultActions;
        }

        Uids.DependencyObjectUidSet += Uids_DependencyObjectUidSet;
    }

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    private static ILocalizer Instance { get; set; } = NullLocalizer.Instance;

    private static ILogger Logger { get; set; } = NullLogger.Instance;

    private LanguageDictionary CurrentDictionary { get; set; } = new("");

    private LanguageDictionary DefaultDictionary { get; set; } = new("");

    public static ILocalizer Get() => Instance;

    public void AddLanguageDictionary(LanguageDictionary languageDictionary)
    {
        if (this.languageDictionaries.TryGetValue(
            languageDictionary.Language,
            out LanguageDictionary? targetDictionary) is true)
        {
            int previousItemsCount = targetDictionary.GetItemsCount();

            foreach (LanguageDictionaryItem item in languageDictionary.GetItems())
            {
                targetDictionary.AddItem(item);
            }

            Logger.LogInformation("Merged dictionaries. [Language: {Language} Items: {PreviousItemsCount} -> {CurrentItemsCount}]",
                targetDictionary.Language, previousItemsCount, targetDictionary.GetItemsCount());

            return;
        }

        LanguageDictionary newDictionary = new(languageDictionary.Language);

        foreach (LanguageDictionaryItem item in languageDictionary.GetItems())
        {
            newDictionary.AddItem(item);
        }

        this.languageDictionaries.Add(newDictionary.Language, newDictionary);
        Logger.LogInformation("Added new dictionary. [Language: {Language} Items: {ItemsCount}]",
            newDictionary.Language, newDictionary.GetItemsCount());
    }

    public IEnumerable<string> GetAvailableLanguages()
    {
        try
        {
            return this.languageDictionaries
                .Values
                .Select(x => x.Language)
                .ToArray();
        }
        catch (Exception exception)
        {
            FailedToGetAvailableLanguagesException localizerException = new(innerException: exception);
            Logger.LogError(localizerException, localizerException.Message);
            throw localizerException;
        }
    }

    public string GetCurrentLanguage() => CurrentDictionary.Language;

    public void SetLanguage(string language)
    {
        string previousLanguage = CurrentDictionary.Language;

        try
        {
            if (this.languageDictionaries.TryGetValue(
                language,
                out LanguageDictionary? dictionary) is true &&
                dictionary is not null)
            {
                CurrentDictionary = dictionary;
                LocalizeDependencyObjects();
                OnLanguageChanged(previousLanguage, CurrentDictionary.Language);
                return;
            }
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
            if (this.languageDictionaries.TryGetValue(
                GetCurrentLanguage(),
                out LanguageDictionary? dictionary) is true &&
                dictionary?.TryGetItems(
                    uid,
                    out LanguageDictionary.Items? items) is true &&
                    items.LastOrDefault() is LanguageDictionaryItem item)
            {
                return item.Value;
            }
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

        return string.Empty;
    }

    public IEnumerable<string> GetLocalizedStrings(string uid)
    {
        try
        {
            if (this.languageDictionaries.TryGetValue(
                GetCurrentLanguage(),
                out LanguageDictionary? dictionary) is true &&
                dictionary?.TryGetItems(
                    uid,
                    out LanguageDictionary.Items? items) is true)
            {
                return items.Select(x => x.Value);
            }
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

        return Array.Empty<string>();
    }

    public LanguageDictionary GetCurrentLanguageDictionary() => CurrentDictionary;

    public IEnumerable<LanguageDictionary> GetLanguageDictionaries() => this.languageDictionaries.Values;

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
        LocalizeDependencyObject(dependencyObject);
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
                .Select(x => x.GetProperty(attachedPropertyName))
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
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name == name);
    }

    private void LocalizeDependencyObjects()
    {
        foreach (DependencyObject dependencyObject in this.dependencyObjectsReferences.GetDependencyObjects())
        {
            LocalizeDependencyObject(dependencyObject);
        }
    }

    private void LocalizeDependencyObject(DependencyObject dependencyObject)
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

        if (CurrentDictionary.TryGetItems(uid, out LanguageDictionary.Items? items) is true ||
            DefaultDictionary.TryGetItems(uid, out items) is true)
        {
            foreach (LanguageDictionaryItem item in items)
            {
                LocalizeDependencyObject(dependencyObject, uidDependencyPropertyName ?? item.DependencyPropertyName, item.Value);
            }

            return;
        }

        Logger.LogWarning("DependencyObject does not have Uid in the dictionary. [Type: {Type} Uid: {Uid}]", dependencyObject.GetType(), uid);
    }

    private void LocalizeDependencyObject(DependencyObject dependencyObject, string dependencyPropertyName, string value)
    {
        if (GetDependencyProperty(
            dependencyObject,
            dependencyPropertyName) is DependencyProperty dependencyProperty)
        {
            LocalizeDependencyObjectsWithDependencyProperty(dependencyObject, dependencyProperty, value);
            return;
        }

        LocalizeDependencyObjectsWithoutDependencyProperty(dependencyObject, value);
    }

    private void LocalizeDependencyObjectsWithDependencyProperty(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string value)
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
            throw new FailedToConertValueException(
                Uids.GetUid(dependencyObject),
                propertyType,
                value,
                innerException: exception);
        }
    }

    private void LocalizeDependencyObjectsWithoutDependencyProperty(DependencyObject dependencyObject, string value)
    {
        foreach (LocalizationActions.ActionItem item in this.localizationActions
            .Where(x => x.TargetType == dependencyObject.GetType()))
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