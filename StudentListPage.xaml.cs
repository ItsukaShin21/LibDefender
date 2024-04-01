using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public partial class StudentListPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;
        public StudentListPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void StudentsListQuery(string studentListQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(studentListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            StudentsList.ItemsSource = newData.DefaultView;
        }

        private void StudentDeleteQuery(string studentDelete)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(studentDelete, connection);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private void StudentListPage_Loaded(object sender, RoutedEventArgs e)
        {
            string studentListQuery = "SELECT * FROM students";
            StudentsListQuery(studentListQuery);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the data object (the row) associated with the delete button clicked
            if (sender is Button button && button.DataContext is DataRowView dataContext)
            {
                // Extract the studentID from the data object
                var studentID = dataContext["studentID"].ToString();

                // Construct the delete query
                string studentDelete = $"DELETE FROM students WHERE studentID = '{studentID}'";

                // Execute the delete query
                StudentDeleteQuery(studentDelete);

                // Refresh the DataGrid after deletion
                string studentListQuery = "SELECT * FROM students";
                StudentsListQuery(studentListQuery);
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }

        }
    }
}
