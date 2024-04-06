using MySql.Data.MySqlClient;
using System.Numerics;
using System.Windows;

namespace LibDefender
{
    public partial class StudentRegisterModal : Window
    {
        private static StudentRegisterModal? newInstance;
        readonly string connectionString = DatabaseConfig.systemDatabase;

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
        }

        private void RegisterStudentQuery(string query, BigInteger rfidUID, string studentID, string studentName, string course, string email, string contactNumber)
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
                MessageBox.Show("Student has been successfully registered!");
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BigInteger rfidUID = BigInteger.Parse(rfidTxtBox.Text);
            string studentID = idTxtBox.Text;
            string studentName = studentNameTxtBox.Text;
            string course = courseComboBox.Text;
            string email = emailTxtBox.Text;
            string contactNumber = contactNumberTxtBox.Text;

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
    }
}