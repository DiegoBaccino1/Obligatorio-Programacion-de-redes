using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminConsumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIClientService.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IService service;
        public UsersController(IService service)
        {
            this.service = service;
        }
        
        [HttpPost]
        public IActionResult POSTUser(int id,[FromQuery]string username, [FromQuery] string password)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("New username is empty");
            if (string.IsNullOrEmpty(password))
                return BadRequest("New password is empty");
            try
            {
                bool ret = service.CreateUser(username, password);
                return Ok(ret);
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut]
        public IActionResult ModifyUser(int id, [FromQuery] string username, [FromQuery] string newPassword, [FromQuery] string newUsername)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is empty");
            if (string.IsNullOrEmpty(newUsername))
                return BadRequest("New username is empty");
            if (string.IsNullOrEmpty(newPassword))
                return BadRequest("New password is empty");
            try
            {
                string newCredentials = newUsername + "%" + newPassword;
                bool ret = service.ModifyUser(username, newCredentials);
                return Ok(ret);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username is empty");
            try
            {
                bool ret = service.DeleteUser(username);
                return Ok(ret);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
