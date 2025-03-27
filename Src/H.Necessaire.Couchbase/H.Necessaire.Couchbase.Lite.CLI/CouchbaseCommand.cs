using H.Necessaire.CLI.Commands;
using System;
using System.Threading.Tasks;
using H.Necessaire.Couchbase.Lite.Querying;

namespace H.Necessaire.Couchbase.Lite.CLI
{
    [Alias("couch")]
    internal class CouchbaseCommand : CommandBase
    {
        static readonly string[] usageSyntax = [
            "couchbase|couch debug"
        ];
        protected override string[] GetUsageSyntaxes() => usageSyntax;

        public override Task<OperationResult> Run() => RunSubCommand();

        class DebugSubCommand : SubCommandBase
        {
            public override async Task<OperationResult> Run(params Note[] args)
            {
                await Task.CompletedTask;

                var couch = new CouchbaseInteractor("dev-play-db");

                using (var personScope = couch.NewOperationScope(collectionName: nameof(Person)))
                using (var addressScope = couch.NewOperationScope(collectionName: nameof(GeoAddressWithID)))
                {
                    //var address = new GeoAddressWithID();
                    //var person = new Person { GeoAddressID = address.ID };

                    //(await addressScope.Save<GeoAddressWithID, Guid>(address)).ThrowOnFail();
                    //(await personScope.Save<Person, Guid>(person)).ThrowOnFail();

                    personScope.SelectAll<Person>().Where(x => x.CreatedAt.FromAlias(nameof(Person)) >= DateTime.UtcNow);
                }

                return OperationResult.Win();
            }
        }
    }

    class Person : EphemeralTypeBase, IGuidIdentity
    {
        public Person() => DoNotExpire();

        public Guid ID { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid? GeoAddressID { get; set; }
    }

    class GeoAddressWithID : GeoAddress, IGuidIdentity
    {
        public Guid ID { get; set; } = Guid.NewGuid();
    }
}
