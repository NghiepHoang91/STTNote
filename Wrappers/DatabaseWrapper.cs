using STTNote.ActionResults;
using STTNote.DataContext;
using STTNote.Models;
using STTNote.Services;
using System.Collections.Generic;
using System.Text;

namespace STTNote.Wrappers
{
    public class DatabaseWrapper
    {
        public DatabaseService DatabaseService { get; set; }

        public DatabaseWrapper()
        {
            DatabaseService = DatabaseService.Instance;
        }

        public static DatabaseWrapper Instance
        {
            get { return new DatabaseWrapper(); }
        }

        public ActionResult InitDefaultTables()
        {
            var tableList = new List<string>
            {
                nameof(Note),
                nameof(AppConfig),
                nameof(NoteCheckPoint),
                nameof(NoteProfile),
                nameof(Profile)
            };

            var connection = SqliteContext.Instance.GetConnection();
            var command = connection.CreateCommand();
            var queryBag = new StringBuilder();
            foreach (var tableName in tableList)
            {
                string sqlQuery = SqliteContext.Instance.BuildCreateTableQueryFromModel(tableName);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    queryBag.Append(sqlQuery);
                }
            }

            command.CommandText = queryBag.ToString();
            command.ExecuteNonQuery();

            return ActionResult.Success;
        }
    }
}