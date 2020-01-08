using System;

namespace MiFloraGateway
{
    public class BooleanTypeConverter : ITypeConverter
    {
        public Type Type => typeof(bool);

        public object ConvertFromString(string value)
        {
            return bool.Parse(value);
        }

        public string ConvertToString(object value)
        {
            return value.ToString();
        }
    }
}
