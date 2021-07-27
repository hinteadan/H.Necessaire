using H.Necessaire.Runtime.RavenDB.Security.Resources.Model;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace H.Necessaire.Runtime.RavenDB.Security.Resources.Indexes
{
    class UserAuthKeyFilterIndex : AbstractIndexCreationTask<UserAuthKey>
    {
        public UserAuthKeyFilterIndex()
        {
            Map = docs => docs.Select(doc =>
                new
                {
                    doc.ID,
                }
            );
        }
    }
}
