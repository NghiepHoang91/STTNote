using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace STTNote.Extensions
{
    public static class ConvertExtension
    {
        public static bool? ToBoolean(this string? data)
        {
            var supportValues = new Dictionary<string, bool>
            {
                { "0", false },
                { "1", true },
                { "true", true },
                { "false", false }
            };

            return supportValues.GetValue(data?.ToString()?.ToLower() ?? string.Empty);
        }

        public static string ToBase64(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static T FromBase64<T>(this string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return (T)new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}