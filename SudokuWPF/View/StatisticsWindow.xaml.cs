using System;
using System.Windows;
using SudokuWPF.Model;

namespace SudokuWPF.View
{
    public partial class StatisticsWindow : Window
    {
        private readonly GameStatistics _statistics;

        public StatisticsWindow()
        {
            InitializeComponent();
            _statistics = new GameStatistics();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var stats = _statistics.GetStats();
            
            TotalGamesText.Text = $"Всього ігор: {stats.TotalGames}";
            CompletedGamesText.Text = $"Завершено ігор: {stats.CompletedGames}";

            BestTimeEasyText.Text = $"Легкий рівень: {FormatTimeSpan(stats.BestTimeEasy)}";
            BestTimeMediumText.Text = $"Середній рівень: {FormatTimeSpan(stats.BestTimeMedium)}";
            BestTimeHardText.Text = $"Важкий рівень: {FormatTimeSpan(stats.BestTimeHard)}";
        }

        private string FormatTimeSpan(TimeSpan time)
        {
            if (time == TimeSpan.MaxValue)
                return "Немає запису";
            return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Ви впевнені, що хочете очистити всю статистику?", 
                "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _statistics.ClearStats();
                LoadStatistics();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 