using H.Necessaire.Serialization;

namespace H.Necessaire.Runtime
{
    public static class WellKnown
    {
        public static class QdActionType
        {
            public static string ProcessIpAddress = "ProcessIpAddress";
            public static string ProcessIpAddressPayload(string ipAddress, string ownerID) => $"{ipAddress}|{ownerID}";
            public static string ProcessIpAddressPayload(ConsumerIdentity consumerIdentity) => ProcessIpAddressPayload(consumerIdentity.IpAddress, consumerIdentity.ID.ToString());


            public static string ProcessRuntimePlatform = "ProcessRuntimePlatform";
            public static string ProcessRuntimePlatformPayload(ConsumerIdentity consumerIdentity) => consumerIdentity.ToJsonObject();
        }
    }
}
