using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibDefender
{
    public partial class StudentListPage : Page
    {
        public static StudentListPage CurrentInstance { get; private set; } = null!;

        public StudentListPage()
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentInstance = this;
        }

        public void StudentListPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataFetcher.FetchStudentsData();
        }

        private void DeleteButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && button.DataContext is DataRowView dataContext)
            {
                var studentID = dataContext["studentID"].ToString();
                var studentName = dataContext["studentName"].ToString();

                // Ask for user confirmation
                var result = MessageBox.Show($"Are you sure you want to delete {studentName}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    StudentManager.DeleteStudent(studentID!);
                    DataFetcher.FetchStudentsData();
                }
            }
            else
            {
                MessageBox.Show("Error: Unable to retrieve data for deletion.");
            }
        }

        private void RegisterModalButton_Click(object sender, RoutedEventArgs e)
        {
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Visible;
            }
            StudentRegisterModal.Instance.ShowDialog();
        }
    }
}