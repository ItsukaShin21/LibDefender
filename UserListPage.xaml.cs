using System.Data;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;


namespace LibDefender
{
    public partial class UserListPage : Page
    {
        readonly string connectionString = DatabaseConfig.systemDatabase;

        public UserListPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void UsersListQuery(string userListQuery)
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(userListQuery, connection);

            connection.Open();

            DataTable newData = new();

            using MySqlDataAdapter dataAdapter = new(command);
            dataAdapter.Fill(newData);

            UsersList.ItemsSource = newData.DefaultView;
        }

        private void UserDeleteQuery(string userRfid)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand("DELETE FROM users WHERE userRfid = @userRfid", connection);
                command.Parameters.AddWithValue("@userRfid", userRfid);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                MessageBox.Show($"Error deleting user: {ex.Message}");
            }
        }

        public void RefreshUsersList()
        {
            string userListQuery = "SELECT * FROM users";
            UsersListQuery(userListQuery);
        }

        private void UserListPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshUsersList();
        }

        private void DeleteButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView dataContext)
            {
                var userRfid = dataContext["userRfid"].ToString();
                var userName = dataContext["username"].ToString();

                // Ask for user confirmation
                var result = MessageBox.Show($"Are you sure you want to delete {userName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    UserDeleteQuery(userRfid!);
                    RefreshUsersList();
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Visible;
            }
            UserRegisterModal.Instance.ShowDialog();
        }
    }
}
