using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public partial class BookListPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public BookListPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void BooksListQuery(string bookListQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(bookListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            BooksList.ItemsSource = newData.DefaultView;
        }

        public void RefreshBooksList()
        {
            string booksListQuery = "SELECT * FROM books";
            BooksListQuery(booksListQuery);
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
                    BookDeleteQuery(bookRfid!);
                    RefreshBooksList();
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }
        }

        private void BookDeleteQuery(string bookRfid)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand("DELETE FROM books WHERE bookRfid = @bookRfid", connection);
                command.Parameters.AddWithValue("@bookRfid", bookRfid);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                MessageBox.Show($"Error deleting the book: {ex.Message}");
            }
        }

        private void BookListPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshBooksList();
        }
    }
}
