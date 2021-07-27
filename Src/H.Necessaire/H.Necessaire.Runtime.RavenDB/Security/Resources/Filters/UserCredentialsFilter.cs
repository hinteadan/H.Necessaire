using System;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Filters
{
    class UserCredentialsFilter
    {
        public Guid[] IDs { get; set; }
        public string[] PasswordHashVersions { get; set; }
        public Guid[] UserInfoIDs { get; set; }
    }
}
