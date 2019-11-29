using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace Api.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public UserController(IStudentService studentService)
        {
            _studentService = studentService;
        }


        //[HttpPost("authenticate")]
        //public IActionResult Authenticate([FromBody]Account userParam)
        //{
        //    var user = _studentService.Authenticate(userParam.username, userParam.password);

        //    if (user == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(user);
        //}

        [HttpGet]
        public IActionResult Get()
        {
            //var users = ;
            return Ok(_studentService.GetAll());
        }
    }
}
