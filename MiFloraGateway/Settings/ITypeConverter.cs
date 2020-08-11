using System;

namespace MiFloraGateway
{
    public interface ITypeConverter
    {
        Type Type { get; }
        string? ConvertToString(object? value);
        object? ConvertFromString(string? value);
    }
}
