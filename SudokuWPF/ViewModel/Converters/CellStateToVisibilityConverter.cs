using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SudokuWPF.Model.Enums;

namespace SudokuWPF.ViewModel.Converters
{
    public class CellStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CellStateEnum state)
            {
                return state == CellStateEnum.Answer || state == CellStateEnum.UserInputCorrect || state == CellStateEnum.Hint
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 