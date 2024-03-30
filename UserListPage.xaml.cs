using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace LibDefender
{
    public partial class UserListPage : Page
    {
        public UserListPage()
        {
            InitializeComponent();
            PopulateDataGrid();
            this.DataContext = this;
        }
        private void PopulateDataGrid()
        {
            var dummyData = new ObservableCollection<DummyItem4>(new[]
            {
                new DummyItem4 { UserRfid = 121, Username = "John Doe", Email = "example@gmail.com", Password = "asdsad", UserType = "Admin" },
                new DummyItem4 { UserRfid = 131, Username = "Alice Smith", Email = "example@gmail.com", Password = "ewrv", UserType = "Library Staff" },
                new DummyItem4 { UserRfid = 141, Username = "Bob Johnson", Email = "example@gmail.com", Password = "rtyjfb", UserType = "Library Staff" }
            });

            // Set the ItemsSource of the DataGrid to the collection
            DataGrid.ItemsSource = dummyData;
        }
    }
    public class DummyItem4
    {
        public int? UserRfid { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserType { get; set; }
    }
}
