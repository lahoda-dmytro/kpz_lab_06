using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.ViewModel.CustomEventArgs;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class GameCollection
    {
        internal GameCollection(DifficultyLevels level)
        {
            InitializeVariables(level);
        }

        

        private void GameGeneratorEventHandler(object sender, GameGeneratorEventArgs e)
        {
            var count = 0; 
            lock (_qLock) 
            {
                if (_games == null) 
                    _games = new Queue<CellClass[,]>(); 
                _games.Enqueue(e.Cells);
                count = _games.Count;
            }
            RaiseEvent(count); 
            _makeMoreGames.Set(); 
        }

      

        private const int _cDepth = 5;

       

        private DifficultyLevels _level;
        private Queue<CellClass[,]> _games;
        private object _qLock;
        private bool _stop;
        private AutoResetEvent _makeMoreGames;
        private GameGenerator _cGameGenerator;


        internal event EventHandler<GameManagerEventArgs> GameManagerEvent;

     
        internal int GameCount
        {
            get
            {
                lock (_qLock) 
                {
                    if (_games == null) 
                        _games = new Queue<CellClass[,]>(); 
                    return _games.Count;
                }
            }
        }

        internal CellClass[,] GetGame
        {
            get
            {
                try
                {
                    lock (_qLock)
                    {
                        if (_games == null) 
                            _games = new Queue<CellClass[,]>(); 
                        if (_games.Count > 0) 
                        {
                            var cells = _games.Dequeue(); 
                            RaiseEvent(_games.Count); 
                            _makeMoreGames.Set(); 
                            return cells;
                        }
                    }
                }
                catch (Exception)
                {
                }
                return null; 
            }
        }

        internal void StartThread()
        {
            CreateGames(); 
        }

        internal void StopThread()
        {
            _stop = true; 
            _makeMoreGames.Set(); 
        }

        internal string SaveGames()
        {
            try
            {
                lock (_qLock) 
                {
                    if (_games != null) 
                    {
                        var sTemp = new StringBuilder(); 
                        foreach (var item in _games.Where(item => item != null))
                            sTemp.Append(ConvertGameToString(item)); 
                        return sTemp.ToString(); 
                    }
                    _games = new Queue<CellClass[,]>(); 
                }
            }
            catch (Exception)
            {
            }
            return null; 
        }

        internal void LoadGames(string sGames)
        {
            if (!string.IsNullOrWhiteSpace(sGames)) 
            {
                lock (_qLock) 
                {
                    if (_games == null)
                        _games = new Queue<CellClass[,]>(); 
                    var iPtr = 0; 
                    while (sGames.Length > iPtr) 
                    {
                        var sTemp = sGames.Substring(iPtr, 162); 
                        var cells = ConvertStringToGame(sTemp); 
                        if (cells != null)
                            _games.Enqueue(cells);
                        iPtr += 162; 
                    }
                    RaiseEvent(_games.Count); 
                }
            }
        }

      

        private void InitializeVariables(DifficultyLevels level)
        {
            _level = level; 
            _stop = false; 
            _qLock = new object(); 
            _makeMoreGames = new AutoResetEvent(false); 
            _cGameGenerator = new GameGenerator(level); 
            _cGameGenerator.GameGeneratorEvent += GameGeneratorEventHandler; 
            lock (_qLock) 
            {
                _games = new Queue<CellClass[,]>(); 
            }
        }

        private void CreateGames()
        {
            var t = new Thread(GameMaker) {IsBackground = true};
            t.Start(); 
        }

        private void GameMaker()
        {
            do 
            {
                try
                {
                    lock (_qLock) 
                    {
                        if (_games == null) 
                            _games = new Queue<CellClass[,]>(); 
                        if (!_cGameGenerator.Busy && (_games.Count < _cDepth))
                            _cGameGenerator.CreateNewGame(); 
                    }
                    _makeMoreGames.WaitOne();
                }
                catch (Exception)
                {
                }
            } while (!_stop); 
        }

        private static string ConvertGameToString(CellClass[,] cells)
        {
            var sTemp = new StringBuilder(); 
            for (var col = 0; col < 9; col++) 
                for (var row = 0; row < 9; row++) 
                    sTemp.Append(cells[col, row]); 
            return sTemp.ToString(); 
        }

        private static CellClass[,] ConvertStringToGame(string sInput)
        {
            if (sInput.Length >= 162) 
            {
                var cells = new CellClass[9, 9];
                var iPtr = 0; 
                for (var col = 0; col < 9; col++)
                    for (var row = 0; row < 9; row++)
                    {
                        var sTemp = sInput.Substring(iPtr, 2);
                        cells[col, row] = new CellClass(col, row, sTemp);
                        if (cells[col, row].InvalidState)
                            return null; 
                        iPtr += 2; 
                    }
                return cells; 
            }
            return null; 
        }

        protected virtual void RaiseEvent(int count)
        {
            var handler = GameManagerEvent;
            if (handler != null)
            {
                var e = new GameManagerEventArgs(_level, count);
                handler(this, e);
            }
        }

    }
}