using System;
using System.Data.SQLite;
using System.IO;

namespace M560V_21_ControlApp.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DatabaseFileName = "OkumaSchedule.db";

        public static string GetDatabasePath()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dataDir = Path.Combine(baseDir, "Data");

                if (!Directory.Exists(dataDir))
                    Directory.CreateDirectory(dataDir);

                string dbPath = Path.Combine(dataDir, DatabaseFileName);

                if (string.IsNullOrEmpty(dbPath))
                    throw new Exception("Database path could not be resolved.");

                return dbPath;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetDatabasePath(): " + ex.Message);
            }
        }

        public static SQLiteConnection GetConnection()
        {
            string dbPath = GetDatabasePath();
            string connString = "Data Source=" + dbPath + ";Version=3;";
            return new SQLiteConnection(connString);
        }
    }
}
