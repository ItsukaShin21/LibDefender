using System.Windows;


namespace LibDefender
{
    public partial class BorrowBookModal : Window
    {
        private static BorrowBookModal? newInstance;

        public static BorrowBookModal Instance
        {
            get
            {
                if (newInstance == null)
                {
                    newInstance = new BorrowBookModal();
                    newInstance.Closed += (s, e) => newInstance = null;
                }
                return newInstance;
            }
        }

        public BorrowBookModal()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow as AdminWindow;

        }
    }
}
