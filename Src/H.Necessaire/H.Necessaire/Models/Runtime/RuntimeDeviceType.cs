namespace H.Necessaire
{
    public enum RuntimeDeviceType
    {
        Bot = -1,

        Unknown = 0,

        MobilePhone = 100,
        MobilePhoneAndroid = 101,
        MobilePhoneApple = 102,

        Tablet = 500,
        TabletAndroid = 501,
        TabletApple = 502,

        Desktop = 1000,
        DesktopWindows = 1001,
        DesktopLinux = 1002,
        DesktopMac = 1003,
        DesktopIos = 1004,
    }
}
