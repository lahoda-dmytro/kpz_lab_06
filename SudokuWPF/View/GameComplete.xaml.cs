

using System.Windows;
using SudokuWPF.ViewModel;

namespace SudokuWPF.View
{
    public partial class GameComplete : Window
    {
      
        public GameComplete(ViewModelClass vm)
        {
            InitializeComponent();

            DataContext = vm; // Set the data context for this window
        }

        

        

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close(); // OK button pressed.  Close the window.
        }

   
    }
}