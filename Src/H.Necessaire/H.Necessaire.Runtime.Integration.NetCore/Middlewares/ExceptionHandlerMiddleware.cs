using H.Necessaire.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.Integration.NetCore.Middlewares
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
            catch (OperationResultException exception)
            {
                await HandleOperationResultException(context, exception);
            }
            catch (Exception exception)
            {
                await HandleOperationResultException(context, exception);
            }
        }

        private async Task HandleOperationResultException(HttpContext context, Exception ex, int httpStatusCode = (int)HttpStatusCode.BadRequest)
        {
            logger.LogError(ex, $"API Endpoint Exception @ {context.Request.Path}: {ex.Message}");
            foreach (Exception flatException in ex.Flatten())
            {
                logger.LogError(flatException, flatException.Message);
            }

            string result = new
            {
                error = "There was an error processing your request. Check the reasons below for details.",
                reasons = (ex as OperationResultException)?.OperationResult?.FlattenReasons() ?? "Unknown reasons, please contact an administrator".AsArray(),
            }
            .ToJsonObject();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = httpStatusCode;

            await context.Response.WriteAsync(result);
        }
    }
}
