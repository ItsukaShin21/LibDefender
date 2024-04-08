using System.Numerics;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace LibDefender
{
    public partial class UserRegisterModal : Window
    {
        private static UserRegisterModal? newInstance;
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public static UserRegisterModal Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new UserRegisterModal();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }

        public UserRegisterModal()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow as AdminWindow;
        }

        private void RegisterUserQuery(string query, BigInteger userRfid, string username, string email, string password, string userType)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@userRfid", userRfid);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@userType", userType);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result == 0)
            {
                this.Close();
                MessageBox.Show($"{username} has been successfully registered!");
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BigInteger userRfid = BigInteger.Parse(rfidTxtBox.Text);
            string username = usernameTxtBox.Text;
            string email = emailTxtBox.Text;
            string password = passwordTxtBox.Text;
            string userType = userTypeTxtBox.Text;

            string query = "INSERT INTO users (userRfid, username, email, password, userType)" +
                "VALUES (@userRfid, @username, @email, @password, @userType)";

            RegisterUserQuery(query, userRfid, username, email, password, userType);
        }
    }
}
