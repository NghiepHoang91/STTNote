using System.Collections.ObjectModel;

namespace STTNote.Models
{
    internal class FontHeight : ObservableCollection<string>
    {
        public FontHeight()
        {
            int maxSize = 90;
            for (int size = 8; size <= maxSize; size++)
            {
                this.Add($"{size}");
            }
        }
    }
}