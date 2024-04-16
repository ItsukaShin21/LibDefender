using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace LibDefender
{
    public partial class BorrowBookModal : Window
    {
        public static BorrowBookModal CurrentInstance { get; private set; } = null!;

        [GeneratedRegex("[0-9]+")]
        private static partial Regex studentRfidRegexInput();
        [GeneratedRegex("[0-9]+")]
        private static partial Regex bookRfidRegexInput();

        private static BorrowBookModal? newInstance;

        public static BorrowBookModal Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new BorrowBookModal();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }

        public BorrowBookModal()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow as AdminWindow;
            CurrentInstance = this;
        }

        //Function to validate the Regex of the textboxes
        private static void RegexValidation(TextCompositionEventArgs e, Regex regex)
        {
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void StudentRfidTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
           if (StudentRfidTxtBox.Text.Length == 10)
            {
                StudentManager.FetchStudentData(StudentRfidTxtBox.Text);
                DataFetcher.FetchStudentBorrowedBooks(StudentRfidTxtBox.Text);
            }
        }

        private void BookRfidTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DataFetcher.FetchStudentBorrowedBooks(StudentRfidTxtBox.Text);
           
            if (!string.IsNullOrEmpty(StudentRfidTxtBox.Text) && !string.IsNullOrEmpty(BookRfidTxtBox.Text))
            {
                BookManager.UpdateBook((BookRfidTxtBox.Text), (StudentRfidTxtBox.Text));
            }
        }

        private void StudentRfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            RegexValidation(e, studentRfidRegexInput());
        }

        private void BookRfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            RegexValidation(e, bookRfidRegexInput());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Hidden;
            }
        }
    }
}
