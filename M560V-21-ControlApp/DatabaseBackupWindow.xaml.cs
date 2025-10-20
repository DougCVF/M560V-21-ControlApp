using System;
using System.IO;
using System.Linq;
using System.Windows;
using M560V_21_ControlApp.Data;

namespace M560V_21_ControlApp.Windows
{
    public partial class DatabaseBackupWindow : Window
    {
        public DatabaseBackupWindow()
        {
            InitializeComponent();
            LoadBackupList();
        }

        private void LoadBackupList()
        {
            lstBackups.Items.Clear();
            var backups = DatabaseBackupManager.GetBackupFiles()
                .OrderByDescending(f => f)
                .ToArray();

            if (backups.Length == 0)
            {
                lstBackups.Items.Add("No backups found.");
                lstBackups.IsEnabled = false;
                return;
            }

            lstBackups.IsEnabled = true;
            foreach (var file in backups)
                lstBackups.Items.Add(System.IO.Path.GetFileName(file));
        }

        private void btnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            string path = DatabaseBackupManager.BackupDatabase();
            if (path != null)
            {
                MessageBox.Show("Backup created successfully:\n" + path,
                    "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBackupList();
            }
            else
            {
                MessageBox.Show("Backup failed.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            if (lstBackups.SelectedItem == null)
            {
                MessageBox.Show("Please select a backup to restore.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selected = lstBackups.SelectedItem.ToString();
            if (selected == "No backups found.") return;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = System.IO.Path.Combine(basePath, "Backups", selected);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Selected backup file not found.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                "This will overwrite the current database.\nContinue?",
                "Confirm Restore",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                if (DatabaseBackupManager.RestoreDatabase(fullPath))
                    MessageBox.Show("Database restored successfully.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Restore failed.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBackupList();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
