using System;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.ViewModel.CustomEventArgs
{
    internal class GameManagerEventArgs : EventArgs
    {
     
        internal GameManagerEventArgs(DifficultyLevels level, int count)
        {
            Level = level; 
            Count = count;
        }

        internal DifficultyLevels Level { get; private set; }

        internal int Count { get; private set; }

     
    }
}