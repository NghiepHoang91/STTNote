using System;

namespace STTNote.Models
{
    public class Note
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Password { get; set; }
        public string NoteStatusCode { get; set; }
        public DateTime CreateDate { get; set; }

        public Note()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}