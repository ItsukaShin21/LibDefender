using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class StudentListPage : Page
    {
        public StudentListPage()
        {
            InitializeComponent();
            PopulateDataGrid();
            this.DataContext = this;
        }

        private void PopulateDataGrid()
        {
            var dummyData = new ObservableCollection<DummyItem>(new[]
            {
                new DummyItem { StudentRfid = 121, StudentID = 1, StudentName = "John Doe", Course = "Computer Science", Email = "example@gmail.com", ContactNumber = "09123456789" },
                new DummyItem { StudentRfid = 131, StudentID = 2, StudentName = "Alice Smith", Course = "Mathematics", Email = "example@gmail.com", ContactNumber = "09123456789" },
                new DummyItem { StudentRfid = 141, StudentID = 3, StudentName = "Bob Johnson", Course = "Physics", Email = "example@gmail.com", ContactNumber = "09123456789" }
            });

            // Set the ItemsSource of the DataGrid to the collection
            DataGrid.ItemsSource = dummyData;
        }
    }

    public class DummyItem
    {
        public int? StudentRfid { get; set; }
        public int? StudentID { get; set; }
        public string? StudentName { get; set; }
        public string? Course { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
    }
}
