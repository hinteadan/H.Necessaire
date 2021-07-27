using System;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Model
{
    class UserAuthKey : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Key { get; set; }
    }
}
