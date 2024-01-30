using STTNote.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace STTNote.Helpers
{
    public static class ReferrencesHelper
    {
        private static Dictionary<string, object> _actionSpot = new Dictionary<string, object>();

        public static void RegisterAction<TMessage>(Action<TMessage> action)
        {
            if (action != null)
            {
                _actionSpot[typeof(TMessage).Name] = action;
            }
        }

        public static void RegisterActionAsync<TMessage>(Func<TMessage, Task> action)
        {
            if (action != null)
            {
                _actionSpot[typeof(TMessage).Name] = action;
            }
        }

        public static void SendMessage<TMessage>(TMessage message)
        {
            var actionObj = _actionSpot.GetValue(message.GetType().Name);
            if (actionObj == null) return;

            Application.Current.Dispatcher.Invoke
                (() =>
                {
                    switch (actionObj)
                    {
                        case Action<TMessage> action:
                            {
                                action.Invoke(message);
                            }
                            break;

                        case Func<TMessage, Task> func:
                            {
                                Task.Run(async () =>
                                {
                                    await func.Invoke(message);
                                });
                            }
                            break;
                    }
                });
        }
    }
}