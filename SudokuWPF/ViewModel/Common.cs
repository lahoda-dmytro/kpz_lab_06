using System;
using System.Diagnostics;
using System.Text;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel
{
    public static class Common
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

        public static bool IsValidAnswer(int value)
        {
            return value >= 0 && value <= 9;
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

        public static bool IsSameRegion(CellIndex cell1, CellIndex cell2)
        {
            return cell1.Region == cell2.Region;
        }

        public static bool IsSameRow(CellIndex cell1, CellIndex cell2)
        {
            return cell1.Row == cell2.Row;
        }

        public static bool IsSameColumn(CellIndex cell1, CellIndex cell2)
        {
            return cell1.Column == cell2.Column;
        }
    }
}