using System.Collections.Generic;
using System.Linq;
using SudokuWPF.Model.Structures;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class PopulatePuzzle
    {
       

        private readonly CellClass[] _cells = new CellClass[81];

      
        internal CellClass[,] GeneratePuzzle()
        {
            ClearCells(); 
            GenerateGrid(); 
            var grid = TransferGameToGrid();
            // Дебаг-лог: підрахунок клітинок Answer
            int answers = 0;
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    if (grid[row, col] != null && grid[row, col].CellState == CellStateEnum.Answer) answers++;
            System.Diagnostics.Debug.WriteLine($"[PopulatePuzzle] Answers: {answers}");
            // Дебаг-лог: вивід сітки після генерації
            System.Diagnostics.Debug.WriteLine("[DEBUG] Після генерації:");
            for (int row = 0; row < 9; row++) {
                string line = "";
                for (int col = 0; col < 9; col++) {
                    line += grid[row, col].Answer + " ";
                }
                System.Diagnostics.Debug.WriteLine(line);
            }
            return grid;
        }

      

        private void ClearCells()
        {
            for (var i = 0; i < 81; i++) 
                _cells[i] = null; 
        }

        private void GenerateGrid()
        {
            var available = InitializeArray();
            var index = 0;
            var maxAttempts = 1000;
            var attempts = 0;

            while (index < 81 && attempts < maxAttempts)
            {
                attempts++;
                if (available[index].Count > 0)
                {
                    var i = RandomClass.GetRandomInt(0, available[index].Count - 1); // Використовуємо правильний діапазон
                    int row = index / 9;
                    int col = index % 9;
                    var item = new CellClass(col, row, available[index][i]);
                    if (Conflicts(item))
                    {
                        available[index].RemoveAt(i);
                        item = null;
                    }
                    else
                    {
                        _cells[index] = item;
                        item = null;
                        available[index].RemoveAt(i);
                        index++;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        available[index] = InitArray();
                        index--;
                        _cells[index] = null;
                    }
                    else
                    {
                        ClearCells();
                        available = InitializeArray();
                        index = 0;
                    }
                }
            }

            if (attempts >= maxAttempts)
            {
                System.Diagnostics.Debug.WriteLine("[PopulatePuzzle] Max attempts reached, using simple puzzle");
                GenerateSimplePuzzle();
            }
        }

        private void GenerateSimplePuzzle()
        {
            int[] basePattern = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < 81; i++)
            {
                int row = i / 9;
                int col = i % 9;
                int value = basePattern[(row + col) % 9];
                _cells[i] = new CellClass(col, row, value);
            }
        }

        private static List<int>[] InitializeArray()
        {
            var available = new List<int>[81]; 
            for (var i = 0; i < 81; i++)
                available[i] = InitArray(); 
            return available; 
        }

        private static List<int> InitArray()
        {
            var retVal = new List<int>(); 
            for (var j = 1; j <= 9; j++) 
                retVal.Add(j); 
            return retVal; 
        }

        private bool Conflicts(CellClass check)
        {
            return
                _cells.Where(item => item != null)
                    .Any(
                        item =>
                            (item.IsSameRow(check) || item.IsSameCol(check) || item.IsSameRegion(check)) &&
                            (item.Answer == check.Answer));
        }

        private CellClass[,] TransferGameToGrid()
        {
            var cells = new CellClass[9, 9];
            for (int i = 0; i < 81; i++)
            {
                int row = i / 9;
                int col = i % 9;
                if (_cells[i] != null)
                {
                    cells[col, row] = new CellClass(col, row);
                    cells[col, row].Answer = _cells[i].Answer;
                    cells[col, row].UserAnswer = _cells[i].Answer;
                    cells[col, row].CellState = CellStateEnum.Answer;
                }
                else
                {
                    cells[col, row] = new CellClass(col, row);
                    cells[col, row].CellState = CellStateEnum.Blank;
                    cells[col, row].UserAnswer = 0;
                }
            }
            return cells;
        }

      
    }
}