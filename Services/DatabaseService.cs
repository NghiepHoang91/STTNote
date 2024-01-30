using STTNote.ActionResults;
using STTNote.DataContext;
using STTNote.Extensions;
using STTNote.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace STTNote.Services
{
    public class DatabaseService
    {
        public static DatabaseService Instance
        {
            get
            {
                return new DatabaseService();
            }
        }

        private DatabaseService()
        { }

        public ActionResult Insert<T>(T model)
        {
            if (model == null) return ActionResult.Failed("Model is not vaild");

            try
            {
                var connection = SqliteContext.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = SqliteContext.Instance.BuildInsertQueryFromModel(model);
                if (command.ExecuteNonQuery() > 0)
                {
                    return ActionResult.Success;
                }
                return ActionResult.Failed("Execute Query Failed");
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public ActionResult Update<T>(T model)
        {
            if (model == null) return ActionResult.Failed("Model is not vaild");

            try
            {
                var connection = SqliteContext.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = SqliteContext.Instance.BuildUpdateQueryFromModel(model);
                if (command.ExecuteNonQuery() > 0)
                {
                    return ActionResult.Success;
                }
                return ActionResult.Failed("Execute Query Failed");
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public ActionResult Delete<T>(string id)
        {
            if (string.IsNullOrEmpty(id)) return ActionResult.Failed("Id is not vaild");

            try
            {
                var tableName = typeof(T).Name;
                var connection = SqliteContext.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = SqliteContext.Instance.BuildDeleteFromId(id, tableName);
                if (command.ExecuteNonQuery() > 0)
                {
                    return ActionResult.Success;
                }
                return ActionResult.Failed("Execute Query Failed");
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public ActionResult<T> GetById<T>(string id)
        {
            if (string.IsNullOrEmpty(id)) return ActionResult<T>.Failed("Id is not vaild");

            try
            {
                var tableName = typeof(T).Name;
                var connection = SqliteContext.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = SqliteContext.Instance.BuildGetById(id, tableName);
                using SQLiteDataReader reader = command.ExecuteReader();

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        T model = (T)ReflectionHelper.CreateClassInstanceByName(tableName);
                        reader.TransferValueToModel(model);
                        ActionResult<T> result = ActionResult<T>.Success;
                        result.ReturnValue = model;
                        return result;
                    }
                }
                return ActionResult<T>.Failed("Execute Query Failed");
            }
            catch (Exception ex)
            {
                return ActionResult<T>.Failed(ex.Message);
            }
        }

        public ActionResult<List<T>> GetAll<T>()
        {
            try
            {
                var tableName = typeof(T).Name;
                var connection = SqliteContext.Instance.GetConnection();
                var command = connection.CreateCommand();
                command.CommandText = SqliteContext.Instance.BuildGetAll(tableName);
                using SQLiteDataReader reader = command.ExecuteReader();

                List<T> listOfT = new List<T>();
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        T model = (T)ReflectionHelper.CreateClassInstanceByName(tableName);
                        reader.TransferValueToModel(model);
                        listOfT.Add(model);
                    }

                    ActionResult<List<T>> result = ActionResult<List<T>>.Success;
                    result.ReturnValue = listOfT;
                    return result;
                }
                return ActionResult<List<T>>.Failed("Execute Query Failed");
            }
            catch (Exception ex)
            {
                return ActionResult<List<T>>.Failed(ex.Message);
            }
        }
    }
}