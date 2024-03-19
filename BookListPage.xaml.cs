using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class BookListPage : Page
    {
        public BookListPage()
        {
            InitializeComponent();
            PopulateDataGrid();
            this.DataContext = this;
        }

        private void PopulateDataGrid()
        {
            var dummyData = new ObservableCollection<DummyItem2>(new[]
            {
                new DummyItem2 { BookID = 1, BookName = "English", DateAdded = "Feb 29, 2024 01:23:45", Status = "Burrowed" },
                new DummyItem2 { BookID = 2, BookName = "Science", DateAdded = "Feb 29, 2024 01:23:45", Status = "Available" },
                new DummyItem2 { BookID = 3, BookName = "Filipino", DateAdded = "Feb 29, 2024 01:23:45", Status = "Burrowed" }
            });

            // Set the ItemsSource of the DataGrid to the collection
            DataGrid.ItemsSource = dummyData;
        }
    }

    public class DummyItem2
    {
        public int BookID { get; set; }
        public string? BookName { get; set; }
        public string? DateAdded { get; set; }
        public string? Status { get; set; }
    }
}
