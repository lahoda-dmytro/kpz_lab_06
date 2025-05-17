using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.Model
{
    public class GameSaveLoad
    {
        private readonly string _saveDirectory = "saves";
        private readonly string _savesListFile = "saves.json";

        public GameSaveLoad()
        {
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }
        }

        public async Task SaveGameAsync(GameState gameState)
        {
            string fileName = $"game_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string filePath = Path.Combine(_saveDirectory, fileName);

            string json = JsonConvert.SerializeObject(gameState);
            await Task.Run(() => File.WriteAllText(filePath, json));

            var savedGame = new SavedGameInfo
            {
                FileName = fileName,
                SaveDate = DateTime.Now,
                Difficulty = gameState.Difficulty
            };

            await UpdateSavesListAsync(savedGame);
        }

        public async Task<List<SavedGameInfo>> GetSavedGamesAsync()
        {
            string filePath = Path.Combine(_saveDirectory, _savesListFile);
            if (!File.Exists(filePath))
            {
                return new List<SavedGameInfo>();
            }

            string json = await Task.Run(() => File.ReadAllText(filePath));
            return JsonConvert.DeserializeObject<List<SavedGameInfo>>(json) ?? new List<SavedGameInfo>();
        }

        public async Task<GameState> LoadGameAsync(string fileName)
        {
            string filePath = Path.Combine(_saveDirectory, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Збережена гра не знайдена", fileName);
            }

            string json = await Task.Run(() => File.ReadAllText(filePath));
            return JsonConvert.DeserializeObject<GameState>(json);
        }

        public async Task DeleteGameAsync(string fileName)
        {
            string filePath = Path.Combine(_saveDirectory, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }

            var saves = await GetSavedGamesAsync();
            saves.RemoveAll(s => s.FileName == fileName);
            string json = JsonConvert.SerializeObject(saves);
            await Task.Run(() => File.WriteAllText(Path.Combine(_saveDirectory, _savesListFile), json));
        }

        private async Task UpdateSavesListAsync(SavedGameInfo newSave)
        {
            var saves = await GetSavedGamesAsync();
            saves.Add(newSave);
            string json = JsonConvert.SerializeObject(saves);
            await Task.Run(() => File.WriteAllText(Path.Combine(_saveDirectory, _savesListFile), json));
        }
    }
} 