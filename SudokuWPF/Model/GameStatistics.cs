using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.Model
{
    public class GameStatistics
    {
        private const string StatsFile = "game_stats.json";
        private static readonly string StatsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, StatsFile);

        public class GameStats
        {
            public int TotalGames { get; set; }
            public int CompletedGames { get; set; }
            public Dictionary<DifficultyLevels, int> GamesByDifficulty { get; set; }
            public Dictionary<DifficultyLevels, TimeSpan> BestTimes { get; set; }
            public List<GameRecord> RecentGames { get; set; }

            public GameStats()
            {
                GamesByDifficulty = new Dictionary<DifficultyLevels, int>();
                BestTimes = new Dictionary<DifficultyLevels, TimeSpan>();
                RecentGames = new List<GameRecord>();
            }
        }

        public class GameRecord
        {
            public DateTime Date { get; set; }
            public DifficultyLevels Difficulty { get; set; }
            public TimeSpan Duration { get; set; }
            public bool Completed { get; set; }
        }

        private static GameStats _stats;

        public static async Task LoadStatsAsync()
        {
            if (File.Exists(StatsPath))
            {
                var json = await File.ReadAllTextAsync(StatsPath);
                _stats = JsonSerializer.Deserialize<GameStats>(json);
            }
            else
            {
                _stats = new GameStats();
            }
        }

        public static async Task SaveStatsAsync()
        {
            var json = JsonSerializer.Serialize(_stats);
            await File.WriteAllTextAsync(StatsPath, json);
        }

        public static void AddGameRecord(DifficultyLevels difficulty, TimeSpan duration, bool completed)
        {
            if (_stats == null)
            {
                _stats = new GameStats();
            }

            _stats.TotalGames++;
            if (completed)
            {
                _stats.CompletedGames++;
            }

            if (!_stats.GamesByDifficulty.ContainsKey(difficulty))
            {
                _stats.GamesByDifficulty[difficulty] = 0;
            }
            _stats.GamesByDifficulty[difficulty]++;

            if (completed)
            {
                if (!_stats.BestTimes.ContainsKey(difficulty) || duration < _stats.BestTimes[difficulty])
                {
                    _stats.BestTimes[difficulty] = duration;
                }
            }

            _stats.RecentGames.Add(new GameRecord
            {
                Date = DateTime.Now,
                Difficulty = difficulty,
                Duration = duration,
                Completed = completed
            });

            // Keep only last 10 games
            if (_stats.RecentGames.Count > 10)
            {
                _stats.RecentGames.RemoveAt(0);
            }
        }

        public static GameStats GetStats()
        {
            return _stats ?? new GameStats();
        }
    }
} 