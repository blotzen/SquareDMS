using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SquareDMS.RestEndpoint.Controllers
{
    [Route("api/fileformats")]
    [ApiController]
    public class FileFormatsController : ControllerBase
    {


        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// GET: api/<ValuesController>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<FileFormat> Get()
        {
            return null;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Partial Update of the given FileFormat.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchAsync(int id,
            [FromBody] JsonPatchDocument<FileFormat> fileFormatPatch)
        {
            if (fileFormatPatch is null)
                return BadRequest();

            // create empty fileFormat ..
            var patchedFileFormat = new FileFormat();

            // .. and apply patch on it
            fileFormatPatch.ApplyTo(patchedFileFormat);

            if (!TryValidateModel(patchedFileFormat))
                return BadRequest();

            // create dispatcher
            var fileFormatDispatcher = new FileFormatDispatcher();
            await fileFormatDispatcher.PatchFileFormatAsync(1, id, patchedFileFormat);

            return NoContent();
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
