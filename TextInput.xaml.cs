using STTNote.Enums;
using STTNote.Helpers;
using STTNote.Views;
using System.Windows;
using System.Windows.Input;

namespace STTNote
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : BaseWindow
    {
        private FormInputMode _mode;
        private string titleContent;
        public string InputValue { get; set; }

        public TextInput(FormInputMode mode, string titleContent = null)
        {
            _mode = mode;
            this.titleContent = titleContent;

            InitializeComponent();
            SetInputMode();
        }

        private void SetInputMode()
        {
            switch (_mode)
            {
                case FormInputMode.Password:
                    {
                        lblTitle.Content = titleContent ?? "Enter password:";
                        txtPlainText.Visibility = Visibility.Hidden;
                        txtPassword.Visibility = Visibility.Visible;
                        txtPassword.Focus();
                    }
                    break;

                case FormInputMode.PLainText:
                    {
                        lblTitle.Content = titleContent ?? "Enter Text:";
                        txtPlainText.Visibility = Visibility.Visible;
                        txtPassword.Visibility = Visibility.Hidden;
                        txtPlainText.Focus();
                    }
                    break;
            }
        }

        private void btnOk_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DialogResult = true;
            switch (_mode)
            {
                case FormInputMode.Password:
                    {
                        InputValue = txtPassword.Password;
                    }
                    break;

                case FormInputMode.PLainText:
                    {
                        InputValue = txtPlainText.Text;
                    }
                    break;
            }
            WindowsHelper.CloseWindow(WindowId);
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
                WindowsHelper.CloseWindow(WindowId);
            }

            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}