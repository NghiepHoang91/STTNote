using STTNote.ActionResults;
using STTNote.Models;
using STTNote.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STTNote.Wrappers
{
    public class NoteProfileWrapper
    {
        public DatabaseService DatabaseService { get; set; }

        public NoteProfileWrapper()
        {
            DatabaseService = DatabaseService.Instance;
        }

        public static NoteProfileWrapper Instance
        {
            get { return new NoteProfileWrapper(); }
        }

        public ActionResult Insert(NoteProfile noteProfile)
        {
            return DatabaseService.Insert(noteProfile);
        }

        public ActionResult Update(NoteProfile noteProfile)
        {
            return DatabaseService.Update(noteProfile);
        }

        public ActionResult<List<string>> GetNoteIdsByProfile(string profileId)
        {
            var profileNotes = DatabaseService.GetAll<NoteProfile>();
            var noteIdList = profileNotes.ReturnValue.Where(n => n.ProfileId.Equals(profileId))
                .Select(n => n.NoteId)
                .Distinct()
                .ToList();

            var result = ActionResult<List<string>>.Success;
            result.ReturnValue = noteIdList;

            return result;
        }
    }
}