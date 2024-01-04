using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Model
{
    class UserAuthKey : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Key { get; set; }
        public Note[] Notes { get; set; }
    }
}
