using System;
using System.Collections.Generic;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class MaskPuzzle
    {
        internal MaskPuzzle(DifficultyLevels Level)
        {
            _level = Level;
        }

        internal bool NotGood { get; private set; }

        private readonly int _cMaxInterations = 10;

        private readonly DifficultyLevels _level;

        internal void MaskBoard(CellClass[,] cells)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[MaskPuzzle] Starting board masking");
                var list = TransferToList(cells);
                var mask = 81 - GetMaskedValue(_level);
                System.Diagnostics.Debug.WriteLine($"[MaskPuzzle] Target cells to remove: {mask}");
                var cSolve = new SolveGame();
                var numIterations = 0;
                NotGood = false;

                do
                {
                    var index1 = FindRandomCell(list);
                    var index2 = GetMirror(index1);
                    
                    // Зберігаємо поточний стан клітинок
                    var currentState1 = cells[index1.Column, index1.Row].CellState;
                    var currentState2 = cells[index2.Column, index2.Row].CellState;

                    // Пробуємо видалити обидві клітинки
                    cells[index1.Column, index1.Row].CellState = CellStateEnum.Blank;
                    cells[index2.Column, index2.Row].CellState = CellStateEnum.Blank;

                    if (cSolve.SolvePuzzle(cells))
                    {
                        mask -= RemoveCells(list);
                        System.Diagnostics.Debug.WriteLine($"[MaskPuzzle] Successfully removed cells at [{index1.Column},{index1.Row}] and [{index2.Column},{index2.Row}]. Remaining cells to remove: {mask}");
                        numIterations = 0;
                    }
                    else
                    {
                        // Відновлюємо стан клітинок
                        cells[index1.Column, index1.Row].CellState = currentState1;
                        cells[index2.Column, index2.Row].CellState = currentState2;
                        
                        numIterations++;
                        System.Diagnostics.Debug.WriteLine($"[MaskPuzzle] Failed to remove cells. Attempt {numIterations}/{_cMaxInterations}");
                        
                        if (numIterations > _cMaxInterations)
                        {
                            NotGood = true;
                            System.Diagnostics.Debug.WriteLine("[MaskPuzzle] Max iterations reached, stopping");
                            break;
                        }
                    }
                } while (mask > 0);

                if (mask > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[MaskPuzzle] Failed to remove all required cells. Remaining: {mask}");
                    NotGood = true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[MaskPuzzle] Successfully masked the board");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MaskPuzzle] Exception occurred: {ex.Message}");
                NotGood = true;
            }
        }

        private static List<CellClass> TransferToList(CellClass[,] cells)
        {
            var retList = new List<CellClass>(81);
            for (var col = 0; col < 9; col++)
                for (var row = 0; row < 9; row++)
                    retList.Add(cells[col, row]);
            return retList;
        }

        private static CellIndex GetMirror(CellIndex index)
        {
            var col = 8 - index.Column;
            return new CellIndex(col, index.Row);
        }

        private static CellIndex FindRandomCell(List<CellClass> list)
        {
            var index = RandomClass.GetRandomInt(0, list.Count - 1);
            return list[index].CellIndex;
        }

        private static int RemoveCells(List<CellClass> cells)
        {
            var index = FindFirstMaskedCell(cells);
            if (index < 0) return 0;
            cells.RemoveAt(index);
            var count = 1;
            index = FindFirstMaskedCell(cells);
            if (index >= 0)
            {
                cells.RemoveAt(index);
                count++;
            }
            return count;
        }

        private static int FindFirstMaskedCell(List<CellClass> cells)
        {
            for (var i = 0; i < cells.Count; i++)
                if (cells[i].CellState == CellStateEnum.Blank)
                    return i;
            return -1;
        }

        private static int GetMaskedValue(DifficultyLevels level)
        {
            int min;
            int max;
            switch (level)
            {
                case DifficultyLevels.VeryEasy:
                    min = 50;
                    max = 60;
                    break;

                case DifficultyLevels.Easy:
                    min = 36;
                    max = 49;
                    break;

                case DifficultyLevels.Medium:
                    min = 32;
                    max = 35;
                    break;

                case DifficultyLevels.Hard:
                    min = 28;
                    max = 31;
                    break;

                default:
                    min = 22;
                    max = 27;
                    break;
            }
            return RandomClass.GetRandomInt(min, max);
        }
    }
}