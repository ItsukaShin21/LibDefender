using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class BookListPage : Page
    {
        public static BookListPage CurrentInstance { get; private set; } = null!;

        public BookListPage()
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentInstance = this;
        }

        private void DeleteButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView dataContext)
            {
                var bookRfid = dataContext["bookRfid"].ToString();
                var bookName = dataContext["bookName"].ToString();

                // Ask for user confirmation
                var result = MessageBox.Show($"Are you sure you want to delete {bookName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    BookManager.DeleteBook(bookRfid!);
                    DataFetcher.FetchBooks();
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }
        }

        private void BookListPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            progressRing.IsActive = true;

            DataFetcher.FetchBooks();
            BookManager.RefreshBookStatus();

            progressRing.IsActive = false;
        }

        private void BookRegisterModalButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Visible;
            }
            BookRegisterModal.Instance.ShowDialog();
        }
    }
}
