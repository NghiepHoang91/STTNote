using STTNote.Const;
using STTNote.Enums;
using STTNote.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class PasswordBoxColorConverter : MarkupExtension, IValueConverter
    {
        private static PasswordBoxColorConverter _instance;

        public static PasswordBoxColorConverter GetInstance()
        {
            return _instance ??= new PasswordBoxColorConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return SystemColors.InfoBrushKey;

            var password = string.Empty;
            if (value is Note note)
            {
                password = note?.Password ?? string.Empty;
            }

            return string.IsNullOrEmpty(password) ? SystemColors.InfoBrushKey : SystemColors.ControlBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SystemColors.InfoBrushKey;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }
    }
}