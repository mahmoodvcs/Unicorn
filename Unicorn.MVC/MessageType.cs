using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unicorn.Mvc
{
    public enum MessageType
    {
        Success,
        Info,
        Warning,
        Danger,
        Error = Danger,
    }
    public class UIMessage
    {
        public string Message { get; set; }
        //public string Header { get; set; }
        //public string Footer { get; set; }
        public MessageType Type { get; set; }
        public bool Closable { get; set; }
    }
}