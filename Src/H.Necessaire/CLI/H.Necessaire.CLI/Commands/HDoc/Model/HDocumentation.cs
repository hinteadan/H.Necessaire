using System;

namespace H.Necessaire.CLI.Commands.HDoc.Model
{
    public class HDocumentation : EphemeralTypeBase
    {
        public HDocumentation()
        {
            CreatedAt = DateTime.UtcNow;
            AsOf = DateTime.UtcNow;
            ValidFrom = DateTime.UtcNow;
            ValidFor = null;
        }

        public HDocTypeInfo[] AllTypes { get; set; }
    }
}
