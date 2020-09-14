using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEndpoint.Services;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace RestEndpoint.Controllers
{
    [Authorize]
    [Route("api/v1/documentversions")]
    [ApiController]
    public class DocumentVersionController : ControllerBase
    {
        private readonly DocumentVersionService _documentVersionService;

        /// <summary>
        /// 
        /// </summary>
        public DocumentVersionController(DocumentVersionService documentVersionService)
        {
            _documentVersionService = documentVersionService;
        }

        /// <summary>
        /// Creates a new DocumentVersion in the DMS. Uses Streaming.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostFileFormatAsync([FromForm] DocumentVersion documentVersion)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _documentVersionService.CreateDocumentVersionAsync(userIdClaimed.Value, documentVersion);

            return Ok(postResult);
        }


    }
}
