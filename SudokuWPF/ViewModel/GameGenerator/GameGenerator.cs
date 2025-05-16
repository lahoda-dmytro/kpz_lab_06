using System;
using System.Threading;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel.CustomEventArgs;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class GameGenerator
    {
        internal GameGenerator(DifficultyLevels level)
        {
            _level = level;
        }

        internal bool Busy { get; private set; }

       

        private readonly DifficultyLevels _level;


        internal event EventHandler<GameGeneratorEventArgs> GameGeneratorEvent;

      
        internal void CreateNewGame()
        {
            var t = new Thread(GenerateNewGame) {IsBackground = true}; 
            t.Start(); 
        }

       

        private void GenerateNewGame()
        {
            var cells = GenerateNewBoard(); 
            RaiseEvent(cells); 
        }

        private CellClass[,] GenerateNewBoard()
        {
            try
            {
                Busy = true; 
                CellClass[,] cells; 
                var cPopulate = new PopulatePuzzle();
                var cMask = new MaskPuzzle(_level);
                do
                {
                    cells = cPopulate.GeneratePuzzle(); 
                    cMask.MaskBoard(cells); 
                } while (cMask.NotGood);
                return cells;
            }
            finally
            {
                Busy = false; 
            }
        }

        protected virtual void RaiseEvent(CellClass[,] cells)
        {
            var handler = GameGeneratorEvent;
            if (handler != null)
            {
                var e = new GameGeneratorEventArgs(cells);
                handler(this, e);
            }
        }

     
    }
}