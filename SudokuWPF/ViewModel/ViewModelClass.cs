using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using SudokuWPF.Model;
using SudokuWPF.Model.Enums;
using SudokuWPF.Model.Structures;
using SudokuWPF.Properties;
using SudokuWPF.View;
using SudokuWPF.ViewModel.CustomEventArgs;
using SudokuWPF.ViewModel.Enums;
using SudokuWPF.ViewModel.GameGenerator;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SudokuWPF.ViewModel
{
    public class ViewModelClass : INotifyPropertyChanged
    {
        private static readonly object _lock = new object();
        private static ViewModelClass _instance;

        private GameTimer _timer;
        private GamesManager _games;
        private string _statusMsg;
        private StartButtonStateEnum _startButtonState;
        private GameModel _model;
        private MainWindow _view;
        private DifficultyLevels _gameLevel;
        private bool _isEnableGameControls;
        private bool _isShowGameGrid;
        private bool _isEnterNotes;
        private bool _isShowSolution;
        private bool _isShowNotes;
        private string _gameTimeElapsed;
        private CellClass _selectedCell;
        private bool _isGameStarted;
        private DifficultyLevels _selectedDifficulty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ElapsedTime
        {
            get
            {
                if (GameInProgress) 
                {
                    return _timer == null ? "" : _timer.ElapsedTime;
                }
                return ""; 
            }
            private set { OnPropertyChanged(); 
            }
        }

        public string GameTimeElapsed
        {
            get { return _gameTimeElapsed; }
            private set { _gameTimeElapsed = $"Судоко вирішено за {value}."; }
        }

        public string StatusMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_statusMsg)) 
                    return ""; 
                return _statusMsg; 
            }
            private set
            {
                _statusMsg = value; 
                OnPropertyChanged(); 
            }
        }

        public StartButtonStateEnum StartButtonState
        {
            get { return _startButtonState; }
            private set
            {
                _startButtonState = value;
                OnPropertyChanged();
            }
        }

        public string GameCountVeryEasy
        {
            get { return GetGameCount(DifficultyLevels.VeryEasy); }
            private set { OnPropertyChanged(); }
        }

        public string GameCountEasy
        {
            get { return GetGameCount(DifficultyLevels.Easy); }
            private set { OnPropertyChanged(); }
        }

        public string GameCountMedium
        {
            get { return GetGameCount(DifficultyLevels.Medium); }
            private set { OnPropertyChanged(); }
        }

        public string GameCountHard
        {
            get { return GetGameCount(DifficultyLevels.Hard); }
            private set { OnPropertyChanged(); }
        }

        public string GameCountExpert
        {
            get { return GetGameCount(DifficultyLevels.Expert); }
            private set { OnPropertyChanged(); }
        }

        public bool IsEnableGameControls
        {
            get { return _isEnableGameControls; }
            private set
            {
                _isEnableGameControls = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowGameGrid
        {
            get { return _isShowGameGrid; }
            private set
            {
                _isShowGameGrid = value;
                OnPropertyChanged();
            }
        }

        #region
        public CellClass Cell00 => _model[0, 0];

        public CellClass Cell10 => _model[1, 0];

        public CellClass Cell20 => _model[2, 0];

        public CellClass Cell30 => _model[3, 0];
        public CellClass Cell40 => _model[4, 0];

        public CellClass Cell50 => _model[5, 0];

        public CellClass Cell60 => _model[6, 0];
        public CellClass Cell70 => _model[7, 0];

        public CellClass Cell80 => _model[8, 0];

       

        public CellClass Cell01 => _model[0, 1];

        public CellClass Cell11 => _model[1, 1];

        public CellClass Cell21 => _model[2, 1];

        public CellClass Cell31 => _model[3, 1];

        public CellClass Cell41 => _model[4, 1];

        public CellClass Cell51 => _model[5, 1];

        public CellClass Cell61 => _model[6, 1];

        public CellClass Cell71 => _model[7, 1];

        public CellClass Cell81 => _model[8, 1];

       

        public CellClass Cell02 => _model[0, 2];

        public CellClass Cell12 => _model[1, 2];

        public CellClass Cell22 => _model[2, 2];

        public CellClass Cell32 => _model[3, 2];

        public CellClass Cell42 => _model[4, 2];

        public CellClass Cell52 => _model[5, 2];

        public CellClass Cell62 => _model[6, 2];

        public CellClass Cell72 => _model[7, 2];

        public CellClass Cell82 => _model[8, 2];

       

        public CellClass Cell03 => _model[0, 3];

        public CellClass Cell13 => _model[1, 3];

        public CellClass Cell23 => _model[2, 3];

        public CellClass Cell33 => _model[3, 3];

        public CellClass Cell43 => _model[4, 3];

        public CellClass Cell53 => _model[5, 3];

        public CellClass Cell63 => _model[6, 3];

        public CellClass Cell73 => _model[7, 3];

        public CellClass Cell83 => _model[8, 3];

      

        public CellClass Cell04 => _model[0, 4];

        public CellClass Cell14 => _model[1, 4];

        public CellClass Cell24 => _model[2, 4];

        public CellClass Cell34 => _model[3, 4];

        public CellClass Cell44 => _model[4, 4];

        public CellClass Cell54 => _model[5, 4];

        public CellClass Cell64 => _model[6, 4];

        public CellClass Cell74 => _model[7, 4];

        public CellClass Cell84 => _model[8, 4];

      

        public CellClass Cell05 => _model[0, 5];

        public CellClass Cell15 => _model[1, 5];

        public CellClass Cell25 => _model[2, 5];

        public CellClass Cell35 => _model[3, 5];

        public CellClass Cell45 => _model[4, 5];

        public CellClass Cell55 => _model[5, 5];

        public CellClass Cell65 => _model[6, 5];

        public CellClass Cell75 => _model[7, 5];

        public CellClass Cell85 => _model[8, 5];

      

        public CellClass Cell06 => _model[0, 6];

        public CellClass Cell16 => _model[1, 6];

        public CellClass Cell26 => _model[2, 6];

        public CellClass Cell36 => _model[3, 6];

        public CellClass Cell46 => _model[4, 6];

        public CellClass Cell56 => _model[5, 6];

        public CellClass Cell66 => _model[6, 6];

        public CellClass Cell76 => _model[7, 6];

        public CellClass Cell86 => _model[8, 6];

      

        public CellClass Cell07 => _model[0, 7];

        public CellClass Cell17 => _model[1, 7];

        public CellClass Cell27 => _model[2, 7];

        public CellClass Cell37 => _model[3, 7];

        public CellClass Cell47 => _model[4, 7];

        public CellClass Cell57 => _model[5, 7];

        public CellClass Cell67 => _model[6, 7];

        public CellClass Cell77 => _model[7, 7];

        public CellClass Cell87 => _model[8, 7];

      

        public CellClass Cell08 => _model[0, 8];

        public CellClass Cell18 => _model[1, 8];

        public CellClass Cell28 => _model[2, 8];

        public CellClass Cell38 => _model[3, 8];

        public CellClass Cell48 => _model[4, 8];

        public CellClass Cell58 => _model[5, 8];

        public CellClass Cell68 => _model[6, 8];

        public CellClass Cell78 => _model[7, 8];

        public CellClass Cell88 => _model[8, 8];

        #endregion

        public DifficultyLevels GameLevel
        {
            get { return _gameLevel; }
            set
            {
                var bLoadNewGame = _gameLevel != value; 
                _gameLevel = value; 
                Settings.Default.Level = _gameLevel.GetHashCode();
                if (bLoadNewGame) 
                    LoadNewGame(); 
                OnPropertyChanged(); 
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool IsEnterNotes
        {
            get { return _isEnterNotes; }
            set
            {
                if (value && (IsShowSolution || IsShowNotes))
                {
                    ShowErrorMessage("Ви можете обрати лише один варіант: 'Олівець', 'Відповіді' або 'Підказки'.");
                    return;
                }
                _isEnterNotes = value;
                OnPropertyChanged("IsEnterNotes");
                
            }
        }

        public bool IsShowNotes
        {
            get { return _isShowNotes; }
            set
            {
                if (value && (IsEnterNotes || IsShowSolution))
                {
                    ShowErrorMessage("Ви можете обрати лише один варіант: 'Олівець', 'Відповіді' або 'Підказки'.");
                    return;
                }
                _isShowNotes = value;
                if (IsValidGame())
                    if (value) 
                        _model.ShowNotes(); 
                    else
                        _model.HideNotes();
                OnPropertyChanged("IsShowNotes");
            }
        }

        public bool IsShowSolution
        {
            get { return _isShowSolution; }
            set
            {
                if (value && (IsEnterNotes || IsShowNotes))
                {
                    ShowErrorMessage("Ви можете обрати лише один варіант: 'Олівець', 'Відповіді' або 'Підказки'.");
                    return;
                }
                _isShowSolution = value;
                if (IsValidGame()) 
                    if (value) 
                        _model.ShowSolution(); 
                    else
                        _model.HideSolution(IsShowNotes);
                OnPropertyChanged("IsShowSolution");
                
            }
        }

        

        private bool GameInProgress { get; set; }

        private void GameTimerEventHandler(object sender, GameTimerEventArgs e)
        {
            ElapsedTime = e.ElapsedTime; 
        }

        private void GamesManagerEventHandler(object sender, GameManagerEventArgs e)
        {
            switch (e.Level) 
            {
                case DifficultyLevels.VeryEasy:
                    GameCountVeryEasy = e.Count.ToString(); 
                    break;

                case DifficultyLevels.Easy: 
                    GameCountEasy = e.Count.ToString();
                    break;

                case DifficultyLevels.Medium: 
                    GameCountMedium = e.Count.ToString(); 
                    break;

                case DifficultyLevels.Hard: 
                    GameCountHard = e.Count.ToString(); 
                    break;

                case DifficultyLevels.Expert: 
                    GameCountExpert = e.Count.ToString(); 
                    break;
            }
        }

        public static ViewModelClass GetInstance(MainWindow window)
        {
            if (_instance == null) 
            {
                lock (_lock) 
                {
                    if (_instance == null)
                    {
                        _instance = new ViewModelClass(); 
                        _instance.InitClass(window);
                    }
                }
            }
            return _instance; 
        }

        internal bool CloseClicked()
        {
            return CloseClick();
        }

        internal void NewClicked()
        {
            LoadNewGame();
        }

        internal void StartClicked()
        {
            StartGame();
        }

        internal void ResetClicked()
        {
            ResetGame();
        }

        internal void PrintClicked()
        {
            
        }

     
        internal void CellClicked(int col, int row)
        {
            ProcessCellClick(col, row);
        }

       

        private void InitClass(MainWindow window)
        {
            Debug.WriteLine("Initialize View Model ...");
            _view = window; 
            _timer = new GameTimer(); 
            _timer.GameTimerEvent += GameTimerEventHandler; 
            _games = new GamesManager(); 
            _games.GamesManagerEvent += GamesManagerEventHandler; 
            GameInProgress = false;
            StartButtonState = StartButtonStateEnum.Disable; 
            EnableGameControls(false, false);
            _model = new GameModel(CreateEmptyCells());
            LoadSettings(); 
        }

        private void EnableGameControls(bool bEnable, bool bShow)
        {
            IsEnableGameControls = bEnable;
            IsShowGameGrid = bShow;
        }

        private void LoadSettings()
        {
            GameLevel = ConvertGameLevel(Settings.Default.Level); 
        }

        private bool CloseClick()
        {
            if (GameInProgress) 
            {
                var iResults = MessageBox.Show("Ви впевнені що хочите залишити гру?", "Sudoku",
                    MessageBoxButton.YesNo);
                if (iResults == MessageBoxResult.Yes) 
                    StopGame();
                else
                    return true; 
            }
            else
                StopGame(); 
            return false; 
        }

        private void StopGame()
        {
            _games.StopGamesManager();
            SaveSettings(); 
        }

        private void SaveSettings()
        {
            Settings.Default.Level = (int) GameLevel; 
            Settings.Default.Save(); 
        }

        private void LoadNewGame()
        {
            try
            {
                var cells = _games.GetGame(GameLevel);
                Debug.WriteLine($"LoadNewGame: Got cells from _games.GetGame({GameLevel})");
                if (cells != null)
                {
                    Debug.WriteLine("LoadNewGame: Cells are not null, creating new model");
                    // Створюємо нову модель з отриманими клітинками
                    _model = new GameModel(cells);
                    
                    StartButtonState = StartButtonStateEnum.Start;
                    StatusMessage = "Готово до початку гри";
                    _isGameStarted = false;
                    OnPropertyChanged(nameof(IsGameStarted));
                    
                    // Гарантуємо, що сітка буде показана
                    EnableGameControls(true, true);
                    UpdateAllCells();

                    // Виводимо початковий стан
                    _model.PrintAnswersToConsole();
                    _model.PrintUserAnswersToConsole();
                }
                else
                {
                    Debug.WriteLine("LoadNewGame: Cells are null!");
                    StatusMessage = "Помилка: Не вдалося завантажити гру";
                    EnableGameControls(false, false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadNewGame: Exception occurred: {ex.Message}");
                StatusMessage = $"Помилка: {ex.Message}";
                EnableGameControls(false, false);
            }
        }

        private void StartGame()
        {
            if (!GameInProgress)
            {
                _timer.StartTimer();
                GameInProgress = true;
                StartButtonState = StartButtonStateEnum.Pause;
                EnableGameControls(true, true);
                StatusMessage = "Гра почалась!";
                _isGameStarted = true;
                OnPropertyChanged(nameof(IsGameStarted));
            }
            else
            {
                _timer.StopTimer();
                GameInProgress = false;
                StartButtonState = StartButtonStateEnum.Resume;
                StatusMessage = "Гра на паузі";
            }
        }

        private void PauseGame()
        {
            StartButtonState = StartButtonStateEnum.Resume; 
            _timer.PauseTimer(); 
            EnableGameControls(false, false); 
        }

        private void ResumeGame()
        {
            StartButtonState = StartButtonStateEnum.Pause; 
            _timer.ResumeTimer();
            EnableGameControls(true, true); 
        }

        private void StartNewGame()
        {
            GameInProgress = true; 
            ShowBoard();
            _timer.StartTimer(); 
            StartButtonState = StartButtonStateEnum.Pause;
            EnableGameControls(true, true);
            UpdateEmptyCount(); 
        }

        private void ShowBoard()
        {
            ClearForm(); 
            UpdateAllCells();
        }

        private void UpdateEmptyCount(int count = 0)
        {
            if (IsValidGame()) 
            {
                _model.EmptyCount += count; 
                StatusMessage = $"{_model.EmptyCount}"; 
                if (_model.GameComplete)
                    GameEnded(true);
            }
        }

        private bool IsValidGame()
        {
            return GameInProgress && (_model != null) && (_model.CellList != null);
        }

        private void ClearForm()
        {
            ClearCheckboxes();
            StatusMessage = ""; 
        }

        private void ClearCheckboxes()
        {
            IsEnterNotes = false;
            IsShowNotes = false;
            IsShowSolution = false;
        }

        private void UpdateAllCells()
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                    OnPropertyChanged($"Cell{row}{col}");
        }

        private void ResetGame()
        {
            if (IsValidGame()) 
            {
                ClearForm(); 
                _timer.ResetTimer();
                _model.ResetPuzzle(); 
                UpdateEmptyCount(); 
            }
        }

      

        private void ProcessCellClick(int col, int row)
        {
            if (IsValidGame() && (_model[row, col].CellState != CellStateEnum.Answer))       
                ProcessNumberPad(col, row, _view.ShowNumberPad()); 
        }

        private void ProcessNumberPad(int col, int row, InputPadStateEnum value)
        {
            switch (value)
            {
                case InputPadStateEnum.HintRaised: 
                    ProcessHint(col, row); 
                    break;

                case InputPadStateEnum.ClearRaised:
                    ProcessClearCell(col, row); 
                    break;

                case InputPadStateEnum.Number1:
                    ProcessNumberPad(col, row, 1); 
                    break;

                case InputPadStateEnum.Number2: 
                    ProcessNumberPad(col, row, 2); 
                    break;

                case InputPadStateEnum.Number3: 
                    ProcessNumberPad(col, row, 3);
                    break;

                case InputPadStateEnum.Number4: 
                    ProcessNumberPad(col, row, 4);
                    break;

                case InputPadStateEnum.Number5: 
                    ProcessNumberPad(col, row, 5); 
                    break;

                case InputPadStateEnum.Number6: 
                    ProcessNumberPad(col, row, 6); 
                    break;

                case InputPadStateEnum.Number7: 
                    ProcessNumberPad(col, row, 7); 
                    break;

                case InputPadStateEnum.Number8: 
                    ProcessNumberPad(col, row, 8); 
                    break;

                case InputPadStateEnum.Number9:
                    ProcessNumberPad(col, row, 9); 
                    break;
            }
        }

        private void ProcessHint(int col, int row)
        {
            _model[col, row].CellState = CellStateEnum.Hint; 
            UpdateEmptyCount(-1); 
        }

        private void ProcessClearCell(int col, int row)
        {
            if (_model[col, row].CellState != CellStateEnum.Answer) 
            {
                if (UndoEmptyCount(_model[col, row])) 
                    UpdateEmptyCount(1); 
                _model[col, row].CellState = CellStateEnum.Blank; 
                _model[col, row].UserAnswer = 0; 
                _model.ComputeNote(col, row); 
            }
        }

        private bool UndoEmptyCount(CellClass cell)
        {
            return (cell.CellState == CellStateEnum.Hint) || (cell.CellState == CellStateEnum.UserInputCorrect);
        }

        private void ProcessNumberPad(int col, int row, int value)
        {
            if (IsEnterNotes) 
                ProcessNotes(col, row, value); 
            else
                ProcessAnswer(col, row, value);
        }

        private void ProcessNotes(int col, int row, int value)
        {
            if (Common.IsValidAnswer(value)) 
                ProcessNotes(_model[col, row], value - 1); 
        }

        private void ProcessNotes(CellClass cell, int index)
        {
            switch (cell.CellState)
            {
                case CellStateEnum.Notes:
                    cell.Notes[index].State = !cell.Notes[index].State;
                    break;

                case CellStateEnum.Blank:
                    cell.ClearNotes(); 
                    cell.CellState = CellStateEnum.Notes; 
                    cell.Notes[index].State = !cell.Notes[index].State;
                    break;
            }
        }

        private void ProcessAnswer(int col, int row, int value)
        {
            _model[col, row].UserAnswer = value;
            if (_model[col, row].CellState == CellStateEnum.UserInputCorrect) 
            {
                ScanNotes(col, row, value); 
                UpdateEmptyCount(-1); 
            }
        }

        private void ScanNotes(int col, int row, int value)
        {
            for (var i = 0; i < 9; i++) 
            {
                CheckNotes(_model[col, i], value - 1); 
                CheckNotes(_model[i, row], value - 1);
            }
            var uList = _model.RegionCells(_model[col, row].Region); 
            if (uList != null) 
                for (var i = 0; i < 9; i++) 
                    if (uList[i] != null) 
                        CheckNotes(uList[i], value - 1);
        }

        private void CheckNotes(CellClass cell, int index)
        {
            if ((cell.CellState == CellStateEnum.Notes) && cell.Notes[index].State)
                cell.Notes[index].State = false; 
        }

        private static DifficultyLevels ConvertGameLevel(int value)
        {
            try
            {
                if (Enum.IsDefined(typeof (DifficultyLevels), value)) 
                    return (DifficultyLevels) value; 
            }
            catch (Exception)
            {
            }
            return DifficultyLevels.VeryEasy;
        }

        private string GetGameCount(DifficultyLevels level)
        {
            if (_games == null) 
                return "0"; 
            return _games.GameCount(level).ToString(); 
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public ObservableCollection<CellClass> Cells { get; }
        public ICommand StartGameCommand { get; }
        public ICommand CellClickCommand { get; }
        public ICommand NumberClickCommand { get; }
        public ICommand HintCommand { get; private set; }
        public ICommand SaveGameCommand { get; private set; }
        public ICommand LoadGameCommand { get; private set; }
        public ICommand ShowStatsCommand { get; private set; }

        private void ExecuteHint(object parameter)
        {
            var hint = _model.GetHint();
            if (hint != null && hint.Any())
            {
                var cell = hint.First();
                MessageBox.Show($"Підказка: спробуйте значення {cell.Notes.Select((n, i) => new { Value = i + 1, IsPossible = n.State }).Where(n => n.IsPossible).Select(n => n.Value).FirstOrDefault()} в клітинці ({cell.Col + 1}, {cell.Row + 1})");
            }
            else
            {
                MessageBox.Show("На жаль, підказок немає");
            }
        }

        private async void ExecuteSaveGame(object parameter)
        {
            try
            {
                await _model.SaveGameAsync();
                MessageBox.Show("Гру успішно збережено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні гри: {ex.Message}");
            }
        }

        private async void ExecuteLoadGame(object parameter)
        {
            try
            {
                var savedGames = await _model.GetSavedGamesAsync();
                if (!savedGames.Any())
                {
                    MessageBox.Show("Немає збережених ігор");
                    return;
                }

                var gameList = new List<string>();
                foreach (var game in savedGames)
                {
                    gameList.Add($"Гра від {game.SaveDate:dd.MM.yyyy HH:mm} - {game.Difficulty}");
                }

                var result = MessageBox.Show(
                    "Збережені ігри:\n" + string.Join("\n", gameList) + "\n\nЗавантажити останню гру?",
                    "Завантаження гри",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await _model.LoadGameAsync(savedGames.First().FileName);
                    MessageBox.Show("Гру успішно завантажено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні гри: {ex.Message}");
            }
        }

        private void ExecuteShowStats(object parameter)
        {
            var stats = _model.GetStatistics();
            var message = $"Статистика гри:\n\n" +
                         $"Всього ігор: {stats.TotalGames}\n" +
                         $"Завершено: {stats.CompletedGames}\n" +
                         $"Найкращий час (Легкий): {stats.BestTimeEasy:hh\\:mm\\:ss}\n" +
                         $"Найкращий час (Середній): {stats.BestTimeMedium:hh\\:mm\\:ss}\n" +
                         $"Найкращий час (Важкий): {stats.BestTimeHard:hh\\:mm\\:ss}";

            MessageBox.Show(message, "Статистика");
        }

        private void LoadGame(GameState gameState)
        {
            _model = new GameModel(gameState.Cells);
            GameLevel = gameState.Difficulty;
            ShowBoard();
            StartGame();
        }

        public List<CellClass> CellList => _model?.CellList;

        public ViewModelClass()
        {
            _model = new GameModel(CreateEmptyCells());
            Cells = new ObservableCollection<CellClass>();
            InitializeCells();

            StartGameCommand = new RelayCommand(param => StartGame(), param => CanStartGame());
            CellClickCommand = new RelayCommand<CellClass>(SelectCell);
            NumberClickCommand = new RelayCommand<int>(SetNumber);
            HintCommand = new RelayCommand(ExecuteHint);
            SaveGameCommand = new RelayCommand(ExecuteSaveGame);
            LoadGameCommand = new RelayCommand(ExecuteLoadGame);
            ShowStatsCommand = new RelayCommand(ExecuteShowStats);

            _selectedDifficulty = DifficultyLevels.Easy;
        }

        private void InitializeCells()
        {
            Cells.Clear();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Cells.Add(_model.GetCell(new CellIndex(row, col)));
                }
            }
        }

        private bool CanStartGame()
        {
            return !_isGameStarted;
        }

        private void SelectCell(CellClass cell)
        {
            if (!_isGameStarted) return;

            _selectedCell = cell;
            OnPropertyChanged(nameof(SelectedCell));
        }

        private void SetNumber(int number)
        {
            if (!_isGameStarted || _selectedCell == null) return;

            if (_model.IsValidMove(_selectedCell.CellIndex, number))
            {
                _model.SetCell(_selectedCell.CellIndex, number);
                OnPropertyChanged(nameof(Cells));

                // Виводимо стан після встановлення числа
                _model.PrintUserAnswersToConsole();

                if (_model.IsGameComplete())
                {
                    _isGameStarted = false;
                    OnPropertyChanged(nameof(IsGameStarted));
                }
            }
        }

        public CellClass SelectedCell
        {
            get => _selectedCell;
            set
            {
                _selectedCell = value;
                OnPropertyChanged();
            }
        }

        public bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                _isGameStarted = value;
                OnPropertyChanged();
            }
        }

        public DifficultyLevels SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                _selectedDifficulty = value;
                OnPropertyChanged();
            }
        }

        private CellClass[,] CreateEmptyCells()
        {
            var cells = new CellClass[9, 9];
            for (int col = 0; col < 9; col++)
                for (int row = 0; row < 9; row++)
                    cells[col, row] = new CellClass(col, row);
            return cells;
        }

        private void GameEnded(bool bShowDialog)
        {
            _timer.StopTimer(); 
            GameInProgress = false; 
            ElapsedTime = ""; 
            if (bShowDialog) 
            {
                EnableGameControls(false, true);
                StartButtonState = StartButtonStateEnum.Disable; 
                StatusMessage = "Puzzle complete!"; 
                GameTimeElapsed = _timer.ElapsedTime; 
                _view.ShowGameCompletedDialog();
            }
            else
            {
                EnableGameControls(false, false); 
                StartButtonState = StartButtonStateEnum.Start; 
            }
        }
    }
}