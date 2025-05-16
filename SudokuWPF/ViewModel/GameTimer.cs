using System;
using System.Timers;
using SudokuWPF.Properties;
using SudokuWPF.ViewModel.CustomEventArgs;

namespace SudokuWPF.ViewModel
{
    internal class GameTimer
    {
     
        internal GameTimer()
        {
            ElapsedTime = _initialValue; 
        }

       
        internal string ElapsedTime { get; private set; }

     



        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ComputeElapsedTime();
            RaiseEvent(ElapsedTime);
        }

      

        private readonly string _initialValue = "00:00:00";
        private readonly string _timeFormat = "hh\\:mm\\:ss";
        private readonly int _interval = 1000;

       

        private DateTime _startTime;
        private Timer _timer;

       
        internal event EventHandler<GameTimerEventArgs> GameTimerEvent;

        internal void StartTimer()
        {
            _startTime = DateTime.Now; 
            if (_timer == null) 
                _timer = new Timer(_interval);
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true; 
            _timer.Enabled = true; 
            RaiseEvent(_initialValue); 
        }

        internal void StopTimer()
        {
            try
            {
                if ((_timer != null) && _timer.Enabled) 
                {
                    _timer.Enabled = false; 
                    ComputeElapsedTime(); 
                    RaiseEvent("");
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _timer = null; 
            }
        }

        internal void PauseTimer()
        {
            if ((_timer != null) && _timer.Enabled) 
                _timer.Enabled = false; 
        }

        internal void ResumeTimer()
        {
            if (_timer != null) 
            {
                LoadPreviousTime(); 
                _timer.Enabled = true;
            }
        }

        internal void LoadPreviousTime()
        {
            var diff = Settings.Default.ElapsedTime; 
            _startTime = DateTime.Now - diff;
        }

        internal void ResetTimer()
        {
            if (_timer != null) 
            {
                _timer.Enabled = false; 
                _startTime = DateTime.Now;
                _timer.Enabled = true; 
                RaiseEvent(_initialValue); 
            }
        }

        private void ComputeElapsedTime()
        {
            try
            {
                var diff = DateTime.Now - _startTime; 
                Settings.Default.ElapsedTime = diff;
                ElapsedTime = diff.ToString(_timeFormat); 
            }
            catch (Exception)
            {
                ElapsedTime = _initialValue; 
            }
        }

        protected virtual void RaiseEvent(string value)
        {
            var handler = GameTimerEvent;
            if (handler != null)
            {
                var e = new GameTimerEventArgs(value);
                handler(this, e);
            }
        }

       
    }
}