namespace H.Necessaire
{
    public class ConsumerPlatformInfo
    {
        public string PlatformVersion { get; set; }
        public string Platform { get; set; }
        public bool? IsMobile { get; set; }
        public string UserAgent { get; set; }
        public bool? IsWindows { get; set; }
        public string WindowsVersion { get; set; }

        public Note[] Attributes { get; set; }

        public bool IsEmpty()
        {
            return
                PlatformVersion == null
                && Platform == null
                && IsMobile == null
                && IsWindows == null
                && WindowsVersion == null
                && Attributes == null
                ;
        }
    }
}
