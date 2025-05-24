using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Validation.Engines.Abstract
{
    public abstract class ValidationEngineBase<TEntity> : ImAValidationEngine<TEntity>
    {
        #region Construct
        const string defaultGlobalReasonForMultipleFailedOperations = "There are multiple failure reasons; see comments for details.";
        protected virtual Func<TEntity, Task<OperationResult>>[] CustomRules { get; }

        #endregion

        public async Task<OperationResult<TEntity>> ValidateEntity(TEntity entity, string globalReasonIfNecesarry = null)
        {
            if (entity == null)
                return OperationResult.Fail($"{typeof(TEntity).Name} cannot be null").WithPayload(entity);

            OperationResult result = OperationResult.Win();

            if (CustomRules?.Any() ?? false)
                result
                    = (await Task.WhenAll(
                        CustomRules.Select(
                            rule => InvokeCustomRule(rule, entity)
                        )
                        .ToArray()
                    ))
                    .Merge(globalReasonIfNecesarry.IsEmpty() ? defaultGlobalReasonForMultipleFailedOperations : globalReasonIfNecesarry);

            return result.WithPayload(entity);
        }

        protected Task<OperationResult> ValidateNotNullOrEmpty(string name, object value)
        {
            if (value == null)
                return OperationResult.Fail($"{name} cannot be empty").AsTask();

            if (value is string && string.IsNullOrWhiteSpace(value as string))
                return OperationResult.Fail($"{name} cannot be empty").AsTask();

            return OperationResult.Win().AsTask();
        }

        protected Task<OperationResult> ValidateEmailAddressFormat(string name, string value)
        {
            OperationResult result = OperationResult.Win();

            new Action(() => new MailAddress(value)).TryOrFailWithGrace(onFail: x => result = OperationResult.Fail(x, $"{name} is invalid"));

            return result.AsTask();
        }

        private static async Task<OperationResult> InvokeCustomRule(Func<TEntity, Task<OperationResult>> rule, TEntity entity)
        {
            if (rule == null)
                return OperationResult.Win();

            OperationResult result = OperationResult.Win();

            await
                new Func<Task>(async () =>
                {
                    result = await rule.Invoke(entity);
                })
                .TryOrFailWithGrace(
                    numberOfTimes: 1,
                    onFail: ex => result = OperationResult.Fail(ex)
                );

            return result;
        }
    }
}
