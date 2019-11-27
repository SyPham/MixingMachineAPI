using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;
using Persistence;
using Service;

namespace Api.Controllers
{
    [Route("[controller]")]
    public class RawController : Controller
    {

        private readonly StudentDbcontext _db;
        private readonly IStudentService _studentService;
        public RawController(StudentDbcontext db, IStudentService studentService)
        {
            _db = db;
            _studentService = studentService;
        }
        // GET api/values
        [HttpGet("detail/{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_studentService.GetDetail(id));
        }
        // GET api/values
        [HttpGet("detail/{id}/{value?}")]
        public IActionResult Get(string id,string value)
        {
            return Ok(_studentService.GetDetail(id,value));
        }
        [HttpGet("getrpm/{id}")]
        public async Task<IActionResult> GetRPM(string id)
        {
            return Ok(await _studentService.GetRPM(id));
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var model = from m in _db.machine
                        join lc in _db.location on m.locationID equals lc.id
                        select new
                        {
                            m.machineID,
                            m.description,
                            lc.locationname
                        };
            return Ok(model);
        }


        [HttpGet("getchart/{id}")]
        public IActionResult GetChart(string id)
        {
            return Ok(_studentService.Test(id));
        }
        // GET api/values/5
        //[HttpGet("{id}")]
        //public IActionResult Get(int id)
        //{
        //    return Ok();
        //}

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Student model)
        {
            return Ok();
             
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] Student model)
        {
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok();
        }
    }
}
