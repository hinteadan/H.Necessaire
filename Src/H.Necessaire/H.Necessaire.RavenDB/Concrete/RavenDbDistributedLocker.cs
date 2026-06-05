using System;
using System.Threading;
using System.Threading.Tasks;

namespace H.Necessaire.RavenDB.Concrete
{
    internal class RavenDbDistributedLocker : ImADistributedLocker, ImADependency
    {
        const string idPrefix = "DistributedLock";
        const string idSeparator = "::";
        string coreDatabaseName;
        RavenDbDocumentStore ravenDbDocumentStore;
        readonly SemaphoreSlim lockSemaphore = new SemaphoreSlim(1, 1);
        public void ReferDependencies(ImADependencyProvider dependencyProvider)
        {
            ravenDbDocumentStore = dependencyProvider.Get<RavenDbDocumentStore>();
            coreDatabaseName = dependencyProvider?.GetRuntimeConfig()?.Get("RavenDbConnections")?.Get("DatabaseNames")?.Get("Core")?.ToString();
        }

        public async Task<OperationResult<DistributedLock>> Lock(string lockID, string ownerID, TimeSpan? lockDuration = null)
        {
            string docID = BuildID(lockID);

            await lockSemaphore.WaitAsync();
            using (new ScopedRunner(null, _ => lockSemaphore.Release()))
            {
                return (await HSafe.Run<OperationResult<DistributedLock>>(async () =>
                {
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
                                return OperationResult.Fail("The lock was available but it was just acquired faster by another contender").WithComment("DoNotLog").WithoutPayload<DistributedLock>();

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
                                return OperationResult.Fail("The lock was available as it was expired but it was just acquired faster by another contender").WithComment("DoNotLog").WithoutPayload<DistributedLock>();

                            return existingLock;
                        }

                        //Lock is active but owned by someone else
                        if (existingLock.OwnerID != ownerID)
                            return OperationResult.Fail($"The lock is not available, it's still active and owned by {existingLock.OwnerID}").WithComment("DoNotLog").WithoutPayload<DistributedLock>();

                        //Lock is active and owned by self, we'll renew/extend it
                        existingLock.AsOf = now;
                        existingLock.ValidFrom = now;
                        existingLock.ValidFor = (lockDuration ?? DistributedLock.DefaultLockDuration);

                        //This will fail on concurrency attempt therefore first caller will get the lock
                        if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                            return OperationResult.Fail("The lock was still active and owned by self, but it just expired and was acquired faster by another contender").WithComment("DoNotLog").WithoutPayload<DistributedLock>();

                        return existingLock;
                    }
                })).UnwrapToFirstFailOrLastWin();
            }
        }

        public async Task<OperationResult> Release(string lockID, string ownerID)
        {
            string docID = BuildID(lockID);

            return (await HSafe.Run<OperationResult>(async () =>
            {
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
                            return OperationResult.Fail("The lock was available for release as it was expired, but it was just renewed or released already by another contender").WithComment("DoNotLog");

                        return true;
                    }

                    //Lock is still active but owned by someone else
                    if (existingLock.OwnerID != ownerID)
                        return OperationResult.Fail($"The lock is not available for release, it's still active and owned by {existingLock.OwnerID}").WithComment("DoNotLog");

                    //Lock is active and owned by self, we'll release it
                    dbSession.Delete(docID);

                    if (!await HSafe.Run(async () => await dbSession.SaveChangesAsync()))
                        return OperationResult.Fail("The lock was available for release as it was owned by self, but it was just renewed or released already by another contender").WithComment("DoNotLog");

                    return true;
                }

            })).UnwrapToFirstFailOrLastWin();
        }

        static string BuildID(string lockID) => idPrefix + idSeparator + lockID;
    }
}
