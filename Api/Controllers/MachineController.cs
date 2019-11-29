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
    public class MachineController : Controller
    {

        //private readonly IStudentService _studentService;
        private readonly IMachineService _machineService;
        public MachineController(IMachineService machineService)
        {
            _machineService = machineService;
            
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_machineService.GetAll());
        }
        // GET api/values
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_machineService.Get(id));
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Machine model)
        {
            return Ok(_machineService.Add(model));
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] Machine model)
        {
            return Ok(_machineService.Update(model));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return Ok(_machineService.Delete(id));
        }
    }
}
