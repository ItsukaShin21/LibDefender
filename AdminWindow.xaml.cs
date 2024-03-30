using System.Windows;

namespace LibDefender
{
    public partial class AdminWindow : Window
    {
        private static AdminWindow? newInstance;
        public static AdminWindow Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new AdminWindow();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }
        public AdminWindow()
        {
            InitializeComponent();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(new DashboardPage());
        }

        private void StudentListButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(new StudentListPage());
        }

        private void StudentLogsButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(new StudentLogPage());
        }

        private void BookListButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(new BookListPage());
        }

        private void BurrowListButton_Click(object sender, RoutedEventArgs e)
        {
            PageFrame.Navigate(new BorrowBookPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            LoginWindow.Instance.Show();
        }
    }
}
