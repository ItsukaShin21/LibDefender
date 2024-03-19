using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibDefender
{
    public partial class StudentLogPage : Page
    {
        public StudentLogPage()
        {
            InitializeComponent();
            PopulateDataGrid();
            this.DataContext = this;
        }

        private void PopulateDataGrid()
        {
            var dummyData = new ObservableCollection<DummyItem1>(new[]
            {
                new DummyItem1 { StudentID = 1, StudentName = "John Doe", Course = "Computer Science", TimeIn = "Feb 20, 2024 12:34:26", TimeOut = "Feb 21, 2024 12:34:26" },
                new DummyItem1 { StudentID = 2, StudentName = "John Doe", Course = "Computer Science", TimeIn = "Feb 20, 2024 12:34:26", TimeOut = "Feb 21, 2024 12:34:26" },
                new DummyItem1 { StudentID = 3, StudentName = "John Doe", Course = "Computer Science", TimeIn = "Feb 20, 2024 12:34:26", TimeOut = "Feb 21, 2024 12:34:26" }
            });

            // Set the ItemsSource of the DataGrid to the collection
            DataGrid.ItemsSource = dummyData;
        }
    }
    public class DummyItem1
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public string? Course { get; set; }
        public string? TimeIn { get; set; }
        public string? TimeOut { get; set; }
    }
}
