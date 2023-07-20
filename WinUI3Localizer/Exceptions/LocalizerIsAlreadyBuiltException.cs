using System;

namespace WinUI3Localizer;

public class LocalizerIsAlreadyBuiltException : LocalizerException
{
    public LocalizerIsAlreadyBuiltException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}