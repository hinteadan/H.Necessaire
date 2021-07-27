using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Validation.Engines
{
    public interface ImAValidationEngine<TEntity>
    {
        Task<OperationResult<TEntity>> ValidateEntity(TEntity entity);
    }
}
