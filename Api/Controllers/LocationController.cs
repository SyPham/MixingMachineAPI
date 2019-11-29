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
    public class LocationController : ControllerBase
    {
        private ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

       // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(_locationService.Get(id));
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] Location model)
        {
            return Ok(_locationService.Add(model));
        }

        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] Location model)
        {
            return Ok(_locationService.Update(model));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok(_locationService.Delete(id));
        }
    }
}
