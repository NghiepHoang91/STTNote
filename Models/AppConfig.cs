using STTNote.Enums;

namespace STTNote.Models
{
    public class AppConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsShowOnUI { get; set; }
        public SettingType SettingType { get; set; }
    }
}