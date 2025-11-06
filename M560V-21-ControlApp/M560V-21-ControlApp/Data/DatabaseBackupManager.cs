using System;
using System.IO;
using System.Windows;

namespace M560V_21_ControlApp.Data
{
    public static class DatabaseBackupManager
    {
        private const string DatabaseFileName = "OkumaSchedule.db";

        public static string BackupDatabase(string backupDirectory)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string dataDir = Path.Combine(basePath, "Data");
                string dbPath = Path.Combine(dataDir, DatabaseFileName);

                if (!File.Exists(dbPath))
                    throw new FileNotFoundException("Database file not found.", dbPath);

                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"OkumaSchedule_{timestamp}.bak";
                string backupPath = Path.Combine(backupDirectory, backupFileName);

                File.Copy(dbPath, backupPath, true);
                return backupPath;
            }
            catch (Exception ex)
            {
                LogError("Backup failed", ex);
                MessageBox.Show("Backup failed: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Restore failed: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static string[] GetBackupFiles(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                    return new string[0];

                return Directory.GetFiles(directory, "*.bak");
            }
            catch (Exception ex)
            {
                LogError("Error retrieving backup list", ex);
                return new string[0];
            }
        }

        private static void LogError(string context, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupError.log");
                string message = $"{DateTime.Now:G} - {context}: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
                File.AppendAllText(logPath, message);
            }
            catch { }
        }
    }
}
