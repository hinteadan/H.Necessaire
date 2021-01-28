using System;

namespace H.Necessaire.Notification
{
    public class SmtpNotifierConfiguration
    {
        public string ServerUrl { get; set; }
        public int Port { get; set; } = 587;//TLS:587 SSL:465 Unencrypted:25
        public string Username { get; set; }
        public string Password { get; set; }
        public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public bool IsSslDisabled { get; set; } = false;
    }
}
