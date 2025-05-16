using System;
using System.Diagnostics;
using System.Text;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel
{
    internal class Common
    {
       

        internal const int MaxLevels = 5;
        internal static bool IsValidIndex(int index)
        {
            return (0 <= index) && (index <= 8);
        }

        internal static bool IsValidIndex(int col, int row)
        {
            return IsValidIndex(col) && IsValidIndex(row);
        }

        internal static bool IsValidIndex(CellIndex uIndex)
        {
            if (uIndex != null)
                if (IsValidIndex(uIndex.Column, uIndex.Row))
                    return IsValidIndex(uIndex.Region);
            return false;
        }

        internal static bool IsValidAnswer(int value)
        {
            return (1 <= value) && (value <= 9);
        }

        internal static bool IsValidStateEnum(object value)
        {
            return Enum.IsDefined(typeof (CellStateEnum), value);
        }

        
        internal static bool IsValidGameLevel(object value)
        {
            return Enum.IsDefined(typeof (DifficultyLevels), value);
        }

       
        internal static void PrintGrid(CellClass[,] cells)
        {
            for (var col = 0; col < 9; col++)
            {
                var sTemp = new StringBuilder();
                for (var row = 0; row < 9; row++)
                    sTemp.AppendFormat("{0} ", cells[col, row].Answer);
                Debug.WriteLine("{0}) {1}", col, sTemp);
            }
        }

        
    }
}