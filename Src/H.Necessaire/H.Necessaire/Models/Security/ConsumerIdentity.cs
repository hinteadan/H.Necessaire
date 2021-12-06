namespace H.Necessaire
{
    public class ConsumerIdentity : IDentityBase, ImSyncable
    {
        public string IpAddress => Notes?.Get(WellKnownConsumerIdentityNote.IpAddress);
        public string HostName => Notes?.Get(WellKnownConsumerIdentityNote.HostName);
        public string Protocol => Notes?.Get(WellKnownConsumerIdentityNote.Protocol);
        public string UserAgent => Notes?.Get(WellKnownConsumerIdentityNote.UserAgent);
        public string AiUserID => Notes?.Get(WellKnownConsumerIdentityNote.AiUserID);
        public string Origin => Notes?.Get(WellKnownConsumerIdentityNote.Origin);
        public string Referer => Notes?.Get(WellKnownConsumerIdentityNote.Referer);
    }
}
