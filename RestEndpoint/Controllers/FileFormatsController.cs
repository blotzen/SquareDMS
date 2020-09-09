using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Core = SquareDMS.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SquareDMS.RestEndpoint.Controllers
{
    [Route("api/fileformats")]
    [ApiController]
    public class FileFormatsController : ControllerBase
    {
        /// <summary>
        /// GET: api/<ValuesController>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Core::Models.FileFormat> Get()
        {
            return null;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
