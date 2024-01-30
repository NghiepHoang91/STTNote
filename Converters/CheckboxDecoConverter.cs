using STTNote.Const;
using STTNote.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class CheckboxDecoConverter : MarkupExtension, IValueConverter
    {
        private static CheckboxDecoConverter _instance;

        public static CheckboxDecoConverter GetInstance()
        {
            return _instance ??= new CheckboxDecoConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextDecorationCollection? fontStyle = null;
            if (value == null)
                return fontStyle;

            if (Enum.TryParse<NoteStatus>(value.ToString(), out NoteStatus noteStatus))
            {
                if (noteStatus == NoteStatus.Archived) fontStyle = System.Windows.TextDecorations.Strikethrough;
            }

            return fontStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.FontStyles.Normal;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetInstance();
        }
    }
}