using System;

namespace H.Necessaire
{
    public class InternalIdentity : IDentityBase
    {
        public override Guid ID { get; set; } = Guid.Empty;
    }
}
