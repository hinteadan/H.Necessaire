namespace H.Necessaire.Dapper
{
    public abstract class SqlEntryBase : ISqlEntry
    {
        public virtual string[] GetColumnNames()
        {
            return
                GetType().GetColumnNames();
        }
    }
}
