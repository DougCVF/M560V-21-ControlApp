using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using M560V_21_ControlApp.Data;
using M560V_21_ControlApp.Windows;


namespace M560V_21_ControlApp
{
    public partial class MainWindow : Window
    {
        private readonly string _databasePath;
        private readonly string _backupDirectory;
        private readonly string _lastBackupFile;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // ✅ Corrected database path
                _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "OkumaSchedule.db");
                _backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatabaseBackups");
                Directory.CreateDirectory(_backupDirectory);
                _lastBackupFile = Path.Combine(_backupDirectory, "LastBackup.txt");

                // Initialize UI status fields
                UpdateDatabaseStatus();
                UpdateLastBackupLabel();

                // Auto-refresh status when changing tabs
                MainTabControl.SelectionChanged += (s, e) => UpdateDatabaseStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing database paths: " + ex.Message,
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ───────────────────────────────────────────────────────────────
        // FILE MENU HANDLERS
        // ───────────────────────────────────────────────────────────────

        private void MenuBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var backupWindow = new DatabaseBackupWindow
                {
                    Owner = this
                };

                backupWindow.ShowDialog();  // open modal window

                // ✅ Refresh status immediately after backup/restore
                UpdateDatabaseStatus();
                UpdateLastBackupLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open backup window.\n\n" + ex.Message,
                    "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // ───────────────────────────────────────────────────────────────
        // NAVIGATION MENU
        // ───────────────────────────────────────────────────────────────

        private void MenuBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedIndex > 0)
                MainTabControl.SelectedIndex--;
        }

        // ───────────────────────────────────────────────────────────────
        // HELP MENU
        // ───────────────────────────────────────────────────────────────

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "M560V-21 Control Application\n" +
                "Version 1.0.0\n\n" +
                "Used for schedule generation, part management, and rack control.\n" +
                "© 2025 Your Company",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ───────────────────────────────────────────────────────────────
        // STATUS BAR UPDATES
        // ───────────────────────────────────────────────────────────────

        private void UpdateDatabaseStatus()
        {
            try
            {
                if (File.Exists(_databasePath))
                {
                    txtDbStatus.Text = "Connected";
                    txtDbStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
                }
                else
                {
                    txtDbStatus.Text = "Missing";
                    txtDbStatus.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                txtDbStatus.Text = "Error";
                txtDbStatus.ToolTip = ex.Message;
                txtDbStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void UpdateLastBackupLabel()
        {
            try
            {
                if (!Directory.Exists(_backupDirectory))
                {
                    txtLastBackup.Text = "No backups found";
                    txtLastBackup.Foreground = System.Windows.Media.Brushes.LightGray;
                    return;
                }

                var backupFiles = Directory.GetFiles(_backupDirectory, "*.bak");
                if (backupFiles.Length == 0)
                {
                    txtLastBackup.Text = "None";
                    txtLastBackup.Foreground = System.Windows.Media.Brushes.LightGray;
                    return;
                }

                string latest = null;
                DateTime latestTime = DateTime.MinValue;
                foreach (var file in backupFiles)
                {
                    var time = File.GetLastWriteTime(file);
                    if (time > latestTime)
                    {
                        latestTime = time;
                        latest = file;
                    }
                }

                if (latest != null)
                {
                    txtLastBackup.Text = latestTime.ToString("MM/dd/yyyy HH:mm:ss");
                    txtLastBackup.Foreground = System.Windows.Media.Brushes.LightGreen;
                }
                else
                {
                    txtLastBackup.Text = "None";
                    txtLastBackup.Foreground = System.Windows.Media.Brushes.LightGray;
                }
            }
            catch (Exception ex)
            {
                txtLastBackup.Text = "Error";
                txtLastBackup.ToolTip = ex.Message;
                txtLastBackup.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
