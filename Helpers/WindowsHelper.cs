using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace STTNote.Helpers
{
    public class WindowsHelper
    {
        public static void HideWindow(Guid windowId)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var windowList =
                from Window window in System.Windows.Application.Current.Windows
                where windowId.Equals(window.GetValueByName<Guid>("WindowId"))
                select window;

                foreach (var window in windowList)
                {
                    window.Dispatcher.BeginInvoke(window.Hide);
                }
            });
        }

        public static void CloseWindow(Guid windowId)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var windowList =
                from Window window in System.Windows.Application.Current.Windows
                where windowId.Equals(window.GetValueByName<Guid>("WindowId"))
                select window;

                foreach (var window in windowList)
                {
                    window.Dispatcher.BeginInvoke(window.Close);
                }
            });
        }

        public static IEnumerable<T> FindVisualChildrens<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildrens<T>(ithChild)) yield return childOfChild;
            }
        }

        public static T FindVisualChildrenByTag<T>(DependencyObject depObj, string tag) where T : DependencyObject
        {
            var components = FindVisualChildrens<T>(depObj);
            return components.FirstOrDefault(c => c.GetValueByName<object>("Tag")?.ToString()?.Equals(tag) == true);
        }
    }
}