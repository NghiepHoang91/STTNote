using STTNote.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using STTNote.Extensions;

namespace STTNote.Converters
{
    public class CheckboxConverter : MarkupExtension, IValueConverter
    {
        private static CheckboxConverter _instance;

        public static CheckboxConverter GetInstance()
        {
            return _instance ??= new CheckboxConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (Enum.TryParse<NoteStatus>(value.ToString(), out NoteStatus noteStatus))
            {
                return noteStatus == NoteStatus.Archived;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            return value?.ToString()?.ToBoolean() == true ? NoteStatus.Archived : NoteStatus.New;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }
    }
}