using System;

namespace MiFloraGateway
{
    public enum StringSettingType
    {
        Normal,
        Cron,
        IPAddressOrHostname,
        Password
    }


    [AttributeUsage(AttributeTargets.Field)]
    public class StringSettingAttribute : SettingAttribute
    {
        public StringSettingAttribute(bool isRequired, object defaultValue, bool exposeValueToClient, StringSettingType stringType = StringSettingType.Normal) :
            base(typeof(string), isRequired, defaultValue, exposeValueToClient)
        {
            StringType = stringType;
        }
        public StringSettingType StringType { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute(Type type, bool isRequired, object defaultValue, bool exposeValueToClient)
        {
            Type = type;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
            ExposeValueToClient = exposeValueToClient;
        }

        public Type Type { get; }
        public bool IsRequired { get; }
        public object DefaultValue { get; }
        public bool ExposeValueToClient { get; }
    }
}
