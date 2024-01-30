using STTNote.Helpers;
using STTNote.Wrappers;
using System.Linq;
using System.Windows;

namespace STTNote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitDefaultData();
            KeyboardHelper.Init();

            MainWindow window = new MainWindow();
            window.Show();

            if (HasSilentParameter(e) || ApplicationHelper.IsPasswordProtected())
            {
                window.Hide();
            }
        }

        private bool HasSilentParameter(StartupEventArgs e)
        {
            return e.Args.Length > 0 && e.Args.Any(p => p.Trim().ToLower().Equals("silent"));
        }

        private void InitDefaultData()
        {
            DatabaseWrapper.Instance.InitDefaultTables();
            AppConfigWrapper.Instance.InitDefaultAppConfig();
            ProfileWrapper.Instance.InitDefaultProfile();
        }
    }
}