using Raven.Client.Documents.Indexes;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes
{
    class UserIdentityFilterIndex : AbstractIndexCreationTask<UserInfo>
    {
        public UserIdentityFilterIndex()
        {
            Map = docs => docs.Select(doc =>
                new
                {
                    doc.ID,
                    doc.Email,
                    doc.IDTag,
                    doc.Username,
                    doc.DisplayName,
                }
            );
        }
    }
}
