using H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Abstracts;
using Retyped;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.BridgeDotNet.Runtime.ReactApp.Resources.Logging
{
    internal class LogEntryLocalStorageResource : IndexedDbStorageResourceBase<HNecessaireIndexedDBStorage, Guid, LogEntry, LogFilter>
    {
        protected override string TableName { get; } = nameof(HNecessaireIndexedDBStorage.Log);

        ImASyncer<LogEntry, Guid> syncer;
        public override void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            base.ReferDependencies(dependencyProvider);
            Use(FixIdioticBridgeJsonParser);
            syncer = dependencyProvider.Get<ImASyncer<LogEntry, Guid>>();
        }

        public override async Task<OperationResult> Save(LogEntry entity)
        {
            OperationResult result = await base.Save(entity);
            await syncer.Save(entity);
            return result;
        }

        protected override dexie.Dexie.Collection<object, object> ApplyFilter(dexie.Dexie.Collection<object, object> collection, LogFilter filter)
        {
            if (filter == null)
                return collection;

            var result = collection;

            if (filter.IDs?.Any() == true)
                result = result.and(x => ((string)x[nameof(LogEntry.ID)]).In(filter.IDs.ToStringArray()));

            if (filter.Levels?.Any() == true)
                result = result.and(x => ((int)x[nameof(LogEntry.Level)]).In(filter.Levels.Select(e => (int)e).ToArray()));

            if (filter.ScopeIDs?.Any() == true)
                result = result.and(x => ((string)x[nameof(LogEntry.ScopeID)]).In(filter.ScopeIDs.ToStringArray()));

            if (filter.Methods?.Any() == true)
                result = result.and(x => ((string)x[nameof(LogEntry.Method)]).In(filter.Methods));

            if (filter.Components?.Any() == true)
                result = result.and(x => ((string)x[nameof(LogEntry.Component)]).In(filter.Components));

            if (filter.Applications?.Any() == true)
                result = result.and(x => ((string)x[nameof(LogEntry.Application)]).In(filter.Applications));

            if (filter.FromInclusive != null)
                result = result.and(x => (DateTime.Parse((string)x[nameof(LogEntry.HappenedAt)])) >= filter.FromInclusive.Value);

            if (filter.ToInclusive != null)
                result = result.and(x => (DateTime.Parse((string)x[nameof(LogEntry.HappenedAt)])) <= filter.ToInclusive.Value);

            return result;
        }

        private LogEntry FixIdioticBridgeJsonParser(object json, LogEntry idioticParseResult)
        {
            if (json == null)
                return null;

            object[] jsonNotes = (object[])json[nameof(LogEntry.Notes)];

            if (!jsonNotes?.Any() ?? true)
                return idioticParseResult;

            idioticParseResult.Notes = jsonNotes.Select(x => new Note((string)x[nameof(Note.ID)], (string)x[nameof(Note.Value)])).ToArray();

            return idioticParseResult;
        }
    }
}
