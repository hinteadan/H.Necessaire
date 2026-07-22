using H.Necessaire.RavenDB;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.RavenDB.Core.Versioning
{
    internal class RavenDbReleaseVersionStore : ImAReleaseVersionStore, ImAReleaseVersionProvider, ImADependency
    {
        #region Construct
        string databaseName = null;
        RavenDbDocumentStore ravenDbDocumentStore;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            RuntimeConfig runtimeConfig = dependencyProvider?.GetRuntimeConfig();
            ConfigNode dbNamesConfig = runtimeConfig?.Get("RavenDbConnections")?.Get("DatabaseNames");
            databaseName = dbNamesConfig?.Get("Default")?.ToString()?.NullIfEmpty() ?? dbNamesConfig?.Get("Core")?.ToString()?.NullIfEmpty();
            if (databaseName.IsEmpty())
                throw new OperationResultException("No RavenDB database name specified. Must configure either Core or Default database name via RavenDbConnections.DatabaseNames.Default or RavenDbConnections.DatabaseNames.Core configs.");

            ravenDbDocumentStore = dependencyProvider.Get<RavenDbDocumentStore>();
        }
        #endregion Construct

        public async Task<OperationResult> Save(params ReleaseVersion[] releaseVersions)
        {
            releaseVersions = releaseVersions.ToNoNullsArray();
            if (releaseVersions.IsEmpty())
                return true;

            OperationResult result = null;
            await HSafe.RunWithRetry(async () =>
            {
                if (!HSafe.Run(() => ravenDbDocumentStore.Store.OpenAsyncSession(databaseName), "RavenDB.OpenAsyncSession").Ref(out var sessRes, out var dbSess))
                {
                    result = sessRes;
                    return;
                }

                using (dbSess)
                {
                    string[] ids = releaseVersions.Select(x => x.ID.ToDocID()).ToArray();
                    Dictionary<string, ReleaseVersion> existingReleaseVersions = await dbSess.LoadAsync<ReleaseVersion>(ids);

                    foreach (ReleaseVersion versionToSave in releaseVersions)
                    {
                        string versionToSaveDocID = versionToSave.ID.ToDocID();
                        bool isAlreadyExisting = existingReleaseVersions.TryGetValue(versionToSaveDocID, out ReleaseVersion existingReleaseVersion) && existingReleaseVersion != null;

                        if (!isAlreadyExisting)
                        {
                            await dbSess.StoreAsync(versionToSave, versionToSaveDocID);
                            continue;
                        }

                        if (versionToSave.IsHumanReviewed)
                        {
                            versionToSave.CopyTo(existingReleaseVersion);
                            continue;
                        }

                        if (existingReleaseVersion.IsHumanReviewed)
                        {
                            continue;
                        }

                        versionToSave.CopyTo(existingReleaseVersion);
                    }

                    result = await HSafe.Run(async () => await dbSess.SaveChangesAsync(), "RavenDB.SaveChangesAsync()");
                }
            }, () => result?.IsSuccessful == true);

            return result;
        }

        public async Task<ReleaseVersion[]> GetAllReleases(string idStartingWith = null)
        {
            using (var dbSess = ravenDbDocumentStore.Store.OpenAsyncSession(new SessionOptions { Database = databaseName, NoTracking = true }))
            {
                IAsyncDocumentQuery<ReleaseVersion> query = dbSess.Advanced.AsyncDocumentQuery<ReleaseVersion>();

                if (!idStartingWith.IsEmpty())
                    query = query.WhereStartsWith("id()", idStartingWith.ToDocID());

                query = query.OrderByDescending(x => x.Version.Timestamp);

                ReleaseVersion[] results = await query.ToArrayAsync();

                return results;
            }
        }
    }

    internal static class RavenDbReleaseVersionStoreXtnx
    {
        const string idPrefix = "ReleaseVersion";
        const string idSeparator = "::";
        public static string ToDocID(this string dataID) => string.Join(idSeparator, idPrefix, dataID);

        public static void CopyTo(this ReleaseVersion source, ReleaseVersion destination)
        {
            if (source is null || destination is null)
                throw new InvalidOperationException("Source or Destination is null");

            destination.Description = source.Description;
            destination.Name = source.Name;
            destination.Notes = source.Notes;
            destination.Version = source.Version;
            destination.ID = source.ID;
        }
    }
}
