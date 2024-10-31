using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Model
{
    class UserCredentials : IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Password { get; set; }
        public Guid UserInfo { get; set; }
    }
}
