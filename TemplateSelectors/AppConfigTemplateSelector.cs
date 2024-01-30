using STTNote.Models;
using System.Windows;
using System.Windows.Controls;

namespace STTNote.TemplateSelectors
{
    public class AppConfigTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AppConfig info)
            {
                switch (info.SettingType)
                {
                    case Enums.SettingType.SingleText: return (DataTemplate)((FrameworkElement)container).FindResource("SingleTextTemplate");
                    //case Enums.SettingType.RichText: return (DataTemplate)((FrameworkElement)container).FindResource("");
                    case Enums.SettingType.FilePath: return (DataTemplate)((FrameworkElement)container).FindResource("FileSelectTemplate");
                    case Enums.SettingType.FolderPath: return (DataTemplate)((FrameworkElement)container).FindResource("FolderSelectTemplate");
                    case Enums.SettingType.Checkbox: return (DataTemplate)((FrameworkElement)container).FindResource("CheckBoxTemplate");
                    //case Enums.SettingType.RadioButton: return (DataTemplate)((FrameworkElement)container).FindResource("");
                    //case Enums.SettingType.DropBox: return (DataTemplate)((FrameworkElement)container).FindResource("");
                    case Enums.SettingType.PasswordBox: return (DataTemplate)((FrameworkElement)container).FindResource("PasswordBoxTemplate");
                }
            }

            return null;
        }
    }
}