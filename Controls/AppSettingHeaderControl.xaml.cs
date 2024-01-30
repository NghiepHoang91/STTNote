using STTNote.Helpers;
using STTNote.Views;
using System;

namespace STTNote.Controls
{
    /// <summary>
    /// Interaction logic for AppSettingHeaderControl.xaml
    /// </summary>
    public partial class AppSettingHeaderControl : BaseUserControl
    {
        public AppSettingHeaderControl()
        {
            InitializeComponent();
        }

        private void labelClose_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WindowsHelper.CloseWindow(WindowId);
        }
    }
}