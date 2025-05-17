using System;
using System.Threading;
using System.Diagnostics;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel.CustomEventArgs;
using System.Collections.Generic;
using System.Linq;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class GameGenerator
    {
        private readonly DifficultyLevels _level;
        private bool _busy;
        private readonly object _busyLock = new object();
        private readonly Random _random = new Random();

        internal GameGenerator(DifficultyLevels level)
        {
            _level = level;
        }

        internal bool Busy
        {
            get
            {
                lock (_busyLock)
                {
                    return _busy;
                }
            }
            private set
            {
                lock (_busyLock)
                {
                    _busy = value;
                }
            }
        }

        internal event EventHandler<GameGeneratorEventArgs> GameGeneratorEvent;

        internal void CreateNewGame()
        {
            if (Busy)
            {
                Debug.WriteLine("CreateNewGame: Already busy generating a game");
                return;
            }
            var t = new Thread(GenerateNewGame) { IsBackground = true };
            t.Start();
        }

        private void GenerateNewGame()
        {
            try
            {
                Busy = true;
                Debug.WriteLine("GenerateNewGame: Starting generation");
                var cells = GenerateNewBoard();
                if (cells != null)
                {
                    Debug.WriteLine("GenerateNewGame: Successfully generated new game");
                    RaiseEvent(cells);
                }
                else
                {
                    Debug.WriteLine("GenerateNewGame: Failed to generate new game");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GenerateNewGame: Exception occurred: {ex.Message}");
            }
            finally
            {
                Busy = false;
            }
        }

        private CellClass[,] GenerateNewBoard()
        {
            try
            {
                Debug.WriteLine("GenerateNewBoard: Starting board generation");
                CellClass[,] cells;
                var cPopulate = new PopulatePuzzle();
                var cMask = new MaskPuzzle(_level);
                var attempts = 0;
                const int maxAttempts = 5;

                do
                {
                    cells = cPopulate.GeneratePuzzle();
                    if (cells != null)
                    {
                        Debug.WriteLine("GenerateNewBoard: Successfully populated puzzle");
                        cMask.MaskBoard(cells);
                        if (!cMask.NotGood)
                        {
                            Debug.WriteLine("GenerateNewBoard: Successfully masked puzzle");
                            // Перевіряємо чи є відповіді в сітці
                            int answers = 0;
                            for (int row = 0; row < 9; row++)
                                for (int col = 0; col < 9; col++)
                                    if (cells[col, row].CellState == CellStateEnum.Answer)
                                        answers++;
                            
                            if (answers > 0)
                            {
                                Debug.WriteLine($"GenerateNewBoard: Found {answers} answers in puzzle");
                                return cells;
                            }
                            else
                            {
                                Debug.WriteLine("GenerateNewBoard: No answers found in puzzle");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("GenerateNewBoard: Masking failed");
                        }
                    }
                    attempts++;
                } while (attempts < maxAttempts);

                Debug.WriteLine("GenerateNewBoard: Failed to generate valid board after multiple attempts");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GenerateNewBoard: Exception occurred: {ex.Message}");
                return null;
            }
        }

        private int[,] GenerateSolution()
        {
            try
            {
                var grid = new int[9, 9];
                var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                // Заповнюємо перший рядок випадковими числами
                for (int col = 0; col < 9; col++)
                {
                    int index = _random.Next(numbers.Count);
                    grid[0, col] = numbers[index];
                    numbers.RemoveAt(index);
                }

                // Заповнюємо решту сітки
                if (!SolveSudoku(grid))
                {
                    Debug.WriteLine("Failed to solve sudoku");
                    return null;
                }

                return grid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GenerateSolution: Exception occurred: {ex.Message}");
                return null;
            }
        }

        private bool SolveSudoku(int[,] grid)
        {
            for (int row = 1; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        var numbers = GetValidNumbers(grid, row, col);
                        if (numbers.Count == 0) return false;

                        foreach (var num in numbers.OrderBy(x => _random.Next()))
                        {
                            grid[row, col] = num;
                            if (SolveSudoku(grid)) return true;
                            grid[row, col] = 0;
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private List<int> GetValidNumbers(int[,] grid, int row, int col)
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Перевіряємо рядок
            for (int c = 0; c < 9; c++)
                if (grid[row, c] != 0)
                    numbers.Remove(grid[row, c]);

            // Перевіряємо стовпець
            for (int r = 0; r < 9; r++)
                if (grid[r, col] != 0)
                    numbers.Remove(grid[r, col]);

            // Перевіряємо квадрат 3x3
            int boxRow = (row / 3) * 3;
            int boxCol = (col / 3) * 3;
            for (int r = boxRow; r < boxRow + 3; r++)
                for (int c = boxCol; c < boxCol + 3; c++)
                    if (grid[r, c] != 0)
                        numbers.Remove(grid[r, c]);

            return numbers;
        }

        private void RemoveNumbers(CellClass[,] puzzle)
        {
            int cellsToRemove;
            switch (_level)
            {
                case DifficultyLevels.Easy:
                    cellsToRemove = 30;
                    break;
                case DifficultyLevels.Medium:
                    cellsToRemove = 40;
                    break;
                case DifficultyLevels.Hard:
                    cellsToRemove = 50;
                    break;
                default:
                    cellsToRemove = 30;
                    break;
            }

            var positions = new List<(int row, int col)>();
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    positions.Add((row, col));

            // Перемішуємо позиції
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = positions[i];
                positions[i] = positions[j];
                positions[j] = temp;
            }

            // Видаляємо числа
            for (int i = 0; i < cellsToRemove; i++)
            {
                var (row, col) = positions[i];
                puzzle[row, col].UserAnswer = 0;
                puzzle[row, col].CellState = CellStateEnum.Blank;
            }
        }

        protected virtual void RaiseEvent(CellClass[,] cells)
        {
            try
            {
                Debug.WriteLine("RaiseEvent: Attempting to raise event");
                var handler = GameGeneratorEvent;
                if (handler != null)
                {
                    var e = new GameGeneratorEventArgs(cells);
                    handler(this, e);
                    Debug.WriteLine("RaiseEvent: Event raised successfully");
                }
                else
                {
                    Debug.WriteLine("RaiseEvent: No event handlers registered");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RaiseEvent: Exception occurred: {ex.Message}");
            }
        }
    }
}