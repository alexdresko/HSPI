using System;

namespace Hspi.HspiPlugin3.Events
{
    public class LogEventArgs
    {
        public DateTime Date { get; set; }

        public string ErrorCode { get; set; }

        public string From { get; set; }

        public string Priority { get; set; }

        public string Color { get; set; }

        public string Message { get; set; }

        public string MessageClass { get; set; }

        public DateTime DateTime { get; set; }
    }
}