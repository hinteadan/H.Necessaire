using System;

namespace H.Necessaire
{
    public class DistributedLock : EphemeralTypeBase, IStringIdentity
    {
        public static readonly TimeSpan DefaultLockDuration = TimeSpan.FromSeconds(30);

        public DistributedLock() => ExpireIn(DefaultLockDuration);

        public string ID { get; set; }
        public string OwnerID { get; set; }
    }
}
