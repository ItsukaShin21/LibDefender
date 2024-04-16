using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace LibDefender
{
    public partial class StudentRegisterModal : Window
    {
        private static StudentRegisterModal? newInstance;

        [GeneratedRegex("[0-9]+")]
        private static partial Regex rfidRegexInput();
        [GeneratedRegex("[0-9]+")]
        private static partial Regex idNumberRegexInput();

        public static StudentRegisterModal CurrentInstance { get; private set; } = null!;

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
            CurrentInstance = this;
            DataFetcher.FetchCourseNames();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string rfidUID = rfidTxtBox.Text;
            string studentID = idTxtBox.Text;
            string studentName = studentNameTxtBox.Text;
            string courseName = courseComboBox.Text;
            string email = emailTxtBox.Text;
            string contactNumber = contactNumberTxtBox.Text;

            int course = DataFetcher.FetchCourseID(courseName);

            StudentManager.InsertStudent(rfidUID, studentID, studentName, course, email, contactNumber);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DataFetcher.FetchStudentsData();
            this.Close();
            var adminWindow = Application.Current.Windows.OfType<AdminWindow>().FirstOrDefault();

            if (adminWindow != null)
            {
                adminWindow.Blur.Visibility = Visibility.Hidden;
            }
        }

        private void RfidTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex rfidInput = rfidRegexInput();

            if (!rfidInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void IdTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex idNumberInput = idNumberRegexInput();

            if (!idNumberInput.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }

        private void Rfid_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (rfidTxtBox.Text.Length == 10)
            {
                idTxtBox.Focus();
                rfidTxtBox.IsEnabled = false;
                border.Background = Brushes.Gray;
            }
        }

        private void IdTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (idTxtBox.Text.Length == 7)
            {
                studentNameTxtBox.Focus();
            }
        }

        private void StudentRegisterModal_Loaded(object sender, RoutedEventArgs e)
        {
            rfidTxtBox.Focus();
        }
    }
}