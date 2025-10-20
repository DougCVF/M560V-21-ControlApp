using System;
using System.Data.SQLite;
using System.IO;

namespace M560V_21_ControlApp.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db_init_log.txt");

            try
            {
                File.WriteAllText(logPath, $"[{DateTime.Now}] Initialization started\n");

                string dbPath = DatabaseHelper.GetDatabasePath();
                File.AppendAllText(logPath, $"Database path: {dbPath}\n");

                if (string.IsNullOrEmpty(dbPath))
                    throw new Exception("Database path is null or empty.");

                string dbDir = Path.GetDirectoryName(dbPath);
                File.AppendAllText(logPath, $"Database directory: {dbDir}\n");

                if (string.IsNullOrEmpty(dbDir))
                    throw new Exception("Database directory path is null or empty.");

                if (!Directory.Exists(dbDir))
                {
                    File.AppendAllText(logPath, "Directory does not exist — creating.\n");
                    Directory.CreateDirectory(dbDir);
                }

                bool newDatabase = false;

                File.AppendAllText(logPath, $"Checking if file exists: {File.Exists(dbPath)}\n");
                if (!File.Exists(dbPath))
                {
                    File.AppendAllText(logPath, "Creating SQLite file...\n");
                    SQLiteConnection.CreateFile(dbPath);
                    newDatabase = true;
                }

                File.AppendAllText(logPath, "Opening connection...\n");
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    File.AppendAllText(logPath, "Connection opened successfully.\n");

                    string createPartsTable = @"
                        CREATE TABLE IF NOT EXISTS Parts (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            PartNumber TEXT NOT NULL UNIQUE,
                            Description TEXT,

                            StockWidth REAL DEFAULT 0,
                            StockDepth REAL DEFAULT 0,
                            StockHeight REAL DEFAULT 0,

                            Op10PickXOffset REAL DEFAULT 0,
                            Op10PickYOffset REAL DEFAULT 0,
                            Op10PickZOffset REAL DEFAULT 0,

                            Op20PickXOffset REAL DEFAULT 0,
                            Op20PickYOffset REAL DEFAULT 0,
                            Op20PickZOffset REAL DEFAULT 0,

                            Op20FinXOffset REAL DEFAULT 0,
                            Op20FinYOffset REAL DEFAULT 0,
                            Op20FinZOffset REAL DEFAULT 0,

                            Op10VisePSI INTEGER DEFAULT 120,
                            Op20VisePSI INTEGER DEFAULT 120,

                            Op10ProgramName TEXT DEFAULT '',
                            Op20ProgramName TEXT DEFAULT '',

                            Op10CycleTime REAL DEFAULT 0,
                            Op20CycleTime REAL DEFAULT 0
                        );";

                    File.AppendAllText(logPath, "Creating Parts table...\n");
                    using (var cmd = new SQLiteCommand(createPartsTable, conn))
                        cmd.ExecuteNonQuery();

                    string createTrayRulesTable = @"
                        CREATE TABLE IF NOT EXISTS TrayRules (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            TrayType TEXT NOT NULL,
                            MaxDepth REAL NOT NULL,
                            MaxHeight REAL NOT NULL,
                            QtyPerShelf INTEGER NOT NULL
                        );";
                    File.AppendAllText(logPath, "Creating TrayRules table...\n");
                    using (var cmd = new SQLiteCommand(createTrayRulesTable, conn))
                        cmd.ExecuteNonQuery();

                    if (newDatabase)
                    {
                        string seedData = @"
                            INSERT INTO TrayRules (TrayType, MaxDepth, MaxHeight, QtyPerShelf)
                            VALUES 
                                ('Small', 3.1, 10.1, 10),
                                ('Medium', 6.1, 10.1, 5),
                                ('Large', 8.1, 10.1, 4);";
                        File.AppendAllText(logPath, "Seeding TrayRules...\n");
                        using (var cmd = new SQLiteCommand(seedData, conn))
                            cmd.ExecuteNonQuery();
                    }

                    File.AppendAllText(logPath, "Initialization completed successfully.\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logPath, $"ERROR: {ex.Message}\nSTACK: {ex.StackTrace}\n");
                System.Windows.MessageBox.Show(
                    "Database initialization failed:\n" + ex.Message,
                    "Database Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
        }
    }
}
