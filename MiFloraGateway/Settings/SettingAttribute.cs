using System;

namespace MiFloraGateway
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute(Type type, object defaultValue, bool exposeToClient)
        {
            Type = type;
            DefaultValue = defaultValue;
            ExposeToClient = exposeToClient;
        }

        public Type Type { get; }
        public object DefaultValue { get; }
        public bool ExposeToClient { get; }
    }
}
