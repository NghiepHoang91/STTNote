using System;

namespace STTNote.Models
{
    public class NoteCheckPoint
    {
        public string NoteId { get; set; }
        public string CheckPointMessage { get; set; }
        public DateTime CheckPointDate { get; set; }
    }
}