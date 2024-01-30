using STTNote.Enums;
using System.Data.SQLite;
using System.Linq;

namespace STTNote.Extensions
{
    public static class SqliteExtension
    {
        public static void TransferValueToModel(this SQLiteDataReader reader, object obj)
        {
            var fieldList = obj.GetType().GetFields();
            var propList = obj.GetType().GetProperties();

            if (fieldList?.Count() > 0)
            {
                for (int columnIndex = 0; columnIndex < fieldList.Length; columnIndex++)
                {
                    switch (fieldList[columnIndex].FieldType.Name)
                    {
                        case "Boolean": fieldList[columnIndex].SetValue(obj, reader.GetBoolean(columnIndex)); break;
                        case "Int32": fieldList[columnIndex].SetValue(obj, reader.GetInt32(columnIndex)); break;
                        case "Int64": fieldList[columnIndex].SetValue(obj, reader.GetInt64(columnIndex)); break;
                        case "Double": fieldList[columnIndex].SetValue(obj, reader.GetDouble(columnIndex)); break;
                        case "Float": fieldList[columnIndex].SetValue(obj, reader.GetFloat(columnIndex)); break;
                        case "Char": fieldList[columnIndex].SetValue(obj, reader.GetChar(columnIndex)); break;
                        case "String": fieldList[columnIndex].SetValue(obj, reader.GetString(columnIndex)); break;
                        case "DateTime": fieldList[columnIndex].SetValue(obj, reader.GetDateTime(columnIndex)); break;
                        case "SettingType":
                            {
                                var value = reader.GetInt32(columnIndex);
                                SettingType type = (SettingType)value;
                                fieldList[columnIndex].SetValue(obj, type);
                            }
                            break;

                        case "Object":
                            {
                                var value = reader.GetString(columnIndex);
                                if (value != null)
                                {
                                    object convertedObj = value.FromBase64<object>();
                                    fieldList[columnIndex].SetValue(obj, convertedObj);
                                }
                                else
                                {
                                    fieldList[columnIndex].SetValue(obj, null);
                                }
                            }
                            break;

                        default: fieldList[columnIndex].SetValue(obj, reader.GetString(columnIndex)); break;
                    }
                }
            }//problem with get blob
            else if (propList?.Count() > 0)
            {
                for (int columnIndex = 0; columnIndex < propList.Length; columnIndex++)
                {
                    switch (propList[columnIndex].PropertyType.Name)
                    {
                        case "Boolean": propList[columnIndex].SetValue(obj, reader.GetBoolean(columnIndex)); break;
                        case "Int32": propList[columnIndex].SetValue(obj, reader.GetInt32(columnIndex)); break;
                        case "Int64": propList[columnIndex].SetValue(obj, reader.GetInt64(columnIndex)); break;
                        case "Double": propList[columnIndex].SetValue(obj, reader.GetDouble(columnIndex)); break;
                        case "Float": propList[columnIndex].SetValue(obj, reader.GetFloat(columnIndex)); break;
                        case "Char": propList[columnIndex].SetValue(obj, reader.GetChar(columnIndex)); break;
                        case "String": propList[columnIndex].SetValue(obj, reader.GetString(columnIndex)); break;
                        case "DateTime": propList[columnIndex].SetValue(obj, reader.GetDateTime(columnIndex)); break;
                        case "SettingType":
                            {
                                var value = reader.GetInt32(columnIndex);
                                SettingType type = (SettingType)value;
                                propList[columnIndex].SetValue(obj, type);
                            }
                            break;

                        case "Object":
                            {
                                var value = reader.GetString(columnIndex);
                                if (value != null)
                                {
                                    object convertedObj = value.FromBase64<object>();
                                    propList[columnIndex].SetValue(obj, convertedObj);
                                }
                                else
                                {
                                    propList[columnIndex].SetValue(obj, null);
                                }
                            }
                            break;

                        default: propList[columnIndex].SetValue(obj, reader.GetString(columnIndex)); break;
                    }
                }
            }
        }

        public static string? EscapeSQL(this string? value)
        {
            if (value == null) return value;

            return value
                .Replace("'", "''");
        }
    }
}