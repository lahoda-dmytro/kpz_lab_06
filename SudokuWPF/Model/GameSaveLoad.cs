using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class GameSaveLoad
    {
        private readonly string _saveDirectory;

        public GameSaveLoad()
        {
            _saveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames");
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
        }

        public async Task SaveGameAsync(GameState gameState)
        {
            var fileName = Path.Combine(_saveDirectory, $"game_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            var json = JsonConvert.SerializeObject(gameState, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json);
        }

        public async Task<List<SavedGame>> GetSavedGamesAsync()
        {
            var files = Directory.GetFiles(_saveDirectory, "game_*.json");
            var games = new List<SavedGame>();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var gameState = JsonConvert.DeserializeObject<GameState>(json);
                games.Add(new SavedGame
                {
                    FileName = Path.GetFileName(file),
                    SaveDate = gameState.SaveDate,
                    Difficulty = gameState.Difficulty
                });
            }

            return games.OrderByDescending(g => g.SaveDate).ToList();
        }

        public async Task<GameState> LoadGameAsync(string fileName)
        {
            var filePath = Path.Combine(_saveDirectory, fileName);
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<GameState>(json);
        }
    }

    public class SavedGame
    {
        public string FileName { get; set; }
        public DateTime SaveDate { get; set; }
        public DifficultyLevels Difficulty { get; set; }
    }
} 