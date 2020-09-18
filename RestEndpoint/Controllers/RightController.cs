using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    /// for the Ressource Rigth.
    /// </summary>
    [Authorize]
    [Route("api/v1/rights")]
    [ApiController]
    public class RightController : ControllerBase
    {
        private readonly RightService _rightService;

        /// <summary>
        /// Receives the RightService via DI.
        /// </summary>
        public RightController(RightService rightService)
        {
            _rightService = rightService;
        }

        /// <summary>
        /// Creates a new Right in the DMS.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostRightAsync([FromBody] Right right)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _rightService.CreateRightAsync(userIdClaimed.Value, right);

            return Ok(postResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<Right>>> GetRightsAsync(
            [FromQuery] int? groupId, [FromQuery] int? documentId)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _rightService.RetrieveRightAsync(userIdClaimed.Value, groupId, documentId));
        }

        /// <summary>
        /// Update the access level of a ressource
        /// </summary>
        [HttpPatch]
        public async Task<ActionResult<ManipulationResult>> PatchRightAsync([FromQuery] int? groupId,
            [FromQuery] int? documentId, [FromBody] JsonPatchDocument<Right> rightPatch)
        {
            if (rightPatch is null)
                return BadRequest();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            if (groupId is null || documentId is null)
                return BadRequest("groupId and docId have to be specified");

            // create empty right ..
            var patchedRight = new Right();

            // .. and apply patch on it
            rightPatch.ApplyTo(patchedRight);

            if (!TryValidateModel(patchedRight))
                return BadRequest("Patch syntax invalid");

            var patchResult = await _rightService.UpdateRightAsync(userIdClaimed.Value, groupId.Value,
                documentId.Value, patchedRight);

            // tried to update a non updateable attribute
            if (patchResult is null)
                return BadRequest("Tried to update non updateable attributes");

            return Ok(patchResult);
        }

        /// <summary>
        /// Delets a Right.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult<ManipulationResult>> DeleteFileFormatAsync([FromQuery] int? groupId,
            [FromQuery] int? documentId)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            if (groupId is null || documentId is null)
                return BadRequest("groupId and docId have to be specified");

            var deleteResult = await _rightService.DeleteRightAsync(userIdClaimed.Value,
                groupId.Value, documentId.Value);

            if (deleteResult.ErrorCode == 10)
            {
                return StatusCode(StatusCodes.Status403Forbidden, deleteResult);
            }

            return Ok();
        }
    }
}
