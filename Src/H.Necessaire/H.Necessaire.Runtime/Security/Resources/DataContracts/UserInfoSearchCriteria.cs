using System;

namespace H.Necessaire.Runtime.Security.Resources.DataContracts
{
    public class UserInfoSearchCriteria
    {
        public Guid[] IDs { get; set; }
        public string[] Usernames { get; set; }
    }
}
