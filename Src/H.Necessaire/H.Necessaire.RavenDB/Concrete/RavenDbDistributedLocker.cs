using System;
using System.Threading.Tasks;

namespace H.Necessaire.RavenDB.Concrete
{
    internal class RavenDbDistributedLocker : ImADistributedLocker, ImADependency
    {
        const string idPrefix = "DistributedLock";
        const string idSeparator = "::";
        string coreDatabaseName;
        RavenDbDocumentStore ravenDbDocumentStore;
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            ravenDbDocumentStore = dependencyProvider.Get<RavenDbDocumentStore>();
            coreDatabaseName = dependencyProvider?.GetRuntimeConfig()?.Get("RavenDbConnections")?.Get("DatabaseNames")?.Get("Core")?.ToString();
        }

        public async Task<OperationResult<DistributedLock>> Lock(string lockID, string ownerID, TimeSpan? lockDuration = null)
        {
            string docID = BuildID(lockID);

            using (var dbSession = ravenDbDocumentStore.Store.OpenAsyncSession(coreDatabaseName))
            {
                DistributedLock existingLock = await dbSession.LoadAsync<DistributedLock>(docID);

                DateTime now = DateTime.UtcNow;

                //New Lock
                if (existingLock is null)
                {
                    DistributedLock newLock = new DistributedLock { ID = lockID, OwnerID = ownerID }.AndIf(lockDuration != null, x => x.ExpireIn(lockDuration.Value));
                    await dbSession.StoreAsync(newLock, id: docID);

                    //This will fail on concurrency attempt therefore first caller will get the lock
                    if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                        return "The lock was available but it was just acquired faster by another contender";

                    return newLock;
                }

                if (existingLock.IsExpired())
                {
                    //Renew lock and change the owner

                    existingLock.OwnerID = ownerID;
                    existingLock.AsOf = now;
                    existingLock.ValidFrom = now;
                    existingLock.ValidFor = (lockDuration ?? DistributedLock.DefaultLockDuration);

                    //This will fail on concurrency attempt therefore first caller will get the lock
                    if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                        return "The lock was available as it was expired but it was just acquired faster by another contender";

                    return existingLock;
                }

                //Lock is active but owned by someone else
                if (existingLock.OwnerID != ownerID)
                    return $"The lock is not available, it's still active and owned by {existingLock.OwnerID}";

                //Lock is active and owned by self, we'll renew/extend it
                existingLock.AsOf = now;
                existingLock.ValidFrom = now;
                existingLock.ValidFor = (lockDuration ?? DistributedLock.DefaultLockDuration);

                //This will fail on concurrency attempt therefore first caller will get the lock
                if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                    return "The lock was still active and owned by self, but it just expired and was acquired faster by another contender";

                return existingLock;
            }
        }

        public async Task<OperationResult> Release(string lockID, string ownerID)
        {
            string docID = BuildID(lockID);

            using (var dbSession = ravenDbDocumentStore.Store.OpenAsyncSession(coreDatabaseName))
            {
                DistributedLock existingLock = await dbSession.LoadAsync<DistributedLock>(docID);

                //No lock, nothing to do, return win
                if (existingLock is null)
                    return true;

                //If expired, we don't care about the owner
                if (existingLock.IsExpired())
                {
                    dbSession.Delete(docID);

                    if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                        return "The lock was available for release as it was expired, but it was just renewed or released already by another contender";

                    return true;
                }

                //Lock is still active but owned by someone else
                if (existingLock.OwnerID != ownerID)
                    return $"The lock is not available for release, it's still active and owned by {existingLock.OwnerID}";

                //Lock is active and owned by self, we'll release it
                dbSession.Delete(docID);

                if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                    return "The lock was available for release as it was owned by self, but it was just renewed or released already by another contender";

                return true;
            }
        }

        static string BuildID(string lockID) => idPrefix + idSeparator + lockID;
    }
}
