using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
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

        private void StudentDeleteQuery(string studentID)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand("DELETE FROM students WHERE studentID = @studentID", connection);
                command.Parameters.AddWithValue("@studentID", studentID);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                MessageBox.Show($"Error deleting student: {ex.Message}");
            }
        }

        public void RefreshStudentsList()
        {
            string studentListQuery = "SELECT students.studentRfid, students.studentID, students.studentName, " +
                                      "courses.courseName, students.email, students.contactNumber FROM students " +
                                      "INNER JOIN courses ON students.course = courses.courseID;";
            StudentsListQuery(studentListQuery);
        }

        public void StudentListPage_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshStudentsList();
        }

        private void DeleteButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView dataContext)
            {
                var studentID = dataContext["studentID"].ToString();
                var studentName = dataContext["studentName"].ToString();

                // Ask for user confirmation
                var result = MessageBox.Show($"Are you sure you want to delete {studentName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    StudentDeleteQuery(studentID!);
                    RefreshStudentsList();
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }
        }

        private void RegisterModalButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Visible;
            }
            StudentRegisterModal.Instance.ShowDialog();
        }
    }
}
