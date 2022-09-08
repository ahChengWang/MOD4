using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MOD4.Web
{
    public static class EnumHelper
    {
        public static string GetDescription(this System.Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static List<T> GetEnumValue<T>() where T : struct
        {
            List<T> tValues = new List<T>();
            foreach (T tValue in System.Enum.GetValues(typeof(T)))
            {
                tValues.Add(tValue);
            }
            return tValues;
        }

        public static List<T> GetEnumValue<T>(Type attributeType)
        {
            Type tType = typeof(T);
            List<T> tValues = new List<T>();
            foreach (T tValue in System.Enum.GetValues(tType))
            {
                MemberInfo tMemberInfo = tType.GetMember(tValue.ToString())[0];
                if (Attribute.IsDefined(tMemberInfo, attributeType))
                    tValues.Add(tValue);
            }
            return tValues;
        }
    }
}
