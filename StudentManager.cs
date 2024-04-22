using MySql.Data.MySqlClient;
using System.Windows;

namespace LibDefender
{
    public static class StudentManager
    {
        readonly static string connectionString = DatabaseConfig.systemDatabase;

        //Function for deleting a student data
        public static void DeleteStudent(string studentID)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand("DELETE FROM students WHERE studentID = @studentID", connection);
            command.Parameters.AddWithValue("@studentID", studentID);

            connection.Open();
            command.ExecuteNonQuery();
        }

        //Function to fetch the data of the selected student to display in the student information box
        public static void FetchStudentData(string studentRfid)
        {
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
                BorrowBookModal.CurrentInstance.NameTxtLabel.Content = reader["studentName"].ToString();
                BorrowBookModal.CurrentInstance.CourseTxtLabel.Content = reader["courseName"].ToString();
                BorrowBookModal.CurrentInstance.EmailTxtLabel.Content = reader["email"].ToString();
                long contactNumber = Convert.ToInt64(reader["contactNumber"]);
                string contactNumberString = contactNumber.ToString("D11");
                BorrowBookModal.CurrentInstance.ContactTxtLabel.Content = contactNumberString;
                BorrowBookModal.CurrentInstance.BookRfidTxtBox.Focus();
            }
            else
            {
                MessageBox.Show("This student is not registered!", "Information");
                BorrowBookModal.CurrentInstance.StudentRfidTxtBox.Focus();
                BorrowBookModal.CurrentInstance.StudentRfidTxtBox.Clear();
            }
        }

        //Function for inserting a student data in the students table
        public static void InsertStudent(string rfidUID, string studentID, string studentName, int course, string email, string contactNumber)
        {
            string insertStudentQuery = "INSERT INTO students (studentRfid, studentID, studentName, course, email, contactNumber)" +
                           "VALUES (@rfidUID, @studentID, @studentName, @course, @email, @contactNumber)";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(insertStudentQuery, connection);

            command.Parameters.AddWithValue("@rfidUID", rfidUID);
            command.Parameters.AddWithValue("@studentID", studentID);
            command.Parameters.AddWithValue("@studentName", studentName);
            command.Parameters.AddWithValue("@course", course);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@contactNumber", contactNumber);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result == 0)
            {
                StudentRegisterModal.Instance.Close();
                DataFetcher.FetchStudentsData();
                MessageBox.Show($"{studentName} has been successfully registered!");
                connection.Close();

                var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

                if (adminWindow != null)
                {
                    adminWindow.Blur.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                MessageBox.Show("Registration is failed!");

            }
        }
    }
}
