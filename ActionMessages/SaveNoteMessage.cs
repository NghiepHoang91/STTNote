using STTNote.Enums;
using STTNote.Models;

namespace STTNote.ActionMessages
{
    public class SaveNoteMessage
    {
        public EditingMode Mode { set; get; }
        public Note Note { set; get; }
    }
}