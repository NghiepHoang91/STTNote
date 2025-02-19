namespace STTNote.Models
{
    public class INISectionItem
    {
        internal INISectionItem()
        {
            IsComment = false;
        }

        public string Section { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsComment { get; set; }
    }
}
