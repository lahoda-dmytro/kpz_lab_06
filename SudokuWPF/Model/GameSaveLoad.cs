using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class GameSaveLoad
    {
        private const string SaveDirectory = "Saves";
        private const string SaveFileExtension = ".sudoku";

        public class GameState
        {
            public CellClass[,] Cells { get; set; }
            public int EmptyCount { get; set; }
            public DateTime SaveTime { get; set; }
            public TimeSpan GameTime { get; set; }
        }

        public static async Task SaveGameAsync(GameModel game, TimeSpan gameTime)
        {
            var gameState = new GameState
            {
                Cells = game.GetCells(),
                EmptyCount = game.EmptyCount,
                SaveTime = DateTime.Now,
                GameTime = gameTime
            };

            var json = JsonSerializer.Serialize(gameState);
            var fileName = $"sudoku_{DateTime.Now:yyyyMMdd_HHmmss}{SaveFileExtension}";

            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            var filePath = Path.Combine(SaveDirectory, fileName);
            await File.WriteAllTextAsync(filePath, json);
        }

        public static async Task<GameState> LoadGameAsync(string fileName)
        {
            var filePath = Path.Combine(SaveDirectory, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Save file not found", filePath);
            }

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<GameState>(json);
        }

        public static string[] GetSaveFiles()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(SaveDirectory, $"*{SaveFileExtension}")
                          .Select(Path.GetFileName)
                          .ToArray();
        }
    }
} 