using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public partial class BorrowBookPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public BorrowBookPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void BorrowedBookListQuery(string borrowedBookListQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(borrowedBookListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            BorrowedBooksList.ItemsSource = newData.DefaultView;
        }

        public void RefreshBorrowedBookList()
        {
            string borrowedBookListQuery = "SELECT borrowedbooks.borrowID,books.bookName,students.studentName," +
                                           "borrowedbooks.borrowDate,borrowedbooks.returnDate FROM borrowedbooks " +
                                           "JOIN books ON borrowedbooks.bookRfid = books.bookRfid " +
                                           "JOIN students ON borrowedbooks.studentRfid = students.studentRfid;";
            BorrowedBookListQuery(borrowedBookListQuery);
        }

        private void BorrowBookPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshBorrowedBookList();
        }

        private void BorrowBookModalButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Visible;
            }
            BorrowBookModal.Instance.ShowDialog();
        }
    }
}
