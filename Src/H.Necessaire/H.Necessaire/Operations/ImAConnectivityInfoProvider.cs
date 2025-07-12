using System;
using System.Threading.Tasks;

namespace H.Necessaire.Operations
{
    public interface ImAConnectivityInfoProvider
    {
        event AsyncEventHandler<ConnectivityInfoChangedEventArgs> OnConnectivityInfoChanged;

        Task<ConnectivityInfo> GetConnectivityInfo();

        Task ForceRefresh();
    }

    public class ConnectivityInfoChangedEventArgs : EventArgs
    {
        public ConnectivityInfoChangedEventArgs(ConnectivityInfo connectivityInfo)
        {
            ConnectivityInfo = connectivityInfo;
        }

        public ConnectivityInfo ConnectivityInfo { get; }

        public static implicit operator ConnectivityInfoChangedEventArgs(ConnectivityInfo connectivityInfo) => new ConnectivityInfoChangedEventArgs(connectivityInfo);
    }

    public class ConnectivityInfo
    {
        public bool HasConnectivity { get; set; }
        public ConnectivityLinkSpeedLevel LinkSpeedLevel { get; set; }
        public ConnectivityProfile[] AvailableProfiles { get; set; }
        public string[] Reasons { get; set; }
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
