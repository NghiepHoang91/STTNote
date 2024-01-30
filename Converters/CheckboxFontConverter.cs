using STTNote.Const;
using STTNote.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class CheckboxFontConverter : MarkupExtension, IValueConverter
    {
        private static CheckboxFontConverter _instance;

        public static CheckboxFontConverter GetInstance()
        {
            return _instance ??= new CheckboxFontConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontStyle = FontStyles.Normal;
            if (value == null)
                return fontStyle;

            if (Enum.TryParse<NoteStatus>(value.ToString(), out NoteStatus noteStatus) && noteStatus == NoteStatus.Archived)
                fontStyle = FontStyles.Italic;

            return fontStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Consts.CHECKBOX_COLOR_NEW;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }
    }
}