using System;
using System.Collections.Generic;
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

    public class ConnectivityInfo : IEquatable<ConnectivityInfo>
    {
        public bool HasConnectivity { get; set; }
        public ConnectivityLinkSpeedLevel LinkSpeedLevel { get; set; }
        public ConnectivityProfile[] AvailableProfiles { get; set; }
        public string[] Reasons { get; set; }
        public TimeSpan? LatestResponseDuration { get; set; }

        public bool IsSameAs(ConnectivityInfo other, bool isReasonsCheckIncluded = false)
        {
            if (other is null)
                return false;

            return
                other.HasConnectivity == HasConnectivity
                && other.LinkSpeedLevel == LinkSpeedLevel
                && other.AvailableProfiles.IsSameAs(AvailableProfiles)
                && other.LatestResponseDuration == LatestResponseDuration
                && (!isReasonsCheckIncluded ? true : other.Reasons.IsSameAs(Reasons))
                ;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectivityInfo);
        }

        public bool Equals(ConnectivityInfo other) => IsSameAs(other);

        public override int GetHashCode()
        {
            int hashCode = -1092751998;
            hashCode = hashCode * -1521134295 + HasConnectivity.GetHashCode();
            hashCode = hashCode * -1521134295 + LinkSpeedLevel.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ConnectivityProfile[]>.Default.GetHashCode(AvailableProfiles);
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(Reasons);
            return hashCode;
        }

        public static bool operator ==(ConnectivityInfo a, ConnectivityInfo b) => a is null ? b is null : a?.IsSameAs(b) == true;
        public static bool operator !=(ConnectivityInfo a, ConnectivityInfo b) => a is null ? b != null : a?.IsSameAs(b) != true;
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
