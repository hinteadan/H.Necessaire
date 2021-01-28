using System;

namespace H.Necessaire.Notification
{
    public class HttpNotifierConfiguration
    {
        public Note[] RequestHeaders { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
