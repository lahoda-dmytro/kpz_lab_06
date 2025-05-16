using System;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.Properties;
using SudokuWPF.ViewModel.CustomEventArgs;

namespace SudokuWPF.ViewModel.GameGenerator
{
    internal class GamesManager
    {
      
        internal GamesManager()
        {
            InitializeClass();
        }

      
        private void GameManagerEventHandler(object sender, GameManagerEventArgs e)
        {
            RaiseEvent(e);
        }

       

        private readonly GameCollection[] _games = new GameCollection[Common.MaxLevels];

      

        internal event EventHandler<GameManagerEventArgs> GamesManagerEvent;

        internal void StopGamesManager()
        {
            StopBackgroundTasks(); 
            SaveGames(); 
        }

        internal CellClass[,] GetGame(DifficultyLevels level)
        {
            return _games[(int) level].GetGame; 
        }

        internal int GameCount(DifficultyLevels level)
        {
            return _games[(int) level].GameCount;
        }

      

        private void InitializeClass()
        {
            InitGameCollectionArray(); 
            LoadGames(); 
            StartBackgroundTasks(); 
        }

        private void InitGameCollectionArray()
        {
            foreach (int i in Enum.GetValues(typeof (DifficultyLevels))) 
                _games[i] = InitGameCollection((DifficultyLevels) i); 
        }

        private GameCollection InitGameCollection(DifficultyLevels level)
        {
            var collection = new GameCollection(level); 
            collection.GameManagerEvent += GameManagerEventHandler; 
            return collection; 
        }

        private void LoadGames()
        {
            _games[0].LoadGames(Settings.Default.GamesLevel0); 
            _games[1].LoadGames(Settings.Default.GamesLevel1);
            _games[2].LoadGames(Settings.Default.GamesLevel2);
            _games[3].LoadGames(Settings.Default.GamesLevel3);
            _games[4].LoadGames(Settings.Default.GamesLevel4);
        }

        private void StartBackgroundTasks()
        {
            foreach (var item in _games)
                item.StartThread(); 
        }

        private void StopBackgroundTasks()
        {
            foreach (var item in _games) 
                item.StopThread();
        }

        private void SaveGames()
        {
            Settings.Default.GamesLevel0 = _games[0].SaveGames(); 
            Settings.Default.GamesLevel1 = _games[1].SaveGames();
            Settings.Default.GamesLevel2 = _games[2].SaveGames();
            Settings.Default.GamesLevel3 = _games[3].SaveGames();
            Settings.Default.GamesLevel4 = _games[4].SaveGames();
            Settings.Default.Save(); 
        }

        private void RaiseEvent(GameManagerEventArgs e)
        {
            var handler = GamesManagerEvent; 
            if (handler != null) 
                handler(this, e); 
        }

    }
}