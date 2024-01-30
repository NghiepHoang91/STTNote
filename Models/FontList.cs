using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace STTNote.Models
{
    internal class FontList : ObservableCollection<string>
    {
        public FontList()
        {
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                var fontName = font.ToString();
                if (!this.Contains(fontName))
                {
                    this.Add(fontName);
                }
            }
        }
    }
}