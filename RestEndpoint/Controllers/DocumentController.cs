using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NLog;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
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
        private readonly Logger _logger;

        /// <summary>
        /// Receives the DocumentService via DI.
        /// </summary>
        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
            _logger = LogManager.GetLogger("DocumentController");
        }

        /// <summary>
        /// Creates a new Document.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostDocumentAsync([FromBody] Document document)
        {
            _logger.Info("Startet creating a new Document");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var postResult = await _documentService.CreateDocumentAsync(userIdClaimed.Value, document);

            _logger.Info("New Document ({0}) created with ErrorCode {1}", document.Name, postResult.ErrorCode);

            return Ok(postResult);
        }

        /// <summary>
        /// Retrieves documents depending on the given paramters.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<Document>>> GetDocumentsAsync([FromQuery] int? documentId,
            [FromQuery] int? creator, [FromQuery] int? docType, [FromQuery] string name,
            [FromQuery] bool? locked, [FromQuery] bool? dicard)
        {
            _logger.Info("Startet retrieving Documents");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var retrievalResult = await _documentService.RetrieveDocumentAsync(userIdClaimed.Value, documentId,
                creator, docType, name, locked, dicard);

            _logger.Info("Documents retrieved with ErrorCode {0}", retrievalResult.ErrorCode);

            return Ok(retrievalResult);
        }

        /// <summary>
        /// Partial Update of the given Document.
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult>> PatchDocumentAsync(int id,
            [FromBody] JsonPatchDocument<Document> documentPatch)
        {
            _logger.Info("Startet updating a Document");

            if (documentPatch is null)
                return BadRequest("Patch body is empty");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            // create empty document ..
            var patchedDocument = new Document();

            // .. and apply patch on it
            documentPatch.ApplyTo(patchedDocument);

            if (!TryValidateModel(patchedDocument))
            {
                _logger.Info("Patch syntax invalid");
                return BadRequest("Patch syntax invalid");
            }

            var patchResult = await _documentService.UpdateDocumentAsync(userIdClaimed.Value, id, patchedDocument);

            if (patchResult is null)
            {
                _logger.Info("Tried to update non updateable attributes");
                return BadRequest("Tried to update non updateable attributes");
            }

            _logger.Info("Document updated with ErrorCode {0}", patchResult.ErrorCode);

            return Ok(patchResult);
        }

        /// <summary>
        /// Delets a document.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult>> DeleteDocumentAsync(int id)
        {
            _logger.Info("Startet deleting a Document");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var deletionResult = await _documentService.DeleteDocumentAsync(userIdClaimed.Value, id);

            _logger.Info("Document deleted with ErrorCode {0}", deletionResult.ErrorCode);

            return Ok(deletionResult);
        }
    }
}