using STTNote.Enums;
using System.Collections.Generic;
using System.Linq;

namespace STTNote.ActionResults
{
    public class ActionResult
    {
        public ActionResult()
        {
            Code = ActionResultCode.Success;
            Messages = new List<ActionResultMessage>();
        }

        public static ActionResult Failed(string message, string? title = null)
        {
            return new ActionResult()
            {
                Code = ActionResultCode.Failed,
                Messages = new List<ActionResultMessage>
                {
                    new ActionResultMessage
                    {
                        Message = message,
                        Title = title ?? string.Empty
                    }
                }
            };
        }

        public static ActionResult Success
        {
            get
            {
                return new ActionResult()
                {
                    Code = ActionResultCode.Success,
                    Messages = new List<ActionResultMessage>()
                };
            }
        }

        public ActionResultCode Code { set; get; }
        public List<ActionResultMessage>? Messages { set; get; }

        public bool IsAnyMessage
        {
            get
            {
                return Messages?.Any(m => !string.IsNullOrEmpty(m.Message)) ?? false;
            }
        }

        public bool IsSuccess
        {
            get
            {
                return Code == ActionResultCode.Success;
            }
        }
    }

    public class ActionResult<T> : ActionResult
    {
        public T? ReturnValue { set; get; }

        public ActionResult()
        {
            Code = ActionResultCode.Success;
            Messages = new List<ActionResultMessage>();
            ReturnValue = default(T);
        }

        public new static ActionResult<T> Failed(string message, string? title = null)
        {
            return new ActionResult<T>()
            {
                Code = ActionResultCode.Failed,
                Messages = new List<ActionResultMessage>
                {
                    new ActionResultMessage
                    {
                        Message = message,
                        Title = title ?? string.Empty
                    }
                }
            };
        }

        public new static ActionResult<T> Success
        {
            get
            {
                return new ActionResult<T>()
                {
                    Code = ActionResultCode.Success,
                    Messages = new List<ActionResultMessage>()
                };
            }
        }
    }
}