using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel;
using System.Diagnostics;

namespace SudokuWPF.Model
{
    public class GameModel
    {
        private readonly CellClass[,] _cells;
        private readonly List<CellClass> _cellList;
        private readonly List<CellClass>[] _regionList;
        private readonly GameValidator _validator;
        private readonly GameSaveLoad _saveLoad;
        private readonly GameStatistics _statistics;
        private DifficultyLevels _gameLevel;
        private readonly Random _random;
        private readonly List<CellIndex> _selectedCells;

        public DifficultyLevels GameLevel
        {
            get => _gameLevel;
            set => _gameLevel = value;
        }

        public GameModel(CellClass[,] cells)
        {
            Debug.WriteLine("\nInitializing GameModel with cells:");
            PrintBoard(cells);

            _cells = new CellClass[9, 9];
            _cellList = new List<CellClass>();
            _regionList = new List<CellClass>[9];
            for (int i = 0; i < 9; i++)
            {
                _regionList[i] = new List<CellClass>();
            }
            _validator = new GameValidator(_cells, _cellList, _regionList);
            _saveLoad = new GameSaveLoad();
            _statistics = new GameStatistics();
            _random = new Random();
            _selectedCells = new List<CellIndex>();

            // Копіюємо клітинки з вхідного масиву
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    _cells[row, col] = new CellClass(col, row);
                    _cells[row, col].Answer = cells[row, col].Answer;
                    _cells[row, col].UserAnswer = cells[row, col].UserAnswer;
                    _cells[row, col].CellState = cells[row, col].CellState;
                    
                    Debug.WriteLine($"Copying cell[{row},{col}]: Answer={_cells[row, col].Answer}, State={_cells[row, col].CellState}");
                    
                    // Копіюємо нотатки
                    for (int i = 0; i < 9; i++)
                    {
                        _cells[row, col].Notes[i].State = cells[row, col].Notes[i].State;
                    }

                    _cellList.Add(_cells[row, col]);
                    _regionList[_cells[row, col].Region].Add(_cells[row, col]);
                }
            }

            Debug.WriteLine("\nGameModel initialization complete. Final state:");
            PrintBoard(_cells);

