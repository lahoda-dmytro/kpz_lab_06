using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using SudokuWPF.Model.Enums;
using SudokuWPF.ViewModel;

namespace SudokuWPF.Model.Structures
{
    public class CellClass : INotifyPropertyChanged
    {
   

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private int _userAnswer; 
        private CellStateEnum _cellState; 


        public event PropertyChangedEventHandler PropertyChanged;

        internal CellClass(int col, int row, string state)
        {
            InitCell(col, row); 
            LoadState(state); 
        }

     
        internal CellClass(int index, int answer)
        {
            InitCell(); 
            CellIndex = new CellIndex(index); 
            Answer = answer; 
            CellState = CellStateEnum.Answer; 
        }

        public CellStateEnum CellState
        {
            get { return _cellState; }
            set
            {
                _cellState = value; 
                OnPropertyChanged(); 
            }
        }

        public int UserAnswer
        {
            get { return _userAnswer; }
            set
            {
                if (Common.IsValidAnswer(value)) 
                {
                    _userAnswer = value;
                    if (_userAnswer == Answer)
                        CellState = CellStateEnum.UserInputCorrect; 
                    else
                        CellState = CellStateEnum.UserInputIncorrect; 
                    OnPropertyChanged(); 
                }
            }
        }

       
        public int Answer { get; private set; }

        public bool InvalidState { get; private set; }

        public CellIndex CellIndex { get; private set; }

        public int Row => CellIndex.Row;

     
        public int Col => CellIndex.Column;

        public int Region => CellIndex.Region;

        public string CellName => $"Cell{CellIndex.Column}{CellIndex.Row}";

        public NoteState[] Notes { get; } = new NoteState[9];

        internal void ClearNotes()
        {
            for (var i = 0; i < 9; i++) 
                if (Notes[i] == null) 
                    Notes[i] = new NoteState(i + 1); 
                else
                    Notes[i].State = false; 
        }

        internal bool IsSameRow(CellClass cell)
        {
            if (cell != null)
                return CellIndex.IsSameRow(cell.CellIndex); 
            return false;
        }

        internal bool IsSameCol(CellClass cell)
        {
            if (cell != null) 
                return CellIndex.IsSameColumn(cell.CellIndex);
            return false; 
        }

        internal bool IsSameRegion(CellClass cell)
        {
            if (cell != null) 
                return CellIndex.IsSameRegion(cell.CellIndex); 
            return false; 
        }

        public override string ToString()
        {
            return ToString(false);
        }

        internal string ToString(bool includeUserAnswer)
        {
            var sTemp = new StringBuilder(); 
            sTemp.Append(Answer.ToString());
            sTemp.Append(CellState.GetHashCode()); 
            if (includeUserAnswer)
                sTemp.Append(UserAnswer.ToString());
            return sTemp.ToString(); 
        }

        private void InitCell()
        {
            Answer = 0;
            UserAnswer = 0; 
            CellState = CellStateEnum.Blank; 
            ClearNotes();
        }

        private void InitCell(int col, int row)
        {
            InitCell(); 
            CellIndex = new CellIndex(col, row); 
        }

        private void LoadState(string state)
        {
            InvalidState = true;
            if (state.Length >= 2) 
                if (ExtractAnswer(state.Substring(0, 1))) 
                    if (ExtractCellState(state.Substring(1, 1)))
                        if (state.Length >= 3) 
                        {
                            if (ExtractUserAnswer(state.Substring(2, 1))) 
                                InvalidState = false; 
                        }
                        else 
                            InvalidState = !((CellState == CellStateEnum.Answer) || (CellState == CellStateEnum.Blank));
        }

        private bool ExtractAnswer(string state)
        {
            Answer = ConvertAnswerToInt32(state); 
            return Answer != 0; 
        }

        private bool ExtractUserAnswer(string state)
        {
            UserAnswer = ConvertAnswerToInt32(state); 
            return UserAnswer != 0; 
        }

        private static int ConvertAnswerToInt32(string value)
        {
            int temp;
            var results = int.TryParse(value, out temp); 
            if (results && Common.IsValidAnswer(temp)) 
                return temp; 
            return 0; 
        }

        private bool ExtractCellState(string state)
        {
            try
            {
                var eState = (CellStateEnum) Enum.Parse(typeof (CellStateEnum), state);
                if (Common.IsValidStateEnum(eState)) 
                {
                    CellState = eState; 
                    return true; 
                }
            }
            catch (Exception)
            {
            }
            CellState = CellStateEnum.Blank; 
            return false; 
        }

        
    }
}