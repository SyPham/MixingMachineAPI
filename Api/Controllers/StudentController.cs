using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class StudentController : Controller
    {

        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        // GET api/values
        [HttpGet("{id}")]
        public IActionResult Get(string id)

        {
            var model = _studentService.Test(id);
            return Ok(model);
        }


        // GET api/values/5
        //[HttpGet("{id}")]
        //public IActionResult Get(int id)
        //{
        //    return Ok(_studentService.Get(id));
        //}

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Student model)
        {
            return Ok(_studentService.Add(model));
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] Student model)
        {
            return Ok(_studentService.Update(model));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(_studentService.Delete(id));
        }
    }
}
