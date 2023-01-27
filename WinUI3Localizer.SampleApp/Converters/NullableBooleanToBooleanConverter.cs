using Microsoft.UI.Xaml.Data;
using System;

namespace WinUI3Localizer.SampleApp.Converters;

public class NullableBooleanToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue && boolValue is true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}