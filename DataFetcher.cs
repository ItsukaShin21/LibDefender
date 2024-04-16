using System.Data;
using System.Numerics;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public static class DataFetcher
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function to fetch all data from LogRecords table to display in DataGrid
        public static void FetchLogRecordsData()
        {
            string logsListQuery = "SELECT students.studentName, students.studentID, courses.courseName, logrecords.timein, logrecords.timeout " +
                       "FROM logrecords " +
                       "INNER JOIN students ON logrecords.studentRfid = students.studentRfid " +
                       "INNER JOIN courses ON students.course = courses.courseID;";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(logsListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            StudentLogPage.CurrentInstance.LogRecordsList.ItemsSource = newData.DefaultView;
        }

        //Function to fetch all data from the students table to display in DataGrid
        public static void FetchStudentsData()
        {
            string studentListQuery = "SELECT students.studentRfid, students.studentID, students.studentName, " +
                                      "courses.courseName, students.email, students.contactNumber FROM students " +
                                      "INNER JOIN courses ON students.course = courses.courseID;";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(studentListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            StudentListPage.CurrentInstance.StudentsList.ItemsSource = newData.DefaultView;
        }

        //Function to fetch all data from the books table to display in DataGrid
        public static void FetchBooks() 
        {
            string booksListQuery = "SELECT * FROM books";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(booksListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            BookListPage.CurrentInstance.BooksList.ItemsSource = newData.DefaultView;
        }

        //Function to fetch all the borrowed books data of the student from the borrowedbooks table to display in DataGrid
        public static void FetchStudentBorrowedBooks(string studentRfid)
        {
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

            BorrowBookModal.CurrentInstance.BorrowedBookList.ItemsSource = newData.DefaultView;
        }

        //Function to fetch all courseName in the courses table to display in the ComboBox
        public static void FetchCourseNames()
        {
            string fetchCourseQuery = "SELECT courseName FROM courses";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(fetchCourseQuery, connection);

            connection.Open();

            DataTable courses = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(courses);

            connection.Close();

            StudentRegisterModal.CurrentInstance.courseComboBox.ItemsSource = courses.DefaultView;
            StudentRegisterModal.CurrentInstance.courseComboBox.DisplayMemberPath = "courseName";
        }

        //Function to fetch the courseID in the courses table
        public static int FetchCourseID(string courseName)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT courseID FROM courses WHERE courseName = @courseName";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@courseName", courseName);

            int courseID = Convert.ToInt32(command.ExecuteScalar());
            return courseID;
        }
    }
}
