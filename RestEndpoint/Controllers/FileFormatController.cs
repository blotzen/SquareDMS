using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint.Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SquareDMS.RestEndpoint.Controllers
{
    /// <summary>
    /// Handles the HTTP-Operations 
    /// for the Ressource FileFormat.
    /// </summary>
    [Authorize]
    [Route("api/v1/fileformats")]
    [ApiController]
    public class FileFormatController : ControllerBase
    {
        private readonly FileFormatService _fileFormatService;

        /// <summary>
        /// Receives the FileFormatService via DI.
        /// </summary>
        public FileFormatController(FileFormatService fileFormatService)
        {
            _fileFormatService = fileFormatService;
        }

        /// <summary>
        /// Creates a new FileFormat in the DMS.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostFileFormatAsync([FromBody] FileFormat fileFormat)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _fileFormatService.CreateFileFormatAsync(userIdClaimed.Value, fileFormat);

            return Ok(postResult);
        }

        /// <summary>
        /// Gets FileFormats with the matching conditions in the query
        /// string. (e.g. .../fileFormats/?Description=Cool)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<FileFormat>>> GetFileFormatsAsync(
            [FromQuery] int? fileFormatId, [FromQuery] string extension,
            [FromQuery] string description)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _fileFormatService.RetrieveFileFormatsAsync(userIdClaimed.Value,
                fileFormatId, extension, description));
        }

        /// <summary>
        /// Partial Update of the given FileFormat.
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult>> PatchFileFormatAsync(int id,
            [FromBody] JsonPatchDocument<FileFormat> fileFormatPatch)
        {
            if (fileFormatPatch is null)
                return BadRequest();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            // create empty fileFormat ..
            var patchedFileFormat = new FileFormat();

            // .. and apply patch on it
            fileFormatPatch.ApplyTo(patchedFileFormat);

            if (!TryValidateModel(patchedFileFormat))
                return BadRequest("Patch syntax invalid");

            var patchResult = await _fileFormatService.UpdateFileFormatAsync(userIdClaimed.Value, id, patchedFileFormat);

            if (patchResult is null)
                return BadRequest("Tried to update non updateable attributes");

            return Ok(patchResult);
        }

        /// <summary>
        /// Delets a fileFormat.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult>> DeleteFileFormatAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _fileFormatService.DeleteFileFormatAsync(userIdClaimed.Value, id));
        }
    }
}
