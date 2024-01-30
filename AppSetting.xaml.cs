using Microsoft.Win32;
using STTNote.ActionMessages;
using STTNote.Helpers;
using STTNote.Models;
using STTNote.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STTNote
{
    /// <summary>
    /// Interaction logic for AppSetting.xaml
    /// </summary>
    public partial class AppSetting : BaseWindow
    {
        public ObservableCollection<AppConfig> AppConfigs { get; set; }

        public AppSetting()
        {
            AppConfigs = new ObservableCollection<AppConfig>();
            InitializeComponent();
            HeaderPanel.WindowId = WindowId;
        }

        public void SetAppConfig(List<AppConfig> lstConfig)
        {
            lstConfig?.ForEach(x => AppConfigs.Add(x));
            lstConigs.ItemsSource = AppConfigs;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PutPasswordToUI();
        }

        private void ConfigButton_SelectFile(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            var configId = button.Tag.ToString();
            if (string.IsNullOrEmpty(configId)) return;
            var textbox = WindowsHelper.FindVisualChildrenByTag<TextBox>(this, configId);
            if (textbox == null) return;

            var openfiledialog = new OpenFileDialog();
            var isFileOpen = openfiledialog.ShowDialog();

            if (isFileOpen == true)
            {
                textbox.Text = openfiledialog.FileName;
            }
        }

        private void ConfigButton_SelectFolder(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            var configId = button.Tag.ToString();
            if (string.IsNullOrEmpty(configId)) return;
            var textbox = WindowsHelper.FindVisualChildrenByTag<TextBox>(this, configId);
            if (textbox == null) return;

            var openfiledialog = new System.Windows.Forms.FolderBrowserDialog();
            var dialogResult = openfiledialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                textbox.Text = openfiledialog.SelectedPath;
            }
        }

        private void PutPasswordToUI()
        {
            var passwordBoxs = WindowsHelper.FindVisualChildrens<PasswordBox>(this).ToList();
            foreach (var passBox in passwordBoxs)
            {
                var appConfig = AppConfigs.FirstOrDefault(n => n.Id.Equals(passBox.Tag.ToString()));
                if (appConfig != null)
                {
                    passBox.Password = appConfig.Value.ToString();
                }
            }
        }

        private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReferrencesHelper.SendMessage(new SaveAppSettingMessage
            {
                Configs = AppConfigs.ToList()
            });
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            if (passwordBox != null)
            {
                var configId = passwordBox.Tag as string;
                if (configId == null) return;

                var config = AppConfigs.FirstOrDefault(n => n.Id.Equals(configId));
                if (config == null) return;

                config.Value = passwordBox.Password;
            }
        }
    }
}