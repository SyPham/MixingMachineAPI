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
            //var model = (from m in _db.machine
            //            join rw in _db.rawdata on m.machineID equals rw.machineID
            //            //orderby rw.createddatetime
            //            //where rw.sequence ==21
            //            select new
            //            {
            //                m.machineID,
            //                rw.createddatetime,
            //                m.description,
            //                rw.sequence,
            //            }).Distinct();
            //var arr = new object[]
            //{
            //    model.FirstOrDefault().machineID,
            //    model.LastOrDefault().sequence,
            //    model.FirstOrDefault().createddatetime,
            //    model.LastOrDefault().createddatetime
            //};
            var model = new List<MachineDetail>();

            //var sequenceList = (from s in _db.rawdata                              
            //                   select new
            //                   {
            //                      s.sequence
            //                   }
            //                   ).Distinct().ToList();
            var sequenceList = _db.rawdata.Where(x => x.machineID == id).Select(y=> y.sequence).Distinct().ToList();

            foreach (var item in sequenceList)
            {
                var vm = new MachineDetail();
                vm.MachineID = _db.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.machineID).FirstOrDefault();
                vm.StartTime = _db.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Min();
                vm.EndTime = _db.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.createddatetime).Max();
                vm.RPM = _db.rawdata.Where(x => x.machineID.Equals(id) && x.sequence == item).Select(y => y.RPM).Average();
                model.Add(vm);
            }


            return Ok(model);
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
