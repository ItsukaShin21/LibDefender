using System.Windows;

namespace LibDefender
{
    public partial class StudentRegisterModal : Window
    {
        private static StudentRegisterModal? newInstance;
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
        }
    }
}
