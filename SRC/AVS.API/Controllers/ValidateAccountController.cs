using AVS.Common.Utiities;
using AVS.Core;
using AVS.Data.Model;
using AVS.Data.ViewModel;
using AVS.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Com.Xpresspayments.AVS.API.Controllers
{
    [Route("api/AVS")]
    [ApiController]
    public class ValidateAccountController : ControllerBase
    {
        private readonly ILogger<ValidateAccountController> _logger;
        private readonly AccountValidationCore _accountValidationCore;
        private readonly ILoggerManager _loggerManager;

        public ValidateAccountController(ILogger<ValidateAccountController> logger,
                                         AccountValidationCore accountValidationCore, ILoggerManager loggerManager)
        {
            _logger = logger;
            _accountValidationCore = accountValidationCore;
            _loggerManager = loggerManager;
        }
        
        [HttpPost]
        [Route("ValidateAccount")]
        public async Task<ActionResult> GetAccountValidation([FromBody]AccountValidationVM accountValidationVM , [FromHeader] string AppId)
        {
            var IP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            _loggerManager.LogInformation($"Request from IP Address: {IP} Application Key {AppId} Message : {JsonConvert.SerializeObject(accountValidationVM)}");

            var resp = _accountValidationCore.ProcessAccountValidation(accountValidationVM, AppId , IP);

            _loggerManager.LogInformation($"Response from IP Address: {IP} Application Key {AppId} Message : {JsonConvert.SerializeObject(resp)}");

            return Ok(resp);
            
        }
    }
}
