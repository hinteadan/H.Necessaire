using System;

namespace H.Necessaire.Testicles.Unit.Model.IoC
{
    class PureDependency : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
