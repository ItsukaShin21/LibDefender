using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace LibDefender
{
    public partial class LoginWindow : Window
    {
        private readonly DispatcherTimer errorMessageTimer = new();
        private bool isRfidErrorShown = true;
        private bool isPasswordErrorShown = true;
        private bool isFieldErrorShown = true;
        private const int uidLength = 10;

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

        //Function for Handling the timer of error messages
        private void ErrorMessageTimer_Tick(object? sender, EventArgs e)
        {
            HideErrorMessage(Rfidtxtbox_ErrorMessage, ref isRfidErrorShown);
            HideErrorMessage(Passwordtxtbox_ErrrorMessage, ref isPasswordErrorShown);
            HideErrorMessage(ErrorMessage, ref isFieldErrorShown);

            if (!isRfidErrorShown && !isPasswordErrorShown && !isFieldErrorShown)
            {
                errorMessageTimer.Stop();
                ErrorMessage.Visibility = Visibility.Hidden;
            }
        }

        //Function to handle the visibility fo error messages
        private static void HideErrorMessage(UIElement errorMessage, ref bool isErrorShown)
        {
            if (isErrorShown)
            {
                errorMessage.Visibility = Visibility.Hidden;
                isErrorShown = false;
            }
        }

        //Function to validate the textboxes if empty or not
        private bool ValidateInputs()
        {
            bool isValid = true;

            if (uidPlaceholder.Visibility == Visibility.Visible || string.IsNullOrWhiteSpace(rfiduidTxtBox.Text))
            {
                Rfidtxtbox_ErrorMessage.Visibility = Visibility.Visible;
                isRfidErrorShown = true;
                isValid = false;
            }

            if (passwordPlaceholder.Visibility == Visibility.Visible || string.IsNullOrWhiteSpace(passwordTxtBox.Password))
            {
                Passwordtxtbox_ErrrorMessage.Visibility = Visibility.Visible;
                isPasswordErrorShown = true;
                isValid = false;
            }

            if (!isValid && !errorMessageTimer.IsEnabled)
            {
                errorMessageTimer.Interval = TimeSpan.FromSeconds(2);
                errorMessageTimer.Start();
            }

            return isValid;
        }

        //Function for logging in the user
        private async Task LoginQuery(string rfiduid, string password)
        {
            bool result = await Authentication.AuthenticateUser(rfiduid, password);

            if (result)
            {
                MessageBox.Show("Login Successfully");
                AdminWindow.Instance.Show();
                this.Hide();
            }
            else
            {
                ErrorMessage.Visibility = Visibility.Visible;
                errorMessageTimer.Interval = TimeSpan.FromSeconds(2);
                errorMessageTimer.Start();
                isFieldErrorShown = true;
                rfiduidTxtBox.Clear();
                passwordTxtBox.Clear();
                rfiduidTxtBox.Focus();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rfiduidTxtBox.Focus();
        }

        private void RfiduidTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rfiduidTxtBox.Text))
            {
                uidPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void RfiduidTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rfiduidTxtBox.Text))
            {
                uidPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void PasswordTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordTxtBox.Password))
            {
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordTxtBox.Password))
            {
                passwordPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                string rfiduid = rfiduidTxtBox.Text;
                string password = passwordTxtBox.Password;
                await LoginQuery(rfiduid, password);
                passwordPlaceholder.Visibility = Visibility.Visible;
            }
        }
        private void RfiduidTxtBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (rfiduidTxtBox.Text.Length == uidLength)
            {
                passwordTxtBox.Focus();
                rfiduidTxtBox.IsEnabled = false;
                border.Background = Brushes.Gray;
            }
        }
        private void Rfidtxtbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            FunctionComponents.ValidateInput(e, FunctionComponents.RfidRegex);
        }
        private void Passwordtxtbox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            FunctionComponents.ValidateInput(e, FunctionComponents.PasswordRegex);
        }

        private void UidPlaceholder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            rfiduidTxtBox.Focus();
        }

        private void PasswordPlaceholder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            passwordTxtBox.Focus();
        }
    }
}