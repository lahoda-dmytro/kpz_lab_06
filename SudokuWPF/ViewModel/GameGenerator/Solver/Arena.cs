using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuWPF.ViewModel.GameGenerator.Solver
{
    internal abstract class Arena
    {
       

        private SNode[] _solutionsRows;
        private SColumn[] _headerColumns;

        

        internal Arena(int primary, int secondary)
        {
            InitClass(primary, secondary);

            for (var i = 0; i < primary; i++)
            {
                _solutionsRows[i] = null;
                _headerColumns[i] = new SColumn(i + 1) {Right = null};
                if (i > 0)
                {
                    _headerColumns[i].Left = _headerColumns[i - 1];
                    _headerColumns[i - 1].Right = _headerColumns[i];
                }
            }

            _headerColumns[0].Left = Root;
            Root.Right = _headerColumns[0];
            _headerColumns[primary - 1].Right = Root;
            Root.Left = _headerColumns[primary - 1];
            if (secondary > 0)
                for (var i = 0; i < secondary; i++)
                    _headerColumns[primary + i] = new SColumn(primary + i + 1);
        }

        internal Arena(int columns)
            : this(columns, 0)
        {
        }

      

        private int Initial { get; set; }
        private SColumn Root { get; set; }
        private int Rows { get; set; }
        private SColumn FirstColumn => (SColumn) Root.Right;

     


        internal SNode AddRow(int[] positions)
        {
            SNode result = null;
            if (positions.Length > 0)
            {
                SNode prevNode = null;
                Rows++;
                foreach (var t in positions)
                {
                    if (t > 0)
                    {
                        var thisNode = new SNode(Rows, t)
                        {
                            Left = prevNode,
                            Right = null
                        };
                        if (prevNode != null)
                            prevNode.Right = thisNode;
                        else
                            result = thisNode;
                        var found = false;
                        foreach (var col in _headerColumns.Where(col => col.Col == t))
                        {
                            thisNode.Header = col;
                            col.AddNode(thisNode);
                            found = true;
                        }
                        if (!found)
                            Debug.WriteLine($@"Can't find header for {t}.");
                        prevNode = thisNode;
                        result.Left = prevNode;
                        prevNode.Right = result;
                    }
                }
            }
            return result;
        }

        internal void Solve()
        {
            SolveRecurse(Initial);
        }

        internal void RemoveKnown(List<SNode> solutions)
        {
            foreach (var row in solutions)
            {
                _solutionsRows[Initial] = row;
                Initial++;
                CoverColumn(row.Header);
                var col = row.Right;
                while (Equals(col, row) == false)
                {
                    CoverColumn(col.Header);
                    col = col.Right;
                }
            }
        }

        internal void ShowState()
        {
            var col = FirstColumn;
            while (Equals(col, Root) == false)
            {
                Debug.WriteLine($@"C : {col.ToString()}");
                var row = col.Lower;
                while (Equals(row, col) == false)
                {
                    Debug.WriteLine($@"  R : {row.ToString()}");
                    row = row.Lower;
                }
                col = (SColumn) col.Right;
            }
        }

        internal abstract void HandleSolution(SNode[] rows);

      

        private void InitClass(int primary, int secondary)
        {
            Rows = 0;
            Initial = 0;
            Root = new SColumn(0);

            _solutionsRows = new SNode[primary];
            _headerColumns = new SColumn[primary + secondary];
        }

        private void CoverColumn(SColumn column)
        {
            column.Left.Right = column.Right;
            column.Right.Left = column.Left;
            var row = column.Lower;

            while (Equals(row, column) == false)
            {
                var col = row.Right;
                while (Equals(col, row) == false)
                {
                    col.Upper.Lower = col.Lower;
                    col.Lower.Upper = col.Upper;
                    col.Header.DecRows();
                    col = col.Right;
                }
                row = row.Lower;
            }
        }

        private void UncoverColumn(SColumn column)
        {
            var row = column.Upper;
            while (Equals(row, column) == false)
            {
                var col = row.Left;
                while (Equals(col, row) == false)
                {
                    col.Upper.Lower = col;
                    col.Lower.Upper = col;
                    col.Header.IncRows();
                    col = col.Left;
                }
                row = row.Upper;
            }
            column.Left.Right = column;
            column.Right.Left = column;
        }

        private SColumn NextColumn()
        {
            var result = (SColumn) Root.Right;
            var minRows = result.Rows;
            var scanner = (SColumn) Root.Left;
            while (Equals(scanner, Root.Right) == false)
            {
                if (scanner.Rows < minRows)
                {
                    result = scanner;
                    minRows = scanner.Rows;
                }
                scanner = (SColumn) scanner.Left;
            }
            return result;
        }

        private void SolveRecurse(int index)
        {
            if (Equals(Root, Root.Right))
                HandleSolution(_solutionsRows); 
            else
            {
                var nextCol = NextColumn(); 
                CoverColumn(nextCol); 
                var row = nextCol.Lower; 
                while (Equals(row, nextCol) == false)
                {
                    _solutionsRows[index] = row; 
                    var col = row.Right; 
                    while (Equals(col, row) == false)
                    {
                        CoverColumn(col.Header);
                        col = col.Right;
                    }
                    SolveRecurse(index + 1);

                    col = row.Left;
                    while (Equals(col, row) == false)
                    {
                        UncoverColumn(col.Header);
                        col = col.Left;
                    }
                    _solutionsRows[index] = null; 
                    row = row.Lower;
                }
                UncoverColumn(nextCol); 
            }
        }

     
    }
}