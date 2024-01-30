using NonInvasiveKeyboardHookLibrary;
using System;
using System.Windows.Forms;

namespace STTNote.Helpers
{
    public static class KeyboardHelper
    {
        private static KeyboardHookManager _keyboardHookManager;

        public static void Init()
        {
            _keyboardHookManager = new KeyboardHookManager();
            _keyboardHookManager.Start();
        }

        public static void RegisterKey(Action action, Keys key, params ModifierKeys[] modifierKeys)
        {
            _keyboardHookManager.RegisterHotkey(modifierKeys, (int)key, action);
        }
    }
}