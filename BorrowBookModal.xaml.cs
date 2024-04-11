using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public partial class BorrowBookModal : Window
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;

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

        }

        private void FetchStudentQuery()
        {
            string studentRfid = StudentRfidTxtBox.Text;
            string fetchStudentQuery = $"SELECT students.studentName, courses.courseName, students.email, students.contactNumber FROM students " +
                                       "INNER JOIN courses ON students.course = courses.courseID " +
                                       "WHERE students.studentRfid = @studentRfid;";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(fetchStudentQuery, connection);

            command.Parameters.AddWithValue("@studentRfid", studentRfid);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                NameTxtLabel.Content = reader["studentName"].ToString();
                CourseTxtLabel.Content = reader["courseName"].ToString();
                EmailTxtLabel.Content = reader["email"].ToString();
                long contactNumber = Convert.ToInt64(reader["contactNumber"]);
                string contactNumberString = contactNumber.ToString("D11");
                ContactTxtLabel.Content = contactNumberString;
            }
            else
            {
                MessageBox.Show("This student is not registered!", "Information");
            }

        }

        private void FetchStudentBorrowedBooks()
        {
            string studentRfid = StudentRfidTxtBox.Text;

            string borrowedbookQuery = @"SELECT borrowedbooks.borrowID, books.bookName, borrowedbooks.studentRfid, 
                                       borrowedbooks.borrowDate, borrowedbooks.returnDate FROM borrowedbooks
                                       INNER JOIN books ON borrowedbooks.bookRfid = books.bookRfid
                                       WHERE borrowedbooks.studentRfid = @studentRfid;";

            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(borrowedbookQuery, connection);

            command.Parameters.AddWithValue("@studentRfid", studentRfid);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            BorrowedBookList.ItemsSource = newData.DefaultView;
        }

        private void UpdateBookQuery()
        {
            string bookRfid = BookRfidTxtBox.Text;

            string updateBookQuery = "UPDATE books SET status = 'Borrowed' WHERE bookRfid = @bookRfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(updateBookQuery, connection);

            command.Parameters.AddWithValue("@bookRfid", bookRfid);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private void BorrowBookQuery()
        {
            if (!string.IsNullOrEmpty(StudentRfidTxtBox.Text) && !string.IsNullOrEmpty(BookRfidTxtBox.Text))
            {
                string studentRfid = StudentRfidTxtBox.Text;
                string bookRfid = BookRfidTxtBox.Text;
                DateTime borrowDate = DateTime.Now;
                DateTime returnDate = borrowDate.AddDays(1);

                string insertQuery = $"INSERT INTO borrowedbooks (bookRfid, studentRfid, borrowDate, returnDate) " +
                                     $"VALUES (@bookRfid, @studentRfid, @borrowDate, @returnDate);";

                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(insertQuery, connection);

                command.Parameters.AddWithValue("@bookRfid", bookRfid);
                command.Parameters.AddWithValue("@studentRfid", studentRfid);
                command.Parameters.AddWithValue("@borrowDate", borrowDate);
                command.Parameters.AddWithValue("@returnDate", returnDate);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void StudentRfidTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
           if (StudentRfidTxtBox.Text.Length == 10)
            {
                FetchStudentQuery();
                BookRfidTxtBox.Focus();
            }
        }

        private void BookRfidTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
                FetchStudentBorrowedBooks();
                BorrowBookQuery();
                UpdateBookQuery();

                BookRfidTxtBox.Text = "";
        }

        private void StudentRfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rfidInput = studentRfidRegexInput();

            if (!rfidInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void BookRfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rfidInput = bookRfidRegexInput();

            if (!rfidInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
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
