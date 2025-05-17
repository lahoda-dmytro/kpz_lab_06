using SudokuWPF.Model.Enums;

namespace SudokuWPF.Model
{
    public class SavedGameInfo
    {
        public string FileName { get; set; }
        public System.DateTime SaveDate { get; set; }
        public DifficultyLevels Difficulty { get; set; }
    }
} 