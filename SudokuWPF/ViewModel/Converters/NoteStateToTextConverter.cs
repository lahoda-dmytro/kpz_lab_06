using System;
using System.Globalization;
using System.Windows.Data;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.ViewModel.Converters
{
    public class NoteStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NoteState note)
            {
                return note.State ? note.Value.Trim() : "";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 