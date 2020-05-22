using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaHuaCaoCao.Example
{
    public static class Enum<TEnum> where TEnum : Enum
    {

        /// <summary>
        /// Retrieves an array of the values of the constants in a specified enumeration.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static TEnum[] GetValues()
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static TEnum Parse(string name)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), name);
        }

        public static TAttribute GetRequiredAttribute<TAttribute>(TEnum value) where TAttribute : Attribute
        {
            return (TAttribute)(typeof(TEnum).GetMember(value.ToString()).Single().GetCustomAttributes(typeof(TAttribute), false).Single());
        }
        public static TAttribute GetAttribute<TAttribute>(TEnum value) where TAttribute : Attribute
        {
            return (TAttribute)(typeof(TEnum).GetMember(value.ToString()).Single().GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault());
        }

    }

    public static class EnumExtensionMethods
    {

        public static TAttribute GetAttribute<TEnum, TAttribute>(this TEnum value) where TAttribute : Attribute where TEnum : Enum
        {
            return (TAttribute)(typeof(TEnum).GetMember(value.ToString()).Single().GetCustomAttributes(typeof(TAttribute), false).Single());
        }
    }
}
