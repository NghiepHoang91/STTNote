using STTNote.Const;
using STTNote.Enums;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace STTNote.Converters
{
    public class CheckboxColorConverter : MarkupExtension, IValueConverter
    {
        private static CheckboxColorConverter _instance;

        public static CheckboxColorConverter GetInstance()
        {
            return _instance ??= new CheckboxColorConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Consts.CHECKBOX_COLOR_NEW;

            if (Enum.TryParse<NoteStatus>(value.ToString(), out NoteStatus noteStatus))
            {
                if (noteStatus == NoteStatus.Archived) return Consts.CHECKBOX_COLOR_ARCHIVED;

                return Consts.CHECKBOX_COLOR_NEW;
            }

            return Consts.CHECKBOX_COLOR_NEW;
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