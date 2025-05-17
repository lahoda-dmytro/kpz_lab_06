using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SudokuWPF.Model;
using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    public partial class SavedGamesWindow : Window
    {
        private readonly GameSaveLoad _saveLoad;
        private List<SavedGameInfo> _savedGames;

        public SavedGamesWindow()
        {
            InitializeComponent();
            _saveLoad = new GameSaveLoad();
            LoadSavedGames();
        }

        private async void LoadSavedGames()
        {
            _savedGames = await _saveLoad.GetSavedGamesAsync();
            SavedGamesGrid.ItemsSource = _savedGames;
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = SavedGamesGrid.SelectedItem as SavedGameInfo;
            if (selectedGame == null)
            {
                MessageBox.Show("Будь ласка, виберіть гру для завантаження", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var viewModel = ViewModelClass.GetInstance(Application.Current.MainWindow as MainWindow);
                await viewModel.LoadGameAsync(selectedGame.FileName);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при завантаженні гри: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = SavedGamesGrid.SelectedItem as SavedGameInfo;
            if (selectedGame == null)
            {
                MessageBox.Show("Будь ласка, виберіть гру для видалення", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Ви впевнені, що хочете видалити цю збережену гру?", 
                "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _saveLoad.DeleteGameAsync(selectedGame.FileName);
                    LoadSavedGames();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при видаленні гри: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 