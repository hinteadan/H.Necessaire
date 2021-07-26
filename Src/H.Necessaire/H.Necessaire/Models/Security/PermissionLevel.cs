namespace H.Necessaire
{
    public enum PermissionLevel
    {
        Locked = -1,

        Read = 0,
        ReadAndExecute = 1,

        Modify = 50,

        Create = 100,

        FullControl = 1000,
    }
}
