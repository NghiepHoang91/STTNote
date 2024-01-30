using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace STTNote.Views
{
    public class BaseUserControl : UserControl
    {
        public Guid WindowId { get; set; }

        public BaseUserControl()
        {
        }
    }
}