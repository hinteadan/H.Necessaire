using System;

namespace H.Necessaire.Runtime.Google.FirestoreDB.Security.Resources.Filters
{
    class UserIdentityFilter : IDFilter<Guid>
    {
        static readonly string[] validSortNames = new string[] {
            nameof(UserInfo.ID),
            nameof(UserInfo.IDTag),
            nameof(UserInfo.DisplayName),
            nameof(UserInfo.AsOf),
            nameof(UserInfo.Username),
            nameof(UserInfo.Email),
            nameof(UserInfo.IpAddress),
            nameof(UserInfo.HostName),
            nameof(UserInfo.Protocol),
            nameof(UserInfo.UserAgent),
            nameof(UserInfo.Origin),
            nameof(UserInfo.Referer),
        };
        protected override string[] ValidSortNames => validSortNames;

        public string[] Usernames { get; set; }
    }
}
