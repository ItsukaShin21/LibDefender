using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Data;

namespace LibDefender
{
    public partial class DashboardPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void DashboardPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string StudentCountQuery = "SELECT COUNT(*) AS RegisteredStudents FROM students";
            string BookCountQuery = "SELECT COUNT(*) AS RegisteredBooks FROM books";
            string PendingQuery = "SELECT COUNT(*) AS Pendings FROM books WHERE isBorrowed = 1";
            StudentsCountQuery(StudentCountQuery);
            BooksCountQuery(BookCountQuery);
            PendingCountQuery(PendingQuery);
        }

        private void StudentsCountQuery(string StudentCountQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(StudentCountQuery, connection);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            RegisteredStudents.Content = result;
        }

        private void BooksCountQuery(string BookCountQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(BookCountQuery, connection);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            RegisteredBooks.Content = result;
        }

        private void PendingCountQuery(string PendingQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(PendingQuery, connection);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            PendingReturns.Content = result;
        }
    }
}
