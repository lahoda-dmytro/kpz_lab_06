namespace SudokuWPF.Model.Structures
{
    public class CellIndex
    {
        public int Column { get; }
        public int Row { get; }
        public int Region => (Column / 3) + (Row / 3) * 3;

        public CellIndex(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsSameRow(CellIndex other)
        {
            return other != null && Row == other.Row;
        }

        public bool IsSameColumn(CellIndex other)
        {
            return other != null && Column == other.Column;
        }

        public bool IsSameRegion(CellIndex other)
        {
            return other != null && Region == other.Region;
        }
    }
} 