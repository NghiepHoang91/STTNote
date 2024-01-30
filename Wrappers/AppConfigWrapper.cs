using STTNote.ActionResults;
using STTNote.Const;
using STTNote.Models;
using STTNote.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace STTNote.Wrappers
{
    public class AppConfigWrapper
    {
        public DatabaseService DatabaseService { get; set; }

        public AppConfigWrapper()
        {
            DatabaseService = DatabaseService.Instance;
        }

        public static AppConfigWrapper Instance
        {
            get { return new AppConfigWrapper(); }
        }

        public ActionResult InitDefaultAppConfig()
        {
            var appConfigs = new System.Collections.Generic.List<AppConfig>
            {
                new AppConfig()
                {
                    Id = Consts.ConfigIds.APP_PASSWORD,
                    Name = "App Password",
                    SettingType = Enums.SettingType.PasswordBox,
                    Value = string.Empty,
                    IsShowOnUI = true,
                },
                new AppConfig()
                {
                    Id = Consts.ConfigIds.RUN_ON_STARTUP,
                    Name = "Run On Startup",
                    SettingType = Enums.SettingType.Checkbox,
                    Value = true,
                    IsShowOnUI = true,
                },
                new AppConfig()
                {
                    Id = Consts.ConfigIds.AUTO_UPDATE,
                    Name = "Auto Update",
                    SettingType = Enums.SettingType.Checkbox,
                    Value = false,
                    IsShowOnUI = true,
                },
                new AppConfig()
                {
                    Id = Consts.ConfigIds.CREATE_DESKTOP_SHORTCUT,
                    Name = "Create Shortcut",
                    SettingType = Enums.SettingType.Checkbox,
                    Value = false,
                    IsShowOnUI = true,
                },
            };

            var configs = DatabaseService.GetAll<AppConfig>().ReturnValue;
            appConfigs.ForEach((config) =>
            {
                if (configs?.Any(c => c.Id.Equals(config.Id)) == true) return;
                DatabaseService.Insert(config);
            });

            return ActionResult.Success;
        }

        public AppConfig GetById(string id)
        {
            return DatabaseService.GetById<AppConfig>(id).ReturnValue;
        }

        public ActionResult<List<AppConfig>> GetAll()
        {
            return DatabaseService.GetAll<AppConfig>();
        }
    }
}