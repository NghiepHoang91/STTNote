using System.Windows;

namespace STTNote.Wrappers
{
    public static class ApplicationWrapper
    {
        public static Application CurrentApplication
        {
            get
            {
                return Application.Current ?? new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            }
        }
    }
}