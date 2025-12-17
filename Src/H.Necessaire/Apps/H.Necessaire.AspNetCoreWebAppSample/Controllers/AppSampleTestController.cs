using H.Necessaire.Runtime.Integration.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.AspNetCoreWebAppSample.Controllers
{
    [Route(HAspNetCoreConstants.ApiControllerBaseRoute)]
    [ApiController]
    public class AppSampleTestController : ControllerBase
    {
        [Route("ping"), HttpGet]
        public Task<string> Pong() => $"AppSampleTestController.Pong @ {DateTime.Now.PrintDateAndTime()}".AsTask();
    }
}
