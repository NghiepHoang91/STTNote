using System.Collections.Generic;

namespace STTNote.Const
{
    public class Consts
    {
        public const int GLOBAL_FORM_WIDTH = 400;
        public const int GLOBAL_FORM_HEIGHT = 600;
        public const int HEADER_HEIGHT = 35;

        public const string CHECKBOX_COLOR_ARCHIVED = "#aba9a9";
        public const string CHECKBOX_COLOR_NEW = "DarkSlateGray";
        public const string DATETIME_ISO_FORMAT = "yyyy-MM-dd hh:mm:ss.sss";

        public const string SQLITE_TEXT = "TEXT";
        public const string SQLITE_INT = "INTEGER";
        public const string SQLITE_REAL = "REAL";
        public const string SQLITE_BOOLEAN = "INTEGER";
        public const string SQLITE_DATETIME = "TEXT";

        public const string DOTNET_INT = "Int32";
        public const string DOTNET_LONG = "Int64";
        public const string DOTNET_DOUBLE = "Double";
        public const string DOTNET_FLOAT = "Float";
        public const string DOTNET_CHAR = "Char";
        public const string DOTNET_STRING = "String";
        public const string DOTNET_BOOL = "Boolean";
        public const string DOTNET_DATETIME = "DateTime";
        public const string DOTNET_OBJECT = "Object";
        public const string DOTNET_SETTINGTYPE = "SettingType";

        public static Dictionary<string, string> DotNetToSQLTypeMap = new Dictionary<string, string>
        {
            { DOTNET_INT, SQLITE_INT},
            { DOTNET_LONG, SQLITE_INT},
            { DOTNET_DOUBLE, SQLITE_REAL},
            { DOTNET_FLOAT, SQLITE_REAL},
            { DOTNET_CHAR, SQLITE_TEXT},
            { DOTNET_STRING, SQLITE_TEXT},
            { DOTNET_BOOL, SQLITE_INT},
            { DOTNET_DATETIME, SQLITE_DATETIME},
            { DOTNET_OBJECT, SQLITE_TEXT},
            { DOTNET_SETTINGTYPE, SQLITE_INT},
        };

        public class ConfigIds
        {
            public const string APP_PASSWORD = "ba03005a-a1c2-4410-a250-6ed380f9c360";
            public const string RUN_ON_STARTUP = "424b0842-51ed-4d9c-bbea-2ac66081434f";
            public const string AUTO_UPDATE = "824b084X-51gd-4d9d-bbea-7ac66081434x";
            public const string SELECTED_PROFILE = "924b074X-61gd-4d9d-bbta-3ac66086434z";
            public const string CREATE_DESKTOP_SHORTCUT = "924x074X-61xd-4d9d-bbxa-3ac66086434y";
        }

        public class Config
        {
            public const string CONFIG_PATH = "Config.ini";
            public const string DATABASE = "Database";
            public const string DATABASE_PATH = "Path";
        }
    }
}