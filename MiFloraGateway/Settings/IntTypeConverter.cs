using System;

namespace MiFloraGateway
{
    public class IntTypeConverter : ITypeConverter
    {
        public Type Type => typeof(int);

        public object? ConvertFromString(string? value)
        {
            if (value == null)
                return null;
            return int.Parse(value);
        }

        public string? ConvertToString(object? value)
        {
            return value?.ToString();
        }
    }
}
