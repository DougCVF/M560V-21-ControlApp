using System;
using System.IO;
using System.Windows;

namespace M560V_21_ControlApp.Data
{
    public static class DatabaseBackupManager
    {
        private const string DatabaseFileName = "OkumaSchedule.db";
        private static readonly string BackupFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        public static string BackupDatabase()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string dataDir = Path.Combine(basePath, "Data");
                string dbPath = Path.Combine(dataDir, DatabaseFileName);

                if (!File.Exists(dbPath))
                    throw new FileNotFoundException("Database file not found.", dbPath);

                if (!Directory.Exists(BackupFolder))
                    Directory.CreateDirectory(BackupFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupName = $"OkumaSchedule_{timestamp}.db";
                string backupPath = Path.Combine(BackupFolder, backupName);

                File.Copy(dbPath, backupPath, true);
                return backupPath;
            }
            catch (Exception ex)
            {
                LogError("Backup failed", ex);
                return null;
            }
        }

        public static bool RestoreDatabase(string backupFilePath)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string dataDir = Path.Combine(basePath, "Data");
                string dbPath = Path.Combine(dataDir, DatabaseFileName);

                if (!File.Exists(backupFilePath))
                    throw new FileNotFoundException("Backup file not found.", backupFilePath);

                File.Copy(backupFilePath, dbPath, true);
                return true;
            }
            catch (Exception ex)
            {
                LogError("Restore failed", ex);
                return false;
            }
        }

        public static string[] GetBackupFiles()
        {
            try
            {
                if (!Directory.Exists(BackupFolder))
                    return new string[0]; // ✅ Compatible with .NET 4.5

                return Directory.GetFiles(BackupFolder, "*.db");
            }
            catch (Exception ex)
            {
                LogError("Error retrieving backup list", ex);
                return new string[0]; // ✅ Compatible with .NET 4.5
            }
        }

        private static void LogError(string context, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupError.log");
                string message = string.Format(
                    "{0:G} - {1}: {2}{3}{4}{3}",
                    DateTime.Now, context, ex.Message, Environment.NewLine, ex.StackTrace);
                File.AppendAllText(logPath, message);
            }
            catch
            {
                // Ignore logging errors
            }

            MessageBox.Show(string.Format("{0}: {1}", context, ex.Message),
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
