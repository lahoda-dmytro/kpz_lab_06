namespace SudokuWPF.ViewModel.GameGenerator.Solver
{
    internal class SNode
    {
        internal SNode(int row, int col)
        {
            Left = this;
            Right = this;
            Upper = this;
            Lower = this;
            Header = null;
            Row = row;
            Col = col;
        }

        internal SNode Left { get; set; }
        internal SNode Right { get; set; }
        internal SNode Upper { get; set; }
        internal SNode Lower { get; set; }
        internal SColumn Header { get; set; }
        internal int Row { get; set; }
        internal int Col { get; set; }

        public override string ToString()
        {
            return
                $"Node({Name()}), left({Name(Left)}), right({Name(Right)}), upper({Name(Upper)}), lower({Name(Lower)}), header({Name(Header)})";
        }

        private static string Name(SNode node)
        {
            return node == null ? "NULL" : node.Name();
        }

        private string Name()
        {
            return $"R{Row}, C{Col}";
        }
    }
}