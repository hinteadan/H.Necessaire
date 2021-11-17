namespace H.Necessaire
{
    public enum SyncStatus
    {
        Inexistent = -1,

        NotSynced = 0,
        DeletedAndNotSynced = 1,

        Synced = 1000,
        DeletedAndSynced = 1001,
    }
}
