using System;
using System.ComponentModel;
using SudokuWPF.ViewModel;

namespace SudokuWPF.Model.Structures
{
    public class NoteState : INotifyPropertyChanged
    {
     
        internal NoteState(int value)
        {
            if (Common.IsValidAnswer(value)) 
            {
                _stateValue = $" {value} ";
                State = false; 
            }
            else
                throw new Exception("Invalid input."); 
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

       

        private readonly string _stateValue; 
        private bool _state; 

       

        public event PropertyChangedEventHandler PropertyChanged; 

        public string Value
        {
            get
            {
                if (State) 
                    return _stateValue; 
                return "   "; 
            }
        }

    
        public bool State
        {
            get { return _state; 
            }
            set
            {
                _state = value;
                OnPropertyChanged("Value");
            }
        }

        
    }
}