using System.Numerics;
using System.Windows;
using System.Windows.Media;


namespace LibDefender
{
    public partial class BookRegisterModal : Window
    {
        private static BookRegisterModal? newInstance;

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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            BigInteger rfidUID = BigInteger.Parse(rfidTxtBox.Text);
            string bookName = bookNameTxtBox.Text;
            string authorName = authorTxtBox.Text;

            int result = BookManager.InsertBook(rfidUID, bookName, authorName);

            if (result == 0)
            {
                DataFetcher.FetchBooks();
                this.Close();
                MessageBox.Show($"{bookName} has been successfully registered!");

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

        private void RfidTxtBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (rfidTxtBox.Text.Length == 10)
            {
                bookNameTxtBox.Focus();
                rfidTxtBox.IsEnabled = false;
                border.Background = Brushes.Gray;
            }
        }

        private void BookRegisterModal_Loaded(object sender, RoutedEventArgs e)
        {
            rfidTxtBox.Focus();
        }
    }
}
