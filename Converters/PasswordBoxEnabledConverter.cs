using STTNote.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class PasswordBoxEnabledConverter : MarkupExtension, IValueConverter
    {
        private static PasswordBoxEnabledConverter _instance;

        public static PasswordBoxEnabledConverter GetInstance()
        {
            return _instance ??= new PasswordBoxEnabledConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;

            var password = string.Empty;
            if (value is Note note)
            {
                password = note?.Password ?? string.Empty;
            }

            return string.IsNullOrEmpty(password);
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