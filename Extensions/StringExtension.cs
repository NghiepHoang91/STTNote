using System;
using System.Text.RegularExpressions;

namespace STTNote.Extensions
{
    public static class StringExtension
    {
        public static bool IsNumeric(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue)) return false;

            string numericPattern = @"^-?[0-9]+((?:\.[0-9]+)?|(?:\,[0-9]+)*)$";
            return Regex.IsMatch(stringValue, numericPattern);
        }

        public static int? ToInt32(this string? data)
        {
            if (int.TryParse(data, out int parsedInt))
            {
                return parsedInt;
            }

            try
            {
                var doubleNumber = Convert.ToDouble(data);
                return Convert.ToInt32(doubleNumber);
            }
            catch
            { return null; }
        }
    }
}