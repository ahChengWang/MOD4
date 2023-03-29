using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Utility.Helper
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static List<T> GetEnumValue<T>() where T : struct
        {
            List<T> tValues = new List<T>();
            foreach (T tValue in Enum.GetValues(typeof(T)))
            {
                tValues.Add(tValue);
            }
            return tValues;
        }

        public static List<T> GetEnumValue<T>(Type attributeType)
        {
            Type tType = typeof(T);
            List<T> tValues = new List<T>();
            foreach (T tValue in Enum.GetValues(tType))
            {
                MemberInfo tMemberInfo = tType.GetMember(tValue.ToString())[0];
                if (Attribute.IsDefined(tMemberInfo, attributeType))
                    tValues.Add(tValue);
            }
            return tValues;
        }

        //public static bool CheckPermission<T>(this List<T> value, MenuEnum menuId, PermissionEnum permissionId) where T : AccountMenuInfoEntity
        //{
        //    return Convert.ToBoolean(value.FirstOrDefault(menu => menu.MenuSn == menuId).AccountPermission & (int)permissionId);
        //}

        //public static bool CheckPermission<T, R, Q>(this List<T> value, R menuId, Q permissionId)
        //{
        //    TypeDescriptor.GetConverter(typeof(int));

        //    var _tt = value.FirstOrDefault(menu => (int)typeof(T).GetProperty("MenuSn").GetValue(menu) == 1);

        //    return Convert.ToBoolean(value.FirstOrDefault(menu => (int)typeof(T).GetProperty("MenuSn").GetValue(menu) == menuId).AccountPermission & (int)permissionId);
        //}
    }
}
