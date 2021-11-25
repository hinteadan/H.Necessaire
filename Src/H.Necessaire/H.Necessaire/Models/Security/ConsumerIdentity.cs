namespace H.Necessaire
{
    public class ConsumerIdentity : IDentityBase, ImSyncable
    {
        public string IpAddress => Notes?.Get(WellKnownConsumerIdentityNote.IpAddress);
    }
}
