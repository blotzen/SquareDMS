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
    /// for the Ressource Group.
    /// </summary>
    [Authorize]
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;

        /// <summary>
        /// Receives the GroupService via DI.
        /// </summary>
        public GroupController(GroupService groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Creates a new Group.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostGroupAsync([FromBody] Group group)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _groupService.CreateGroupAsync(userIdClaimed.Value, group);

            return Ok(postResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<Group>>> GetGroupAsync([FromQuery] int? groupId,
            [FromQuery] string name, [FromQuery] string description, [FromQuery] bool? admin, [FromQuery] bool? creator)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var retrievalResult = await _groupService.RetrieveGroupAsync(userIdClaimed.Value, groupId, name, description,
                admin, creator);

            return Ok(retrievalResult);
        }


        /// <summary>
        /// Partial Update of the given Group.
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult>> PatchGroupAsync(int id,
            [FromBody] JsonPatchDocument<Group> groupPatch)
        {
            if (groupPatch is null)
                return BadRequest();

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            // create empty document ..
            var patchedGroup = new Group();

            // .. and apply patch on it
            groupPatch.ApplyTo(patchedGroup);

            if (!TryValidateModel(patchedGroup))
                return BadRequest("Patch syntax invalid");

            var patchResult = await _groupService.UpdateGroupAsync(userIdClaimed.Value, id, patchedGroup);

            if (patchResult is null)
                return BadRequest("Tried to update non updateable attributes");

            return Ok(patchResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult>> DeleteGroupAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _groupService.DeleteGroupAsync(userIdClaimed.Value, id));
        }
    }
}
