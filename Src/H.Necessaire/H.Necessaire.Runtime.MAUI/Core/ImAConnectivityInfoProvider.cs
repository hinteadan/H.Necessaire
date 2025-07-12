namespace H.Necessaire.Runtime.MAUI.Core
{
    public interface ImAConnectivityInfoProvider
    {
        Task<ConnectivityInfo> GetConnectivityInfo();

        Task ForceRefresh();
    }

    public class ConnectivityInfo
    {
        public bool HasConnectivity { get; internal set; }
        public ConnectivityLinkSpeedLevel LinkSpeedLevel { get; internal set; }
        public ConnectivityProfile[] AvailableProfiles { get; internal set; }
        public string[] Reasons { get; internal set; }
    }

    public enum ConnectivityLinkSpeedLevel : sbyte
    {
        NoConnectivity = -1,

        OK = 0,
        Slow = 10,
        VerySlow = 50,
        SuperSlow = 100,
    }

    public enum ConnectivityProfile : byte
    {
        Unknown = 0,

        /// <summary>The bluetooth data connection.</summary>
        Bluetooth = 1,

        /// <summary>The mobile/cellular data connection.</summary>
        Cellular = 2,

        /// <summary>The ethernet data connection.</summary>
        Ethernet = 3,

        /// <summary>The Wi-Fi data connection.</summary>
        WiFi = 4
    }
}
