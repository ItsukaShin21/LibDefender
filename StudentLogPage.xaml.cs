using System.Data;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class StudentLogPage : Page
    {
        public static StudentLogPage CurrentInstance { get; private set; } = null!;

        public StudentLogPage()
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentInstance = this;
        }

        private void RfidTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string rfid = RfidTxtBox.Text;
            TableQueries.CheckRfid(rfid);
        }

        private void StudentLogPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataFetcher.FetchLogRecordsData();
        }

        private void PreviewTextInput_RfidTxtBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            FunctionComponents.ValidateInput(e, FunctionComponents.RfidRegex);
        }
    }
}
