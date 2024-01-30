using System;

namespace STTNote.Models
{
    public class Profile
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public Profile()
        {
            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
        }
    }
}