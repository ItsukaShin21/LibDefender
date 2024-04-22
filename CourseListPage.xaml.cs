using System.Windows.Controls;

namespace LibDefender
{
    public partial class CourseListPage : Page
    {
        public static CourseListPage CurrentInstance { get; private set; } = null!;

        public CourseListPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
