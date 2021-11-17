using Retyped;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp
{
    public class ConsumerIdentityLocalStorageResource : IndexedDbResourceBase<HNecessaireIndexedDBStorage, ConsumerIdentity, Guid>
    {
        #region Construct
        ImASyncer<ConsumerIdentity, Guid> syncer;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            Use(FixIdioticBridgeJsonParser);
            syncer = dependencyProvider.Get<ImASyncer<ConsumerIdentity, Guid>>();
        }
        #endregion

        public Task<ConsumerIdentity> Load(Guid id)
        {
            return base.Load(storage.ConsumerIdentity, id);
        }

        public async Task Delete(Guid id)
        {
            await base.Delete(storage.ConsumerIdentity, id);
            await syncer.Delete(id);
        }

        public async Task Save(ConsumerIdentity consumerIdentity)
        {
            await base.Save(storage.ConsumerIdentity, consumerIdentity);
            await syncer.Save(consumerIdentity);
        }

        public async Task<ConsumerIdentity> Search()
        {
            return (await base.Search(null))?.FirstOrDefault();
        }

        protected override dexie.Dexie.Collection<object, object> ApplyFilter(object filter)
        {
            return storage.ConsumerIdentity.toCollection();
        }

        protected override Guid GetIdFor(ConsumerIdentity item) => item.ID;

        private ConsumerIdentity FixIdioticBridgeJsonParser(object json, ConsumerIdentity idioticParseResult)
        {
            if (json == null)
                return null;

            idioticParseResult.Notes = ((object[])json[nameof(ConsumerIdentity.Notes)]).Select(x => new Note((string)x[nameof(Note.ID)], (string)x[nameof(Note.Value)])).ToArray();
            return idioticParseResult;
        }
    }
}
