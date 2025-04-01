using H.Necessaire.CLI.Commands;
using System;
using System.Threading.Tasks;
using H.Necessaire.Couchbase.Lite.Querying;
using System.Linq;

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

                    var query = personScope.Select<Person>(
                        HCb.Select<Person>((x => x.ID, nameof(Person), nameof(PersonWithAddress.PersonID)))
                        , HCb.Select<Person>((x => x.FirstName, nameof(Person), nameof(PersonWithAddress.FirstName)))
                        , HCb.Select<Person>((x => x.LastName, nameof(Person), nameof(PersonWithAddress.LastName)))
                        , HCb.Select<Person>((x => x.GeoAddressID, nameof(Person), nameof(Person.GeoAddressID)))
                        , HCb.Select<GeoAddressWithID>((x => x.ID, nameof(GeoAddressWithID), nameof(PersonWithAddress.AddressID)))
                        , HCb.Select<GeoAddressWithID>((x => x.StreetAddress, nameof(GeoAddressWithID), nameof(PersonWithAddress.StreetAddress)))
                        , HCb.Select<GeoAddressWithID>((x => x.Country, nameof(GeoAddressWithID), nameof(PersonWithAddress.Country)))
                    ).Join().With<Person, GeoAddressWithID>(addressScope, nameof(GeoAddressWithID), (p, a) => p.GeoAddressID == a.ID)
                    ;

                    var results = (await personScope.StreamQuery<PersonWithAddress>(query)).ThrowOnFailOrReturn().ToArray();

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

    class PersonWithAddress : GeoAddress
    {
        public Guid PersonID { get; set; }
        public Guid AddressID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
