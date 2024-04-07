using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
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
        private bool isRfidErrorShown = true;
        private bool isPasswordErrorShown = true;
        private bool isFieldErrorShown = true;

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
            errorMessageTimer.Interval = TimeSpan.FromSeconds(2);
            errorMessageTimer.Tick += ErrorMessageTimer_Tick;
        }
        private static void SetPlaceholder(string placeholder, TextBox textbox)
        {
            textbox.Text = placeholder;
            textbox.Foreground = Brushes.Gray;
            textbox.Tag = placeholder;
        }
        private static void UnsetPlaceholder(string placeholder, TextBox textbox)
        {
            textbox.Text = placeholder;
            textbox.Foreground = Brushes.Black;
            textbox.Tag = placeholder;
        }
        private void ErrorMessageTimer_Tick(object? sender, EventArgs e)
        {
            if (isRfidErrorShown)
            {
                Rfidtxtbox_ErrorMessage.Visibility = Visibility.Hidden;
                isRfidErrorShown = false;
            }
            if (isPasswordErrorShown)
            {
                Passwordtxtbox_ErrrorMessage.Visibility = Visibility.Hidden;
                isPasswordErrorShown = false;
            }
            if (isFieldErrorShown)
            {
                ErrorMessage.Visibility = Visibility.Hidden;
                isFieldErrorShown = false;
            }
            if (isRfidErrorShown && isPasswordErrorShown && isFieldErrorShown)
            {
                errorMessageTimer.Stop();
                ErrorMessage.Visibility = Visibility.Hidden;
            }
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
                AdminWindow.Instance.Show();
                this.Hide();
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
                errorMessageTimer.Interval = TimeSpan.FromSeconds(2); // Changed to 2 seconds
                errorMessageTimer.Start();
                isFieldErrorShown = true;
                rfiduidTxtBox.Text = "";
                passwordTxtBox.Text = "";
                rfiduidTxtBox.Focus();
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string rfiduid = rfiduidTxtBox.Text;
            string password = passwordTxtBox.Text;

            if (rfiduid == "Enter your RFID UID" || rfiduid == "" || password == "Enter your Password" || password == "")
            {
                if (rfiduid == "Enter your RFID UID" || rfiduid == "")
                {
                    Rfidtxtbox_ErrorMessage.Visibility = Visibility.Visible;
                    isRfidErrorShown = true;
                }
                if (password == "Enter your Password" || password == "")
                {
                    Passwordtxtbox_ErrrorMessage.Visibility = Visibility.Visible;
                    isPasswordErrorShown = true;
                }
                // Set the timer interval and start it only if it's not already running
                if (!errorMessageTimer.IsEnabled)
                {
                    errorMessageTimer.Interval = TimeSpan.FromSeconds(2); // Changed to 2 seconds
                    errorMessageTimer.Start();
                }
            }
            else
            {
                string query = "SELECT COUNT(*) FROM users WHERE userRfid = @rfiduid AND password = @password";
                LoginQuery(query, rfiduid, password);
            }
        }
        private void RfiduidTxtBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (rfiduidTxtBox.Text.Length == 10)
            {
                passwordTxtBox.Focus();
            }
        }
        private void Rfidtxtbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rfidInput = rfidRegexInput();

            if (!rfidInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
        private void Passwordtxtbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex passwordInput = passwordRegexInput();

            if (!passwordInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        [GeneratedRegex("[0-9]+")]
        private static partial Regex rfidRegexInput();
        [GeneratedRegex("[A-Za-z0-9!@#]+")]
        private static partial Regex passwordRegexInput();
    }
}