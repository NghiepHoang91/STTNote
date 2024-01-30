using STTNote.Wrappers;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace STTNote.Helpers
{
    public static class ApplicationHelper
    {
        public static void ForceExitApp()
        {
            ApplicationWrapper.CurrentApplication.Dispatcher
                .BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
        }

        public static void RunAsMainThread(System.Action action)
        {
            ApplicationWrapper.CurrentApplication.Dispatcher.BeginInvoke(action);
        }

        [STAThread]
        public static bool AskForAppPassword()
        {
            var config = AppConfigWrapper.Instance.GetById(Const.Consts.ConfigIds.APP_PASSWORD);
            if (!string.IsNullOrEmpty(config?.Value?.ToString()))
            {
                var passwordInput = new TextInput(Enums.FormInputMode.Password);
                var isOkClicked = passwordInput.ShowDialog();
                if (isOkClicked == false)
                {
                    return false;
                }

                var ecryptedPassword = CryptoHelper.Encrypt(passwordInput.InputValue);
                var ecryptedConfigPassword = CryptoHelper.Encrypt(config?.Value?.ToString() ?? string.Empty);

                if (!ecryptedPassword?.Equals(ecryptedConfigPassword) == true)
                {
                    MessageBox.Show("Password is incorrect!");
                    return false;
                }
            }
            return true;
        }

        public static bool IsPasswordProtected()
        {
            var config = AppConfigWrapper.Instance.GetById(Const.Consts.ConfigIds.APP_PASSWORD);
            if (!string.IsNullOrEmpty(config?.Value?.ToString()))
            {
                return true;
            }
            return false;
        }

        public static void CreateShortcutInStartupFolder()
        {
            var config = AppConfigWrapper.Instance.GetById(Const.Consts.ConfigIds.RUN_ON_STARTUP);

            var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (string.IsNullOrEmpty(startupFolder)) return;

            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var shortcutName = $"{appName} v{appVersion}.lnk";
            var shortcutPath = $"{startupFolder}\\{shortcutName}";
            var silientParam = "silent";

            if (config?.Value == null || !(bool)config.Value)
            {
                ShortcutHelper.DeleteShortcut(shortcutPath);
            }
            else
            {
                ShortcutHelper.CreateShortcut(Environment.ProcessPath, shortcutPath, silientParam);
            }
        }

        public static void CreateShortcutInDesktop()
        {
            var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (string.IsNullOrEmpty(startupFolder)) return;

            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var shortcutName = $"{appName} v{appVersion}.lnk";
            var shortcutPath = $"{startupFolder}\\{shortcutName}";

            ShortcutHelper.CreateShortcut(Environment.ProcessPath, shortcutPath);
        }
    }
}