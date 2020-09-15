using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEndpoint.Services;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestEndpoint.Controllers
{
    /// <summary>
    /// Handles the HTTP-Operations
    /// for the Ressource Document-Version.
    /// Only Create and Get are supported.
    /// </summary>
    [Authorize]
    [Route("api/v1/documentversions")]
    [ApiController]
    public class DocumentVersionController : ControllerBase
    {
        private readonly DocumentVersionService _documentVersionService;

        /// <summary>
        /// Receives the Service via DI.
        /// </summary>
        public DocumentVersionController(DocumentVersionService documentVersionService)
        {
            _documentVersionService = documentVersionService;
        }

        /// <summary>
        /// Creates a new DocumentVersion in the DMS. Uses Streaming.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostDocumentVersionAsync([FromForm] DocumentVersion documentVersion)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _documentVersionService.CreateDocumentVersionAsync(userIdClaimed.Value, documentVersion);

            return Ok(postResult);
        }

        /// <summary>
        /// Gets a DocumentVersion by its id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult> GetDocumentVersionAsync(int id)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var retrievalResult = await _documentVersionService.RetrieveDocumentVersionAsync(userIdClaimed.Value, id);

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);

            var retrievedDocumentVersion = retrievalResult.Resultset.FirstOrDefault();

            if (retrievedDocumentVersion is null)
                return null;

            var streamResult = new FileStreamResult(retrievedDocumentVersion.DownloadFile.DownloadFileStream,
                retrievedDocumentVersion.DownloadFile.MediaType);

            return streamResult;
        }
    }
}
