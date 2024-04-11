using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class StudentLogPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public StudentLogPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void RfidTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rfid = RfidTxtBox.Text;
            CheckRfid(rfid);
        }

        private void CheckRfid(string rfid)
        {
            string CheckQuery = @"SELECT 'student' AS type, studentRfid AS rfid FROM students WHERE studentRfid = @rfid
                                UNION ALL SELECT 'book' AS type, bookRfid AS rfid FROM books WHERE bookRfid = @rfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(CheckQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string type = reader.GetString("type");
                    
                    if (type == "student")
                    {
                        HandleStudentRfid(rfid);
                    }
                    else if (type == "book")
                    {
                        HandleBookRfid(rfid);
                    }
                }
            }
        }

        private void HandleStudentRfid(string rfid)
        {
            string LogStudentQuery = "SELECT COUNT(*) FROM logrecords WHERE studentRfid = @rfid AND timeout IS NULL";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(LogStudentQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            long count = (Int64)command.ExecuteScalar();

            if (count > 0)
            {
                command.CommandText = "UPDATE logrecords SET timeout = NOW() WHERE studentRfid = @rfid AND timeout IS NULL";
                command.ExecuteNonQuery();
            }
            else
            {
                command.CommandText = "INSERT INTO logrecords (studentRfid, timein) VALUES (@rfid, NOW())";
                command.ExecuteNonQuery();
            }
        }

        private void HandleBookRfid(string rfid)
        {
            string CheckBookQuery = "SELECT status FROM books WHERE bookRfid = @rfid";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(CheckBookQuery, connection);

            command.Parameters.AddWithValue("@rfid", rfid);

            connection.Open();

            string checkStatus = (string)command.ExecuteScalar();

            if (checkStatus == "Available")
            {
                MessageBox.Show("A book has been stolen");
            }
        }
    }
}
