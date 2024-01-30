using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class NoteTitleConverter : MarkupExtension, IValueConverter
    {
        private static NoteTitleConverter _instance;

        public static NoteTitleConverter GetInstance()
        {
            return _instance ??= new NoteTitleConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return string.Empty;

            var titleMaxLength = 50;

            if (value?.ToString()?.Length <= titleMaxLength)
                return value?.ToString() ?? string.Empty;

            var trimString = value?.ToString()?.Substring(0, titleMaxLength) ?? string.Empty;
            return string.IsNullOrEmpty(trimString) ? string.Empty : $"{trimString}...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }
    }
}