using SudokuWPF.ViewModel;

namespace SudokuWPF.Model.Structures
{
    public class CellIndex
    {
        internal CellIndex(int col, int row)
        {
            if (Common.IsValidIndex(col, row)) 
            {
                Column = col; 
                Row = row;
                Region = SetRegion(col, row); 
            }
        }

        internal CellIndex(int index)
        {
            if ((0 <= index) && (index <= 80))
            {
                
                index++;
                Column = ComputeColumn(index); 
                Row = ComputeRow(Column, index); 
                Region = SetRegion(Column, Row); 
            }
        }

        internal int Row { get; }

        internal int Column { get; }

        internal int Region { get; }

        internal bool IsSameRow(CellIndex uIndex)
        {
            if (uIndex != null) 
                return uIndex.Row == Row; 
            return false; 
        }

        internal bool IsSameColumn(CellIndex uIndex)
        {
            if (uIndex != null) 
                return uIndex.Column == Column; 
            return false; 
        }

        internal bool IsSameRegion(CellIndex uIndex)
        {
            if (uIndex != null) 
                return uIndex.Region == Region; 
            return false;
        }

        
        private static int ComputeColumn(int index)
        {
            var ret = index%9; 
            if (ret == 0) 
                return 8; 
            return --ret;
        }

        private static int ComputeRow(int col, int index)
        {
            var ret = index/9; 
            if (col == 8) 
                return --ret; 
            return ret; 
        }

        private static int SetRegion(int col, int row)
        {
            if ((0 <= row) && (row <= 2)) 
            {
                if ((0 <= col) && (col <= 2)) 
                    return 0; 
                if ((3 <= col) && (col <= 5)) 
                    return 1;
                return 2;
            }
            if ((3 <= row) && (row <= 5)) 
            {
                if ((0 <= col) && (col <= 2)) 
                    return 3; 
                if ((3 <= col) && (col <= 5))
                    return 4;
                return 5;
            }
            if ((0 <= col) && (col <= 2)) 
                return 6;
            if ((3 <= col) && (col <= 5))
                return 7;
            return 8; 
        }


     
    }
}