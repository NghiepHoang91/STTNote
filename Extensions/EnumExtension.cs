using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace STTNote.Extensions
{
    public static class EnumExtension
    {
        public static string Name(this Enum @enum)
        {
            if (@enum == null)
            {
                return string.Empty;
            }

            return @enum
                .GetType()
                .GetMember(@enum.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName() ?? @enum.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string memberValue) where TEnum : struct
        {
            Enum.TryParse(memberValue, true, out TEnum result);
            return result;
        }
    }
}