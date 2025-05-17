namespace SudokuWPF.ViewModel.GameGenerator.Solver
{
    internal class SColumn : SNode
    {
        internal SColumn(int col)
            : base(0, col)
        {
        }

        internal int Rows { get; private set; }

        internal void IncRows()
        {
            Rows++;
        }

        internal void DecRows()
        {
            Rows--;
        }

        internal void AddNode(SNode node)
        {
            var last = Upper;
            node.Upper = last;
            node.Lower = this;
            last.Lower = node;
            Upper = node;
            IncRows();
        }

        public override string ToString()
        {
            return $"{base.ToString()}, rows {Rows}";
        }
    }
}