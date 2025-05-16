using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel.GameGenerator.Solver;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class SolveGame
    {
       
        internal bool SolvePuzzle(CellClass[,] cells)
        {
            var iTask = ConvertBoard(cells); 
            var cArena = new SudokuArena(iTask, 3, 3);
            cArena.Solve(); 
            return cArena.Solutions == 1; 
        }

       

        private static int[,] ConvertBoard(CellClass[,] cells)
        {
            var iRet = new int[9, 9]; 
            for (var col = 0; col < 9; col++)
                for (var row = 0; row < 9; row++)
                {
                    if (cells[col, row].CellState == CellStateEnum.Answer)
                        iRet[col, row] = cells[col, row].Answer; 
                    else
                        iRet[col, row] = 0; 
                }
            return iRet; 
        }

       
    }
}