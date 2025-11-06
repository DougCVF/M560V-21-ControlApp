using System;
using System.Collections.Generic;
using System.Data.SQLite;
using M560V_21_ControlApp.Models;

namespace M560V_21_ControlApp.Data
{
    public static class Repository
    {
        // -----------------------------
        //  Get All Parts
        // -----------------------------
        public static List<Part> GetAllParts()
        {
            var parts = new List<Part>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    const string query = "SELECT * FROM Parts ORDER BY PartNumber";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var part = new Part
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                PartNumber = reader["PartNumber"].ToString(),
                                Description = reader["Description"].ToString(),
                                StockWidth = Convert.ToDouble(reader["StockWidth"]),
                                StockDepth = Convert.ToDouble(reader["StockDepth"]),
                                StockHeight = Convert.ToDouble(reader["StockHeight"]),
                                Op10PickXOffset = Convert.ToDouble(reader["Op10PickXOffset"]),
                                Op10PickYOffset = Convert.ToDouble(reader["Op10PickYOffset"]),
                                Op10PickZOffset = Convert.ToDouble(reader["Op10PickZOffset"]),
                                Op10CycleTime = Convert.ToDouble(reader["Op10CycleTime"]),
                                Op20PickXOffset = Convert.ToDouble(reader["Op20PickXOffset"]),
                                Op20PickYOffset = Convert.ToDouble(reader["Op20PickYOffset"]),
                                Op20PickZOffset = Convert.ToDouble(reader["Op20PickZOffset"]),
                                Op20FinXOffset = Convert.ToDouble(reader["Op20FinXOffset"]),
                                Op20FinYOffset = Convert.ToDouble(reader["Op20FinYOffset"]),
                                Op20FinZOffset = Convert.ToDouble(reader["Op20FinZOffset"]),
                                Op20CycleTime = Convert.ToDouble(reader["Op20CycleTime"]),
                                Op10VisePSI = Convert.ToDouble(reader["Op10VisePSI"]),
                                Op20VisePSI = Convert.ToDouble(reader["Op20VisePSI"]),
                                Op10ProgramName = reader["Op10ProgramName"].ToString(),
                                Op20ProgramName = reader["Op20ProgramName"].ToString()
                            };

                            parts.Add(part);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error loading parts: " + ex.Message,
                    "Database Error", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }

            return parts;
        }

        // -----------------------------
        //  Insert Part
        // -----------------------------
        public static void InsertPart(Part part)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                const string sql = @"
                    INSERT INTO Parts (
                        PartNumber, Description, StockWidth, StockDepth, StockHeight,
                        Op10PickXOffset, Op10PickYOffset, Op10PickZOffset, Op10CycleTime,
                        Op20PickXOffset, Op20PickYOffset, Op20PickZOffset,
                        Op20FinXOffset, Op20FinYOffset, Op20FinZOffset, Op20CycleTime,
                        Op10VisePSI, Op20VisePSI, Op10ProgramName, Op20ProgramName
                    )
                    VALUES (
                        @PartNumber, @Description, @StockWidth, @StockDepth, @StockHeight,
                        @Op10PickXOffset, @Op10PickYOffset, @Op10PickZOffset, @Op10CycleTime,
                        @Op20PickXOffset, @Op20PickYOffset, @Op20PickZOffset,
                        @Op20FinXOffset, @Op20FinYOffset, @Op20FinZOffset, @Op20CycleTime,
                        @Op10VisePSI, @Op20VisePSI, @Op10ProgramName, @Op20ProgramName
                    );";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AssignPartParameters(cmd, part);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------
        //  Update Part
        // -----------------------------
        public static void UpdatePart(Part part)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                const string sql = @"
                    UPDATE Parts SET
                        PartNumber = @PartNumber,
                        Description = @Description,
                        StockWidth = @StockWidth,
                        StockDepth = @StockDepth,
                        StockHeight = @StockHeight,
                        Op10PickXOffset = @Op10PickXOffset,
                        Op10PickYOffset = @Op10PickYOffset,
                        Op10PickZOffset = @Op10PickZOffset,
                        Op10CycleTime = @Op10CycleTime,
                        Op20PickXOffset = @Op20PickXOffset,
                        Op20PickYOffset = @Op20PickYOffset,
                        Op20PickZOffset = @Op20PickZOffset,
                        Op20FinXOffset = @Op20FinXOffset,
                        Op20FinYOffset = @Op20FinYOffset,
                        Op20FinZOffset = @Op20FinZOffset,
                        Op20CycleTime = @Op20CycleTime,
                        Op10VisePSI = @Op10VisePSI,
                        Op20VisePSI = @Op20VisePSI,
                        Op10ProgramName = @Op10ProgramName,
                        Op20ProgramName = @Op20ProgramName
                    WHERE Id = @Id;";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AssignPartParameters(cmd, part);
                    cmd.Parameters.AddWithValue("@Id", part.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------
        //  Delete Part
        // -----------------------------
        public static void DeletePart(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                const string sql = "DELETE FROM Parts WHERE Id = @Id";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------
        //  Helper
        // -----------------------------
        private static void AssignPartParameters(SQLiteCommand cmd, Part part)
        {
            cmd.Parameters.AddWithValue("@PartNumber", part.PartNumber);
            cmd.Parameters.AddWithValue("@Description", part.Description);
            cmd.Parameters.AddWithValue("@StockWidth", part.StockWidth);
            cmd.Parameters.AddWithValue("@StockDepth", part.StockDepth);
            cmd.Parameters.AddWithValue("@StockHeight", part.StockHeight);
            cmd.Parameters.AddWithValue("@Op10PickXOffset", part.Op10PickXOffset);
            cmd.Parameters.AddWithValue("@Op10PickYOffset", part.Op10PickYOffset);
            cmd.Parameters.AddWithValue("@Op10PickZOffset", part.Op10PickZOffset);
            cmd.Parameters.AddWithValue("@Op10CycleTime", part.Op10CycleTime);
            cmd.Parameters.AddWithValue("@Op20PickXOffset", part.Op20PickXOffset);
            cmd.Parameters.AddWithValue("@Op20PickYOffset", part.Op20PickYOffset);
            cmd.Parameters.AddWithValue("@Op20PickZOffset", part.Op20PickZOffset);
            cmd.Parameters.AddWithValue("@Op20FinXOffset", part.Op20FinXOffset);
            cmd.Parameters.AddWithValue("@Op20FinYOffset", part.Op20FinYOffset);
            cmd.Parameters.AddWithValue("@Op20FinZOffset", part.Op20FinZOffset);
            cmd.Parameters.AddWithValue("@Op20CycleTime", part.Op20CycleTime);
            cmd.Parameters.AddWithValue("@Op10VisePSI", part.Op10VisePSI);
            cmd.Parameters.AddWithValue("@Op20VisePSI", part.Op20VisePSI);
            cmd.Parameters.AddWithValue("@Op10ProgramName", part.Op10ProgramName);
            cmd.Parameters.AddWithValue("@Op20ProgramName", part.Op20ProgramName);
        }
    }
}
