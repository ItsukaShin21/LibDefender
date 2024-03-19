using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class BorrowBookPage : Page
    {
        public BorrowBookPage()
        {
            InitializeComponent();
            PopulateDataGrid();
            this.DataContext = this;
        }

        private void PopulateDataGrid()
        {
            var dummyData = new ObservableCollection<DummyItem3>(new[]
            {
                new DummyItem3 { StudentID = 1, BookID = 23423, DateBurrowed = "Feb 19, 2024 12:34:26", DueDate = "Feb 20, 2024 12:34:26" },
                new DummyItem3 { StudentID = 2, BookID = 5643, DateBurrowed = "Feb 19, 2024 12:34:26", DueDate = "Feb 20, 2024 12:34:26" },
                new DummyItem3 { StudentID = 3, BookID = 6777, DateBurrowed = "Feb 19, 2024 12:34:26", DueDate = "Feb 20, 2024 12:34:26" }
            });

            // Set the ItemsSource of the DataGrid to the collection
            DataGrid.ItemsSource = dummyData;
        }
    }

    public class DummyItem3
    {
        public int StudentID { get; set; }
        public int BookID { get; set; }
        public string? DateBurrowed { get; set; }
        public string? DueDate { get; set; }
    }
}
