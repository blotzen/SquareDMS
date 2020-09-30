using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.Services;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Controllers
{
    /// <summary>
    /// Handles the HTTP-Operations 
    /// for the Ressource DocumentType.
    /// </summary>
    [Authorize]
    [Route("api/v1/DocumentTypes")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly DocumentTypeService _documentTypeService;

        /// <summary>
        /// Receives the DocumentTypeService via DI.
        /// </summary>
        public DocumentTypeController(DocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        /// <summary>
        /// Creates a new DocumentType.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult<DocumentType>>> PostDocumentAsync([FromBody] DocumentType documentType)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _documentTypeService.CreateDocumentTypeAsync(userIdClaimed.Value, documentType);

            return Ok(postResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<DocumentType>>> GetDocumentTypesAsync([FromQuery] int? docTypeId,
            [FromQuery] string name, [FromQuery] string description)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var retrievalResult = await _documentTypeService.RetrieveDocumentTypeAsync(userIdClaimed.Value, docTypeId,
                name, description);

            return Ok(retrievalResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult<DocumentType>>> PatchDocumentTypeAsync(int id,
            [FromBody] JsonPatchDocument<DocumentType> documentTypePatch)
        {
            if (documentTypePatch is null)
                return BadRequest();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            // create empty document ..
            var patchedDocumentType = new DocumentType();

            // .. and apply patch on it
            documentTypePatch.ApplyTo(patchedDocumentType);

            if (!TryValidateModel(patchedDocumentType))
                return BadRequest("Patch syntax invalid");

            var patchResult = await _documentTypeService.UpdateDocumentTypeAsync(userIdClaimed.Value, id, patchedDocumentType);

            if (patchResult is null)
                return BadRequest("Tried to update non updateable attributes");

            return Ok(patchResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult<DocumentType>>> DeleteDocumentTypeAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _documentTypeService.DeleteDocumentTypeAsync(userIdClaimed.Value, id));
        }
    }
}
