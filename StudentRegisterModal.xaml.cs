using MySql.Data.MySqlClient;
using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows;

namespace LibDefender
{
    public partial class StudentRegisterModal : Window
    {
        private static StudentRegisterModal? newInstance;
        readonly string connectionString = DatabaseConfig.systemDatabase;

        [GeneratedRegex("[0-9]+")]
        private static partial Regex rfidRegexInput();
        [GeneratedRegex("[0-9]+")]
        private static partial Regex idNumberRegexInput();


        public static StudentRegisterModal Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new StudentRegisterModal();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }
        public StudentRegisterModal()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow as AdminWindow;
            Courses();
        }

        private void RegisterStudentQuery(string query, BigInteger rfidUID, string studentID, string studentName, int course, string email, string contactNumber)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

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
                this.Close();
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

        private int GetCourseIDQuery(string courseName)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT courseID FROM courses WHERE courseName = @courseName";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@courseName", courseName);

            int courseID = Convert.ToInt32(command.ExecuteScalar());
            return courseID;
        }

        private void FetchCoursesQuery(string fetchCourseQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(fetchCourseQuery, connection);

            connection.Open();

            DataTable courses = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(courses);

            connection.Close();

            courseComboBox.ItemsSource = courses.DefaultView;

            courseComboBox.DisplayMemberPath = "courseName";

        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BigInteger rfidUID = BigInteger.Parse(rfidTxtBox.Text);
            string studentID = idTxtBox.Text;
            string studentName = studentNameTxtBox.Text;
            string courseName = courseComboBox.Text;
            string email = emailTxtBox.Text;
            string contactNumber = contactNumberTxtBox.Text;

            int course = GetCourseIDQuery(courseName);

            string query = "INSERT INTO students (studentRfid, studentID, studentName, course, email, contactNumber)" +
                "VALUES (@rfidUID, @studentID, @studentName, @course, @email, @contactNumber)";

            RegisterStudentQuery(query, rfidUID, studentID, studentName, course, email, contactNumber);
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

        private void Courses()
        {
            string fetchCourseQuery = "SELECT courseName FROM courses";
            FetchCoursesQuery(fetchCourseQuery);
        }

        private void RfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rfidInput = rfidRegexInput();

            if (!rfidInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void IdTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex idNumberInput = idNumberRegexInput();

            if (!idNumberInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Rfid_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (rfidTxtBox.Text.Length == 10)
            {
                idTxtBox.Focus();
            }
        }

        private void IdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (idTxtBox.Text.Length == 7)
            {
                studentNameTxtBox.Focus();
            }
        }
    }
}
