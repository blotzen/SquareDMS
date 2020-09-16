using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint;
using SquareDMS.Services;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Controllers
{
    /// <summary>
    /// Handles the HTTP-Operations 
    /// for the Ressource Document.
    /// </summary>
    [Authorize]
    [Route("api/v1/documents")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;

        /// <summary>
        /// Receives the DocumentService via DI.
        /// </summary>
        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Creates a new Document.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostDocumentAsync([FromBody] Document document)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _documentService.CreateDocumentAsync(userIdClaimed.Value, document);

            return Ok(postResult);
        }

        /// <summary>
        /// Retrieves documents depending on the given paramters.
        /// </summary>
        public async Task<ActionResult<RetrievalResult<Document>>> GetDocumentAsync([FromQuery] int? documentId,
            [FromQuery] int? creator, [FromQuery] int? docType, [FromQuery] string name,
            [FromQuery] bool? locked, [FromQuery] bool? dicard)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var retrievalResult = await _documentService.RetrieveDocumentAsync(userIdClaimed.Value, documentId,
                creator, docType, name, locked, dicard);

            return Ok(retrievalResult);
        }

        /// <summary>
        /// Partial Update of the given Document.
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult>> PatchDocumentAsync(int id,
            [FromBody] JsonPatchDocument<Document> documentPatch)
        {
            if (documentPatch is null)
                return BadRequest();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            // create empty document ..
            var patchedDocument = new Document();

            // .. and apply patch on it
            documentPatch.ApplyTo(patchedDocument);

            if (!TryValidateModel(patchedDocument))
                return BadRequest("Patch syntax invalid");

            return Ok(await _documentService.UpdateDocumentAsync(userIdClaimed.Value, id, patchedDocument));
        }

        /// <summary>
        /// Delets a document.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult>> DeleteDocumentAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _documentService.DeleteDocumentAsync(userIdClaimed.Value, id));
        }
    }
}