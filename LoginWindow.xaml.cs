using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LibDefender
{
    public partial class LoginWindow : Window
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;
        private readonly DispatcherTimer errorMessageTimer = new();

        private static LoginWindow? newInstance;
        public static LoginWindow Instance
        {
            get
            {
                newInstance ??= new LoginWindow();
                return newInstance;
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
        }
        private static void SetPlaceholder(string placeholder, TextBox textBox)
        {
            textBox.Text = placeholder;
            textBox.Foreground = Brushes.Gray;
            textBox.Tag = placeholder;
        }
        private static void UnsetPlaceholder(string placeholder, TextBox textBox)
        {
            textBox.Text = placeholder;
            textBox.Foreground = Brushes.Black;
            textBox.Tag = placeholder;
        }
        private void ErrorMessageTimer_Tick(object? sender, EventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Hidden;
            errorMessageTimer.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetPlaceholder("Enter your RFID UID", rfiduidTxtBox);
            SetPlaceholder("Enter your Password", passwordTxtBox);
            rfiduidTxtBox.Focus();
        }

        private void RfiduidTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rfiduidTxtBox.Text))
            {
                SetPlaceholder("Enter your RFID UID", rfiduidTxtBox);
            }
        }

        private void RfiduidTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (rfiduidTxtBox.Text == "Enter your RFID UID")
            {
                UnsetPlaceholder("", rfiduidTxtBox);
            }
        }

        private void PasswordTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordTxtBox.Text))
            {
                SetPlaceholder("Enter your Password", passwordTxtBox);
            }
        }

        private void PasswordTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTxtBox.Text == "Enter your Password")
            {
                UnsetPlaceholder("", passwordTxtBox);
            }
        }

        private void LoginQuery(string query, string rfiduid, string password)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@rfiduid", rfiduid);
            command.Parameters.AddWithValue("@password", password);

            connection.Open();

            int result = Convert.ToInt32(command.ExecuteScalar());

            if (result > 0)
            {
                MessageBox.Show("Login Successfully");
                this.Hide();
                AdminWindow.Instance.Show();
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
                rfiduidTxtBox.Text = "";
                passwordTxtBox.Text = "";
                rfiduidTxtBox.Focus();
                errorMessageTimer.Start();
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT COUNT(*) FROM users WHERE " +
                           "userRfid = @rfiduid AND password = @password";

            errorMessageTimer.Interval = TimeSpan.FromSeconds(2);
            errorMessageTimer.Tick += ErrorMessageTimer_Tick;

            LoginQuery(query, rfiduidTxtBox.Text, passwordTxtBox.Text);
        }

        private void RfiduidTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(rfiduidTxtBox.Text.Length == 10)
            {
                passwordTxtBox.Focus();
            }
        }
    }
}