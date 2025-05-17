using System;
using System.IO;
using Newtonsoft.Json;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.Model
{
    public class GameStatistics
    {
        private readonly string _statsFile = "stats.json";
        private GameStats _stats;

        public GameStatistics()
        {
            LoadStats();
        }

        private void LoadStats()
        {
            if (File.Exists(_statsFile))
            {
                var json = File.ReadAllText(_statsFile);
                _stats = JsonConvert.DeserializeObject<GameStats>(json);
            }
            else
            {
                _stats = new GameStats
                {
                    TotalGames = 0,
                    CompletedGames = 0,
                    BestTimeEasy = TimeSpan.MaxValue,
                    BestTimeMedium = TimeSpan.MaxValue,
                    BestTimeHard = TimeSpan.MaxValue
                };
            }
        }

        private void SaveStats()
        {
            var json = JsonConvert.SerializeObject(_stats);
            File.WriteAllText(_statsFile, json);
        }

        public GameStats GetStats()
        {
            return _stats;
        }

        public void UpdateStats(bool completed, TimeSpan time, DifficultyLevels difficulty)
        {
            _stats.TotalGames++;
            if (completed)
            {
                _stats.CompletedGames++;
                switch (difficulty)
                {
                    case DifficultyLevels.Easy:
                        if (time < _stats.BestTimeEasy)
                            _stats.BestTimeEasy = time;
                        break;
                    case DifficultyLevels.Medium:
                        if (time < _stats.BestTimeMedium)
                            _stats.BestTimeMedium = time;
                        break;
                    case DifficultyLevels.Hard:
                        if (time < _stats.BestTimeHard)
                            _stats.BestTimeHard = time;
                        break;
                }
            }
            SaveStats();
        }

        public void ClearStats()
        {
            _stats = new GameStats
            {
                TotalGames = 0,
                CompletedGames = 0,
                BestTimeEasy = TimeSpan.MaxValue,
                BestTimeMedium = TimeSpan.MaxValue,
                BestTimeHard = TimeSpan.MaxValue
            };
            SaveStats();
        }
    }
} 