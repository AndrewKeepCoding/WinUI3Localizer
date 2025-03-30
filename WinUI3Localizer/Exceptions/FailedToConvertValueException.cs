using System;

namespace WinUI3Localizer;

public class FailedToConvertValueException(string uid, Type propertyType, string value, string? message = null, Exception? innerException = null)
    : LocalizerException(message, innerException)
{
    public string Uid { get; } = uid;

    public Type PropertyType { get; } = propertyType;

    public string Value { get; } = value;
}