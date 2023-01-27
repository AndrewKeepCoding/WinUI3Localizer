using Microsoft.UI.Xaml;
using System;

namespace WinUI3Localizer;

public static class Uids
{
    public static readonly DependencyProperty UidProperty = DependencyProperty.RegisterAttached(
        "Uid",
        typeof(string),
        typeof(Uids),
        new PropertyMetadata(default));

    /// <summary>
    /// This static event is meant only for the Localizer 
    /// so it can have access to DependencyObjects with Uid.
    /// </summary>
    internal static event EventHandler<DependencyObject>? DependencyObjectUidSet;

    public static string GetUid(DependencyObject dependencyObject)
    {
        return (string)dependencyObject.GetValue(UidProperty);
    }

    public static void SetUid(DependencyObject dependencyObject, string uid)
    {
        dependencyObject.SetValue(UidProperty, uid);
        DependencyObjectUidSet?.Invoke(null, dependencyObject);
    }
}