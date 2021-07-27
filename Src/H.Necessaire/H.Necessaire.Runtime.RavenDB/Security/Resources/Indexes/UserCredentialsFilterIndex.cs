using H.Necessaire.Runtime.RavenDB.Security.Resources.Model;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes
{
    class UserCredentialsFilterIndex : AbstractIndexCreationTask<UserCredentials>
    {
        public UserCredentialsFilterIndex()
        {
            Map = docs => docs.Select(doc =>
                new
                {
                    doc.ID,
                    doc.Password,
                    doc.UserInfo,
                }
            );
        }
    }
}
