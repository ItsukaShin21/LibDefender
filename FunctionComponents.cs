using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LibDefender
{
    public static class FunctionComponents
    {
        public static readonly Regex RfidRegex = new("[0-9]+");
        public static readonly Regex PasswordRegex = new("[A-Za-z0-9!@#]+");

        public static void ValidateInput(TextCompositionEventArgs e, Regex regex)
        {
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true;
            }
        }
    }
}