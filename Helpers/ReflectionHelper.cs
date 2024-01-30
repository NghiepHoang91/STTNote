using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace STTNote.Helpers
{
    public static class ReflectionHelper
    {
        public static T? GetValueByName<T>(this object obj, string propertyName)
        {
            var bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(propertyName, bindingFlag);
            var prop = obj.GetType().GetProperty(propertyName, bindingFlag);

            if (field != null)
            {
                var value = field.GetValue(obj);
                if (value is T clearValue)
                {
                    return clearValue;
                }
            }
            else if (prop != null)
            {
                var value = prop.GetValue(obj);
                if (value is T clearValue)
                {
                    return clearValue;
                }
            }

            return default(T);
        }

        public static void SetValueByName<T>(this object obj, string propertyName, T? value)
        {
            var bindingFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(propertyName, bindingFlag);
            var prop = obj.GetType().GetProperty(propertyName, bindingFlag);
            var valueType = value?.GetType()?.Name;

            if (field != null)
            {
                var fieldType = field.FieldType.Name;
                if (valueType?.Equals(fieldType) == true || (field.IsNullable() && value == null))
                {
                    field.SetValue(obj, value);
                }
            }
            else if (prop != null)
            {
                var propType = prop.PropertyType.Name;
                if (valueType?.Equals(propType) == true || (prop.IsNullable() && value == null))
                {
                    prop.SetValue(obj, value);
                }
            }
        }

        public static bool IsNullable(this object? obj)
        {
            if (obj == null) return true;
            return Nullable.GetUnderlyingType(obj.GetType()) != null;
        }

        public static List<MemberInfo> GetAttributes(this object obj)
        {
            var fieldList = obj.GetType().GetFields();
            var propList = obj.GetType().GetProperties();
            var result = new List<MemberInfo>();

            foreach (var field in fieldList)
            {
                result.Add(field);
            }

            foreach (var prop in propList)
            {
                result.Add(prop);
            }

            return result;
        }

        public static Dictionary<string, object> GetValues(this object obj)
        {
            var fieldList = obj.GetType().GetFields();
            var propList = obj.GetType().GetProperties();
            var result = new Dictionary<string, object>();

            foreach (var field in fieldList)
            {
                result[field.Name] = field?.GetValue(obj);
            }

            foreach (var prop in propList)
            {
                result[prop.Name] = prop?.GetValue(obj);
            }

            return result;
        }

        public static object? CreateClassInstanceByName(string className, params object[] constructorParameters)
        {
            Type? type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType($"{assembly.GetName().Name}.Models.{className}");
                if (type != null)
                {
                    return Activator.CreateInstance(type, constructorParameters);
                }
            }

            return null;
        }
    }
}