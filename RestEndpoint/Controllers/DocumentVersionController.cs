using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEndpoint.Services;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Linq;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Controllers
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
        /// Gets a DocumentVersions payload by its id. Returns an empty result if the
        /// document version wasnt found. (e.g. documentversions/?documentId=12&documentVersionId=3)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetDocumentVersionMetadataAsync(int id,
            [FromQuery] int? documentVersionId, [FromQuery] int? documentId)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            // bad request if no userId was found, or both params are null
            if (userIdClaimed is null || (documentVersionId is null && documentId is null))
                return BadRequest();

            return Ok(await _documentVersionService.RetrieveDocumentVersionMetadataAsync(userIdClaimed.Value,
                documentVersionId, documentId));
        }

        /// <summary>
        /// Gets a DocumentVersions payload by its id. Returns an empty result if the
        /// document version wasnt found.
        /// </summary>
        [HttpGet("{id}/payload")]
        public async Task<ActionResult> GetDocumentVersionPayloadAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var retrievalResult = await _documentVersionService.RetrieveDocumentVersionPayloadAsync(userIdClaimed.Value, id);
            var retrievedDocumentVersion = retrievalResult.Resultset.FirstOrDefault();

            if (retrievedDocumentVersion is null || retrievedDocumentVersion.DownloadFile is null)
                return new EmptyResult();

            return retrievedDocumentVersion.DownloadFile;
        }
    }
}
