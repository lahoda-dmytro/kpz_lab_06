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
            var list = TransferToList(cells); 
            var mask = 81 - GetMaskedValue(_level);
            var cSolve = new SolveGame(); 
            var numIterations = 0; 
            NotGood = false; 
            do
            {
                var index1 = FindRandomCell(list);
                var index2 = GetMirror(index1); 
                SetCellState(cells, index1, index2, CellStateEnum.Blank);
                if (cSolve.SolvePuzzle(cells)) 
                {
                    mask -= RemoveCells(list);
                    numIterations = 0; 
                }
                else
                {
                    SetCellState(cells, index1, index2, CellStateEnum.Answer); 
                    numIterations++; 
                    if (numIterations > _cMaxInterations) 
                    {
                        NotGood = true; 
                        break; 
                    }
                }
            } while (mask > 0); 
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
            var index = RandomClass.GetRandomInt(list.Count - 1);
            return list[index].CellIndex; 
        }

        private static void SetCellState(CellClass[,] cells, CellIndex index1, CellIndex index2, CellStateEnum state)
        {
            cells[index1.Column, index1.Row].CellState = state; 
            cells[index2.Column, index2.Row].CellState = state; 
        }

        private static int RemoveCells(List<CellClass> cells)
        {
            var index = FindFirstMaskedCell(cells); 
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