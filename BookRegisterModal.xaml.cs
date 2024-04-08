using System.Numerics;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;


namespace LibDefender
{
    public partial class BookRegisterModal : Window
    {
        private static BookRegisterModal? newInstance;
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public static BookRegisterModal Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new BookRegisterModal();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }

        public BookRegisterModal()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow as AdminWindow;
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

        private void RegisterBookQuery(string query, BigInteger rfidUID, string bookName, string authorName)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@rfidUID", rfidUID);
            command.Parameters.AddWithValue("@bookName", bookName);
            command.Parameters.AddWithValue("@authorName", authorName);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result == 0)
            {
                this.Close();
                MessageBox.Show($"{bookName} has been successfully registered!");
                connection.Close();

                var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

                if (adminWindow != null)
                {
                    adminWindow.Blur.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("Registration failed!");

            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BigInteger rfidUID = BigInteger.Parse(rfidTxtBox.Text);
            string bookName = bookNameTxtBox.Text;
            string authorName = authorTxtBox.Text;

            string query = "INSERT INTO books (bookRfid, bookName, bookAuthor, dateAdded, status)" +
                "VALUES (@rfidUID, @bookName, @authorName, NOW(), 'Available')";

            RegisterBookQuery(query, rfidUID, bookName, authorName);
        }
    }
}
