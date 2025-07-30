namespace H.Necessaire.Dapper
{
    public interface ImADapperContextProvider
    {
        ImADapperContext GetNewDapperContext(string tableName = null);
    }
}
