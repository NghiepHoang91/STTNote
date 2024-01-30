using STTNote.ActionResults;
using STTNote.Models;
using STTNote.Services;
using System;
using System.Collections.Generic;

namespace STTNote.Wrappers
{
    public class ProfileWrapper
    {
        public DatabaseService DatabaseService { get; set; }

        public ProfileWrapper()
        {
            DatabaseService = DatabaseService.Instance;
        }

        public static ProfileWrapper Instance
        {
            get { return new ProfileWrapper(); }
        }

        public ActionResult InitDefaultProfile()
        {
            var profiles = DatabaseService.GetAll<Profile>().ReturnValue;
            if (profiles?.Count > 0) return ActionResult.Success;

            profiles = new System.Collections.Generic.List<Profile>
            {
                new Profile
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Master"
                }
            };

            profiles.ForEach((profile) =>
            {
                DatabaseService.Insert(profile);
            });

            return ActionResult.Success;
        }

        public ActionResult Insert(Profile profile)
        {
            return DatabaseService.Insert(profile);
        }

        public ActionResult<List<Profile>> Getall()
        {
            return DatabaseService.GetAll<Profile>();
        }
    }
}