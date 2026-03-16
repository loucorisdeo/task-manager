
using System.Windows.Forms;

namespace TaskManager.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Required for WinForms apps
            ApplicationConfiguration.Initialize();

            // Start the app with the Login screen
            System.Windows.Forms.Application.Run(new LoginForm());
        }
    }
}