using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consumer;
using Consumers;
using Entities;
using LogServerImp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace LogAPI.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        public  LogsController()
        {
        }

        [HttpGet]
        public IActionResult GetAllLogs()
        {
            List<Log> logs = SingletonRepository.GetLog();
            return Ok(logs);
        }
        [HttpGet("success")]
        public IActionResult GetSuccessLogs()
        {
            List<Log> logs = SingletonRepository.GetLog().Where(x=>x.Level.Equals(Log.SUCCESS_LEVEL)).ToList();
            return Ok(logs);
        }
        [HttpGet("warning")]
        public IActionResult GetWarningLogs()
        {
            List<Log> logs = SingletonRepository.GetLog().Where(x => x.Level.Equals(Log.WARNING_LEVEL)).ToList();
            return Ok(logs);
        }
    }
}
