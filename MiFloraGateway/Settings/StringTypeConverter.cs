using System;

namespace MiFloraGateway
{
    public class StringTypeConverter : ITypeConverter
    {
        public Type Type => typeof(string);

        public object ConvertFromString(string value)
        {
            return value;
        }

        public string ConvertToString(object value)
        {
            return (string)value;
        }
    }
}
