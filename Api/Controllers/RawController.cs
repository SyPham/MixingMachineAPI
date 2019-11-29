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
        //[HttpGet("detail/{id}")]
        //public IActionResult Get(string id)
        //{
        //    return Ok(_studentService.GetDetail(id));
        //}
        // GET api/values
        [HttpGet("detail/{id}/{start?}/{end?}")]
        public IActionResult Get(string id, string start = "", string end = "")
        {
            return Ok(_studentService.GetDetail(id, start, end));
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

        [HttpGet("exportexcel/{id}/{start?}/{end?}")]
        public IActionResult ExportExcel(string id, string start, string end)
        {
            byte[] data = _studentService.ExportExcel(id, start, end) as byte[];

            return File(data, "application/octet-stream", "DataUpload.xlsx");
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
