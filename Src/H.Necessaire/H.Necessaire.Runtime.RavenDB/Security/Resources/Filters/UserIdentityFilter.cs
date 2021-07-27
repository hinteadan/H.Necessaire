using System;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Filters
{
    class UserIdentityFilter
    {
        public Guid[] IDs { get; set; }
        public string[] Usernames { get; set; }
    }
}
