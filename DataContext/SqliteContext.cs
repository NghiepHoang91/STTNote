using STTNote.Const;
using STTNote.Enums;
using STTNote.Extensions;
using STTNote.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace STTNote.DataContext
{
    public class SqliteContext
    {
        public static SqliteContext Instance
        {
            get
            {
                return new SqliteContext();
            }
        }

        private string defaultConnectionString = @"URI=file:sql.db";
        private string connectionString = "";

        public SQLiteConnection GetConnection()
        {
            var Configs = ConfigHelper.INI.GetFromFile(Consts.Config.CONFIG_PATH);
            var databaseConfig = Configs.FirstOrDefault(n =>
            n.Section.Equals(Consts.Config.DATABASE) &&
            !n.IsComment &&
            n.Key.Equals(Consts.Config.DATABASE_PATH));

            connectionString = defaultConnectionString;
            if (databaseConfig?.Value?.IsLocalFilePath() == true)
            {
                connectionString = $"URI=file:{databaseConfig.Value}";
            }
            else if (databaseConfig?.Value?.IsValidUri() == true)
            {
                connectionString = $"URI={databaseConfig.Value}";
            }

            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        public string BuildCreateTableQueryFromModel(string modelName)
        {
            var model = ReflectionHelper.CreateClassInstanceByName(modelName);
            var properties = model.GetAttributes();
            List<string> columnWithType = new List<string>();
            foreach (var property in properties)
            {
                var columnName = property.Name;
                var columnType = GetSQLType(property);
                columnWithType.Add($"{columnName} {columnType}");
            }

            var columns = string.Join(", ", columnWithType);
            if (columns.Length > 0)
            {
                return $"CREATE TABLE IF NOT EXISTS {model.GetType().Name} ({columns}); ";
            }

            return string.Empty;
        }

        public string BuildInsertQueryFromModel(object model)
        {
            var tableName = model.GetType().Name;
            var propList = model.GetAttributes();
            var valueDict = model.GetValues();
            var columns = string.Join(", ", propList.Select(prop => prop.Name));

            var formatedValues = new List<string>();
            foreach (PropertyInfo prop in propList)
            {
                var valueType = prop.PropertyType.Name;
                var value = valueDict[prop.Name];

                if (valueType.Equals(Consts.DOTNET_STRING) || valueType.Equals(Consts.DOTNET_CHAR))
                {
                    formatedValues.Add($"'{value?.ToString()?.EscapeSQL() ?? string.Empty}'");
                }
                else if (valueType.Equals(Consts.DOTNET_BOOL))
                {
                    formatedValues.Add((bool)value ? "1" : "0");
                }
                else if (valueType.Equals(Consts.DOTNET_DATETIME))
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime convertedDate))
                    {
                        formatedValues.Add($"'{convertedDate.ToString(Consts.DATETIME_ISO_FORMAT)}'");
                    }
                }
                else if (valueType.Equals(Consts.DOTNET_SETTINGTYPE))
                {
                    var setting = (SettingType)value;
                    formatedValues.Add(((int)setting).ToString());
                }
                else
                {
                    formatedValues.Add($"'{value.ToBase64() ?? string.Empty}'");
                }
            }

            var values = string.Join(", ", formatedValues);
            if (columns.Length > 0)
            {
                return $"INSERT INTO {tableName}({columns}) VALUES({values}); ";
            }

            return string.Empty;
        }

        public string BuildUpdateQueryFromModel(object model)
        {
            var propList = model.GetAttributes();
            var valueDictionary = model.GetValues();

            var propertyUpdatedList = new List<string>();
            var modelId = string.Empty;

            foreach (PropertyInfo prop in propList)
            {
                var columnName = prop.Name;
                var value = valueDictionary[columnName];
                var valueType = prop.PropertyType.Name;

                if (columnName.Equals("Id"))
                {
                    modelId = value.ToString();
                    continue;
                }

                var formatedvalue = string.Empty;
                if (valueType.Equals(Consts.DOTNET_STRING) || valueType.Equals(Consts.DOTNET_CHAR))
                {
                    formatedvalue = $"'{value?.ToString()?.EscapeSQL() ?? string.Empty}'";
                }
                else if (valueType.Equals(Consts.DOTNET_BOOL))
                {
                    formatedvalue = (bool)value ? "1" : "0";
                }
                else if (valueType.Equals(Consts.DOTNET_DATETIME))
                {
                    if (DateTime.TryParse(value.ToString(), out DateTime convertedDate))
                    {
                        formatedvalue = $"'{convertedDate.ToString(Consts.DATETIME_ISO_FORMAT)}'";
                    }
                }
                else if (valueType.Equals(Consts.DOTNET_SETTINGTYPE))
                {
                    var setting = (SettingType)value;
                    formatedvalue = ((int)setting).ToString();
                }
                else
                {
                    formatedvalue = $"'{value.ToBase64()}'";
                }

                propertyUpdatedList.Add($"{columnName} = {formatedvalue}");
            }

            if (!string.IsNullOrEmpty(modelId))
            {
                var propertiesQuery = string.Join(", ", propertyUpdatedList);
                return $"UPDATE {model.GetType().Name} SET {propertiesQuery} WHERE Id = '{modelId}'";
            }

            return string.Empty;
        }

        public string BuildDeleteFromId(string Id, string tableName)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(tableName)) return string.Empty;
            return $"DELETE FROM {tableName} WHERE Id = '{Id}';";
        }

        public string BuildGetById(string Id, string tableName)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(tableName)) return string.Empty;
            return $"SELECT * FROM {tableName} WHERE Id = '{Id}';";
        }

        public string BuildGetAll(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return string.Empty;
            return $"SELECT * FROM {tableName};";
        }

        public string GetSQLType(MemberInfo member)
        {
            PropertyInfo propertyInfo = member as PropertyInfo;
            var propName = propertyInfo?.PropertyType?.Name ?? string.Empty;
            return Consts.DotNetToSQLTypeMap[propName];
        }
    }
}