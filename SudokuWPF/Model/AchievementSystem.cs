using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class AchievementSystem
    {
        private readonly DatabaseManager _dbManager;
        private readonly List<Achievement> _achievements;

        public AchievementSystem(DatabaseManager dbManager)
        {
            _dbManager = dbManager;
            _achievements = new List<Achievement>
            {
                new Achievement
                {
                    Id = "first_game",
                    Title = "Перша гра",
                    Description = "Завершіть свою першу гру",
                    IsUnlocked = false
                },
                new Achievement
                {
                    Id = "speed_demon",
                    Title = "Швидкість",
                    Description = "Завершіть гру менш ніж за 5 хвилин",
                    IsUnlocked = false
                },
                new Achievement
                {
                    Id = "perfect_game",
                    Title = "Досконала гра",
                    Description = "Завершіть гру без помилок",
                    IsUnlocked = false
                },
                new Achievement
                {
                    Id = "master",
                    Title = "Майстер",
                    Description = "Завершіть 10 ігор",
                    IsUnlocked = false
                },
                new Achievement
                {
                    Id = "expert",
                    Title = "Експерт",
                    Description = "Завершіть гру на складному рівні",
                    IsUnlocked = false
                }
            };
        }

        public async Task CheckAchievementsAsync(GameModel game, string difficulty, TimeSpan gameTime, int mistakes)
        {
            var stats = await _dbManager.GetStatisticsAsync(difficulty);
            if (stats == null) return;

            var gamesCompleted = (int)stats["GamesCompleted"];
            var bestTime = (TimeSpan?)stats["BestTime"];

            foreach (var achievement in _achievements)
            {
                if (achievement.IsUnlocked) continue;

                switch (achievement.Id)
                {
                    case "first_game":
                        if (gamesCompleted > 0)
                        {
                            achievement.IsUnlocked = true;
                            achievement.UnlockedDate = DateTime.Now;
                        }
                        break;

                    case "speed_demon":
                        if (gameTime.TotalMinutes < 5 && game.GameComplete)
                        {
                            achievement.IsUnlocked = true;
                            achievement.UnlockedDate = DateTime.Now;
                        }
                        break;

                    case "perfect_game":
                        if (mistakes == 0 && game.GameComplete)
                        {
                            achievement.IsUnlocked = true;
                            achievement.UnlockedDate = DateTime.Now;
                        }
                        break;

                    case "master":
                        if (gamesCompleted >= 10)
                        {
                            achievement.IsUnlocked = true;
                            achievement.UnlockedDate = DateTime.Now;
                        }
                        break;

                    case "expert":
                        if (difficulty == "Hard" && game.GameComplete)
                        {
                            achievement.IsUnlocked = true;
                            achievement.UnlockedDate = DateTime.Now;
                        }
                        break;
                }
            }
        }

        public List<Achievement> GetAchievements()
        {
            return _achievements;
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return _achievements.Where(a => a.IsUnlocked).ToList();
        }
    }

    public class Achievement
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedDate { get; set; }
    }
} 