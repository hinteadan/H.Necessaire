using H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Model;
using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Filters
{
    class UserCredentialsFilter : IDFilter<Guid>
    {
        static readonly string[] validSortNames = new string[] {
            nameof(UserCredentials.ID),
            nameof(UserCredentials.Password),
            nameof(UserCredentials.UserInfo),
        };
        protected override string[] ValidSortNames => validSortNames;
        public string[] PasswordHashVersions { get; set; }
        public Guid[] UserInfoIDs { get; set; }
    }
}
