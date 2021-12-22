using SQLite;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

#if DEBUG
using System.Diagnostics;
#endif

namespace WeatherApp.Database
{
    public class SqLiteRepository : IRepository
    {
        readonly SQLiteConnection connection;
        readonly object dbLock;

        public const string DBClauseSyncOff = "PRAGMA SYNCHRONOUS=OFF;";
        public const string DBClauseVacuum = "VACUUM;";

        public SqLiteRepository()
        {
            connection = WeatherApp.Constants.Constants.DBConnection;
            CreateTables();
            dbLock = new object();
        }

        public void SaveData<T>(T toStore)
        {
            lock (dbLock)
            {
                connection.Execute(DBClauseSyncOff);
                connection.BeginTransaction();
                try
                {
                    connection.InsertOrReplace(toStore);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
                finally
                {
                    connection.Commit();
                }
            }
        }

        public void SaveListData<T>(List<T> toStore)
        {
            lock (dbLock)
            {
                connection.Execute(DBClauseSyncOff);
                connection.BeginTransaction();
                try
                {
                    connection.InsertAll(toStore);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
                finally
                {
                    connection.Commit();
                }
            }
        }

        public int Count<T>() where T : class, new()
        {
            var count = GetList<T>();
            return count.Count;
        }

        public List<T> GetList<T>(int top = 0) where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0}", GetName(typeof(T).ToString()));
            var list = connection.Query<T>(sql, string.Empty);
            if (list.Count != 0)
            {
                if (top != 0)
                {
                    list = list.Take(top).ToList();
                }
            }

            return list;
        }

        public List<T> GetList<T, TU>(string para, TU val, int top = 0) where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1}=?", GetName(typeof(T).ToString()), para);
            var list = connection.Query<T>(sql, val);
            if (list.Count != 0)
            {
                if (top != 0)
                {
                    list = list.Take(top).ToList();
                }
            }

            return list;
        }

        public T? GetData<T>() where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0}", GetName(typeof(T).ToString()));
            var list = connection.Query<T>(sql, string.Empty);
            return list != null ? list.FirstOrDefault() : default;
        }

        public T? GetData<T, TU>(string para, TU val) where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1}=?", GetName(typeof(T).ToString()), para);
            var list = connection.Query<T>(sql, val);
            return list != null ? list.FirstOrDefault() : default;
        }

        public void Delete<T>(T stored)
        {
            lock (dbLock)
            {
                connection.Execute(DBClauseSyncOff);
                connection.BeginTransaction();
                try
                {
                    connection.Delete(stored);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
                finally
                {
                    connection.Commit();
                }
            }
        }

        public void DeleteAll()
        {
            lock (dbLock)
            {
                connection.Execute(DBClauseSyncOff);
                connection.BeginTransaction();
                try
                {
                    connection.DeleteAll<UserSettings>();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
                finally
                {
                    connection.Commit();
                }
            }
        }

        public T? GetData<T, TU, TV>(string para1, TU val1, string para2, TV val2) where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1}=? AND {2}=?", GetName(typeof(T).ToString()), para1, para2);
            var list = connection.Query<T>(sql, val1, val2);
            return list != null ? list.FirstOrDefault() : default;
        }

        public int GetID<T>() where T : class, new()
        {
            string sql = string.Format("SELECT last_insert_rowid() FROM {0}", GetName(typeof(T).ToString()));
            var id = connection.ExecuteScalar<int>(sql, string.Empty);
            return id;
        }

        void CreateTables()
        {
            connection.CreateTable<UserSettings>();
            connection.Commit();
        }

        string GetName(string name)
        {
            var list = name.Split('.').ToList();
            if (list.Count == 1)
            {
                return list[0];
            }

            return list[^1];
        }

        public int Count<T, U>(string p, U val) where T : class, new()
        {
            var sql = string.Format("SELECT * FROM {0} WHERE {1}={2}", GetName(typeof(T).ToString()), p, val);
            var list = connection.Query<T>(sql, string.Empty);
            return list.Count;

        }
    }
}
