using System;

namespace WinUI3Localizer;

public class FailedToConertValueException : LocalizerException
{
    public FailedToConertValueException(string uid, Type propertyType, string value, string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        Uid = uid;
        PropertyType = propertyType;
        Value = value;
    }

    public string Uid { get; }

    public Type PropertyType { get; }

    public string Value { get; }
}