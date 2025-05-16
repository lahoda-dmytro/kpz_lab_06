using System.Collections.Generic;
using System.Linq;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class PopulatePuzzle
    {
       

        private readonly CellClass[] _cells = new CellClass[81];

      
        internal CellClass[,] GeneratePuzzle()
        {
            ClearCells(); 
            GenerateGrid(); 
            return TransferGameToGrid();
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
            do
            {
                if (available[index].Count > 0)
                {
                    var i = RandomClass.GetRandomInt(available[index].Count);
                    var item = new CellClass(index, available[index][i]); 
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
                    available[index] = InitArray(); 
                    index--; 
                    _cells[index] = null; 
                }
            } while (index < 81); 
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
            foreach (var item in _cells) 
                cells[item.Col, item.Row] = item;
            return cells;
        }

      
    }
}