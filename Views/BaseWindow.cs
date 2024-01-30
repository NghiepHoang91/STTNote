using System;
using System.Windows;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace STTNote.Views
{
    public class BaseWindow : Window
    {
        public Guid WindowId { get; set; }

        public BaseWindow()
        {
            WindowId = Guid.NewGuid();
        }

        public bool IsWindowLoaded()
        {
            bool isLoaded = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var windowList =
                from Window window in Application.Current.Windows
                where window is BaseWindow baseWindow && baseWindow.WindowId.ToString().Equals(WindowId.ToString())
                select window;

                isLoaded = windowList?.FirstOrDefault()?.IsLoaded ?? false;
            });

            return isLoaded;
        }
    }
}