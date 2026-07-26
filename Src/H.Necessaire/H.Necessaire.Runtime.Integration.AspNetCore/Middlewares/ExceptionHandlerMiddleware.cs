using H.Necessaire;
using H.Necessaire.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace H.Necessaire.Runtime.Integration.AspNetCore.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        #region Construct
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }
        #endregion

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationResultException<UseCaseContext> exception)
            {
                await HandleOperationResultException(context, exception, exception?.OperationResult?.Payload?.FailContext?.ReasonCode ?? (int)HttpStatusCode.BadRequest);
            }
            catch (Exception exception)
            {
                await HandleOperationResultException(context, exception);
            }
        }

        async Task HandleOperationResultException(HttpContext context, Exception ex, int httpStatusCode = (int)HttpStatusCode.BadRequest)
        {
            logger.LogError(ex, $"API Endpoint Exception @ {context.Request.Path}: {ex.Message}");
            foreach (Exception flatException in ex.Flatten() ?? [])
            {
                logger.LogError(flatException, flatException.Message);
            }

            ExceptionPresentationModel result = ex;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;

            await context.Response.WriteAsync(result.ToJsonObject());
        }
    }
}
