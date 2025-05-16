using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class DatabaseManager
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public DatabaseManager()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sudoku.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(connection))
                    {
                        // Таблиця для збереження ігор
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Games (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Difficulty TEXT NOT NULL,
                                StartTime DATETIME NOT NULL,
                                EndTime DATETIME,
                                IsCompleted BOOLEAN NOT NULL,
                                Score INTEGER NOT NULL
                            );";
                        command.ExecuteNonQuery();

                        // Таблиця для збереження клітинок гри
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS GameCells (
                                GameId INTEGER NOT NULL,
                                Col INTEGER NOT NULL,
                                Row INTEGER NOT NULL,
                                Value INTEGER NOT NULL,
                                IsAnswer BOOLEAN NOT NULL,
                                FOREIGN KEY (GameId) REFERENCES Games(Id)
                            );";
                        command.ExecuteNonQuery();

                        // Таблиця для збереження статистики
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Statistics (
                                Difficulty TEXT PRIMARY KEY,
                                GamesPlayed INTEGER NOT NULL,
                                GamesCompleted INTEGER NOT NULL,
                                BestTime INTEGER,
                                AverageTime INTEGER
                            );";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public async Task SaveGameAsync(GameModel game, string difficulty, int score)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SQLiteCommand(connection))
                        {
                            // Зберігаємо основну інформацію про гру
                            command.CommandText = @"
                                INSERT INTO Games (Difficulty, StartTime, IsCompleted, Score)
                                VALUES (@difficulty, @startTime, @isCompleted, @score);";
                            command.Parameters.AddWithValue("@difficulty", difficulty);
                            command.Parameters.AddWithValue("@startTime", DateTime.Now);
                            command.Parameters.AddWithValue("@isCompleted", game.GameComplete);
                            command.Parameters.AddWithValue("@score", score);
                            await command.ExecuteNonQueryAsync();

                            // Отримуємо ID збереженої гри
                            command.CommandText = "SELECT last_insert_rowid();";
                            var gameId = Convert.ToInt64(await command.ExecuteScalarAsync());

                            // Зберігаємо клітинки
                            command.CommandText = @"
                                INSERT INTO GameCells (GameId, Col, Row, Value, IsAnswer)
                                VALUES (@gameId, @col, @row, @value, @isAnswer);";

                            foreach (var cell in game.CellList)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@gameId", gameId);
                                command.Parameters.AddWithValue("@col", cell.Col);
                                command.Parameters.AddWithValue("@row", cell.Row);
                                command.Parameters.AddWithValue("@value", cell.CellState == CellStateEnum.Answer ? cell.Answer : cell.UserAnswer);
                                command.Parameters.AddWithValue("@isAnswer", cell.CellState == CellStateEnum.Answer);
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateStatisticsAsync(string difficulty, bool isCompleted, TimeSpan gameTime)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Statistics (Difficulty, GamesPlayed, GamesCompleted, BestTime, AverageTime)
                        VALUES (@difficulty, 1, @completed, @time, @time)
                        ON CONFLICT(Difficulty) DO UPDATE SET
                            GamesPlayed = GamesPlayed + 1,
                            GamesCompleted = GamesCompleted + @completed,
                            BestTime = CASE 
                                WHEN BestTime IS NULL OR @time < BestTime THEN @time
                                ELSE BestTime
                            END,
                            AverageTime = (AverageTime * GamesPlayed + @time) / (GamesPlayed + 1);";

                    command.Parameters.AddWithValue("@difficulty", difficulty);
                    command.Parameters.AddWithValue("@completed", isCompleted ? 1 : 0);
                    command.Parameters.AddWithValue("@time", (int)gameTime.TotalSeconds);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<Dictionary<string, object>> GetStatisticsAsync(string difficulty)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM Statistics WHERE Difficulty = @difficulty;";
                    command.Parameters.AddWithValue("@difficulty", difficulty);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Dictionary<string, object>
                            {
                                ["GamesPlayed"] = reader.GetInt32(1),
                                ["GamesCompleted"] = reader.GetInt32(2),
                                ["BestTime"] = reader.IsDBNull(3) ? null : (object)TimeSpan.FromSeconds(reader.GetInt32(3)),
                                ["AverageTime"] = reader.IsDBNull(4) ? null : (object)TimeSpan.FromSeconds(reader.GetInt32(4))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
} 