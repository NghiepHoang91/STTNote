using STTNote.Models;
using System.Collections.Generic;

namespace STTNote.ActionMessages
{
    public class SaveAppSettingMessage
    {
        public List<AppConfig> Configs { get; set; }
    }
}