            // Підраховуємо порожні клітинки
            CountEmpties();
        }

        private void InitializeCells()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    _cells[i, j] = new CellClass(i, j);
                    _cells[i, j].Answer = (i * 3 + j) % 9 + 1; // Тестова сітка: числа від 1 до 9
                    _cells[i, j].UserAnswer = _cells[i, j].Answer;
                    _cells[i, j].CellState = CellStateEnum.Answer;
                    _cellList.Add(_cells[i, j]);
                    _regionList[_cells[i, j].Region].Add(_cells[i, j]);
                }
            }
        }

        public void LoadPuzzle(int[,] puzzle)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i, j] != 0)
                    {
                        _cells[i, j].Answer = puzzle[i, j];
                        _cells[i, j].CellState = CellStateEnum.Answer;
                    }
                }
            }
        }

        private int[,] ConvertCellsToIntArray(CellClass[,] cells)
        {
            var result = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = cells[i, j].Answer;
                }
            }
            return result;
        }

        internal CellClass this[int col, int row]
        {
            get
            {
                if ((_cells != null) && Common.IsValidIndex(col, row)) 
                    return _cells[col, row]; 
                return null;
            }
        }

        internal bool GameComplete => EmptyCount == 0;

        internal List<CellClass> CellList => _cellList;

        internal int EmptyCount { get; set; }

        internal void ComputeNote(int col, int row)
        {
            if (Common.IsValidIndex(col, row)) 
                GenerateNote(_cells[col, row]); 
        }

        internal void ResetPuzzle()
        {
            if (_cells != null) 
            {
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState != CellStateEnum.Answer) 
                    {                  
                        CellList[i].CellState = CellStateEnum.Blank; 
                        CellList[i].UserAnswer = 0; 
                    }
                CountEmpties();
            }
        }

        internal void ShowNotes()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState == CellStateEnum.Blank) 
                        CellList[i].CellState = CellStateEnum.Notes;
        }

        internal void HideNotes()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++)
                    if (CellList[i].CellState == CellStateEnum.Notes) 
                        CellList[i].CellState = CellStateEnum.Blank;
        }

        internal void ShowSolution()
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++) 
                    if ((CellList[i].CellState != CellStateEnum.Answer) &&
                        (CellList[i].CellState != CellStateEnum.UserInputCorrect))
                        CellList[i].CellState = CellStateEnum.Hint;
        }

        internal void HideSolution(bool showAllNotes)
        {
            if (_cells != null) 
                for (var i = 0; i < CellList.Count; i++) 
                    if (CellList[i].CellState == CellStateEnum.Hint) 
                        if (showAllNotes)
                            CellList[i].CellState = CellStateEnum.Notes; 
                        else
                            CellList[i].CellState = CellStateEnum.Blank;
        }

        internal List<CellClass> RegionCells(int index)
        {
            if (Common.IsValidIndex(index) && (_regionList != null))
                return _regionList[index]; 
            return null;
        }

        internal CellClass[,] GetCells()
        {
            return _cells;
        }

        private void GenerateNote(CellClass cell)
        {
            if (cell.CellState != CellStateEnum.Answer) 
            {
                for (var i = 0; i < 9; i++) 
                    cell.Notes[i].State = true;
                for (var i = 0; i < 9; i++) 
                {
                    ProcessNote(cell, _cells[cell.Row, i]); 
                    ProcessNote(cell, _cells[i, cell.Col]); 
                }
                foreach (var item in _regionList[cell.Region]) 
                    ProcessNote(cell, item);
            }
        }

        private void ProcessNote(CellClass targetCell, CellClass sourceCell)
        {
            if (sourceCell.CellState == CellStateEnum.Answer) 
                targetCell.Notes[sourceCell.Answer - 1].State = false; 
        }

        private void CountEmpties()
        {
            EmptyCount = 0; 
            foreach (var item in CellList)
                if (item.CellState == CellStateEnum.Blank) 
                    EmptyCount++; 
        }

        public bool ValidateCell(int col, int row)
        {
            return _validator.ValidateCell(col, row);
        }

        public List<CellClass> GetConflictingCells()
        {
            return _validator.GetConflictingCells();
        }

        public bool IsGameValid()
        {
            return _validator.IsGameValid();
        }

        public List<CellClass> GetHint()
        {
            return _validator.GetHint();
        }

        public async Task SaveGameAsync()
        {
            var gameState = new GameState
            {
                Cells = _cells,
                SaveDate = DateTime.Now,
                Difficulty = GameLevel
            };

            await _saveLoad.SaveGameAsync(gameState);
        }

        public async Task<List<SavedGameInfo>> GetSavedGamesAsync()
        {
            return await _saveLoad.GetSavedGamesAsync();
        }

        public async Task LoadGameAsync(string fileName)
        {
            var gameState = await _saveLoad.LoadGameAsync(fileName);
            LoadPuzzle(ConvertCellsToIntArray(gameState.Cells));
            GameLevel = gameState.Difficulty;
        }

        public GameStats GetStatistics()
        {
            return _statistics.GetStats();
        }

        public CellClass GetCell(CellIndex index)
        {
            return _cells[index.Row, index.Column];
        }

        public void SetCell(CellIndex index, int value)
        {
            if (Common.IsValidAnswer(value))
            {
                _cells[index.Row, index.Column].UserAnswer = value;
            }
        }

        public void SelectCell(CellIndex index)
        {
            if (!_selectedCells.Contains(index))
            {
                _selectedCells.Add(index);
            }
        }

        public void DeselectCell(CellIndex index)
        {
            _selectedCells.Remove(index);
        }

        public bool IsCellSelected(CellIndex index)
        {
            return _selectedCells.Contains(index);
        }

        public void ClearSelection()
        {
            _selectedCells.Clear();
        }

        public IEnumerable<CellIndex> GetSelectedCells()
        {
            return _selectedCells;
        }

        public bool IsValidMove(CellIndex index, int value)
        {
            if (!Common.IsValidAnswer(value))
                return false;

            // Перевірка рядка
            for (int row = 0; row < 9; row++)
            {
                if (row != index.Row && _cells[row, index.Column].UserAnswer == value)
                    return false;
            }

            // Перевірка стовпця
            for (int col = 0; col < 9; col++)
            {
                if (col != index.Column && _cells[index.Row, col].UserAnswer == value)
                    return false;
            }

            // Перевірка регіону
            int regionRow = (index.Row / 3) * 3;
            int regionCol = (index.Column / 3) * 3;
            for (int r = regionRow; r < regionRow + 3; r++)
            {
                for (int c = regionCol; c < regionCol + 3; c++)
                {
                    if (r != index.Row && c != index.Column && _cells[r, c].UserAnswer == value)
                        return false;
                }
            }

            return true;
        }

        public bool IsGameComplete()
        {
            // Перевіряємо наявність нулів
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (_cells[row, col].UserAnswer == 0)
                        return false;
                }
            }

            // Перевіряємо правильність заповнення
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (!IsValidMove(new CellIndex(row, col), _cells[row, col].UserAnswer))
                        return false;
                }
            }

            return true;
        }

        public void ResetGame()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    _cells[row, col].UserAnswer = 0;
                }
            }
            ClearSelection();
        }

        public void PrintUserAnswersToConsole()
        {
            System.Diagnostics.Debug.WriteLine("\nПоточний стан поля (UserAnswer):");
            for (int row = 0; row < 9; row++)
            {
                string line = "";
                for (int col = 0; col < 9; col++)
                {
                    line += _cells[row, col].UserAnswer + " ";
                }
                System.Diagnostics.Debug.WriteLine(line);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        public void PrintAnswersToConsole()
        {
            System.Diagnostics.Debug.WriteLine("\nПравильні відповіді (Answer):");
            for (int row = 0; row < 9; row++)
            {
                string line = "";
                for (int col = 0; col < 9; col++)
                {
                    line += _cells[row, col].Answer + " ";
                }
                System.Diagnostics.Debug.WriteLine(line);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        private void PrintBoard(CellClass[,] cells)
        {
            Debug.WriteLine("\nBoard state:");
            for (int row = 0; row < 9; row++)
            {
                string line = "";
                for (int col = 0; col < 9; col++)
                {
                    line += cells[row, col].Answer + " ";
                }
                Debug.WriteLine(line);
            }
            Debug.WriteLine("");
        }
    }

    public class GameState
    {
        public CellClass[,] Cells { get; set; }
        public DateTime SaveDate { get; set; }
        public DifficultyLevels Difficulty { get; set; }
    }

    public class GameStats
    {
        public int TotalGames { get; set; }
        public int CompletedGames { get; set; }
        public TimeSpan BestTimeEasy { get; set; }
        public TimeSpan BestTimeMedium { get; set; }
        public TimeSpan BestTimeHard { get; set; }
    }
}
