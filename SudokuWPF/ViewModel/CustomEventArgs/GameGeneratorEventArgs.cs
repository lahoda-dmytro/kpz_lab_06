using System;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel.CustomEventArgs
{
    internal class GameGeneratorEventArgs : EventArgs
    {
        
        internal GameGeneratorEventArgs(CellClass[,] cells)
        {
            Cells = cells; 
        }

     
        internal CellClass[,] Cells { get; private set; }

       
    }
}