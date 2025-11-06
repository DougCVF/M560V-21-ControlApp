using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using M560V_21_ControlApp.Data;
using MessageBox = System.Windows.MessageBox;


namespace M560V_21_ControlApp.Windows
{
    public partial class DatabaseBackupWindow : Window
    {
        private readonly string backupConfigPath;

        public DatabaseBackupWindow()
        {
            InitializeComponent();

            backupConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupPath.txt");
            LoadBackupPath();
            LoadBackupList();
        }

        // ───────────────────────────────────────────────────────────────
        // BACKUP FOLDER HANDLING
        // ───────────────────────────────────────────────────────────────

        private void LoadBackupPath()
        {
            try
            {
                if (File.Exists(backupConfigPath))
                {
                    string path = File.ReadAllText(backupConfigPath).Trim();
                    if (Directory.Exists(path))
                        lblBackupPath.Text = path;
                    else
                        lblBackupPath.Text = GetDefaultBackupPath();
                }
                else
                {
                    lblBackupPath.Text = GetDefaultBackupPath();
                }

                Directory.CreateDirectory(lblBackupPath.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading backup path: " + ex.Message);
                lblBackupPath.Text = GetDefaultBackupPath();
            }
        }

        private void SaveBackupPath(string path)
        {
            try
            {
                File.WriteAllText(backupConfigPath, path);
                lblBackupPath.Text = path;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Failed to save backup path.\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDefaultBackupPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseBackups");
        }

        private void btnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var dlg = new FolderBrowserDialog())
                {
                    dlg.Description = "Select a folder to store database backups";
                    dlg.SelectedPath = lblBackupPath.Text;
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        SaveBackupPath(dlg.SelectedPath);
                        LoadBackupList();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Folder selection failed: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ───────────────────────────────────────────────────────────────
        // BACKUP / RESTORE ACTIONS
        // ───────────────────────────────────────────────────────────────

        private void LoadBackupList()
        {
            try
            {
                lstBackups.Items.Clear();

                string[] backups = DatabaseBackupManager.GetBackupFiles(lblBackupPath.Text);
                foreach (var b in backups)
                    lstBackups.Items.Add(Path.GetFileName(b));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading backups: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = DatabaseBackupManager.BackupDatabase(lblBackupPath.Text);
                if (!string.IsNullOrEmpty(result))
                {
                    System.Windows.MessageBox.Show("Backup created:\n" + result,
                        "Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBackupList();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Backup failed: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lstBackups.SelectedItem == null)
                {
                    System.Windows.MessageBox.Show("Please select a backup file to restore.");
                    return;
                }

                string selectedFile = Path.Combine(lblBackupPath.Text, lstBackups.SelectedItem.ToString());
                if (DatabaseBackupManager.RestoreDatabase(selectedFile))
                {
                    System.Windows.MessageBox.Show("Database restored successfully.",
                        "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Restore failed: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBackupList();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
