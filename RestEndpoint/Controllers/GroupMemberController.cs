using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.Services;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Controllers
{
    /// <summary>
    /// Handles the HTTP-Operations 
    /// for the Ressource GroupMember.
    /// </summary>
    [Authorize]
    [Route("api/v1/groupmembers")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly GroupMemberService _groupMemberService;

        /// <summary>
        /// Receives the GroupMemberService via DI.
        /// </summary>
        public GroupMemberController(GroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        /// <summary>
        /// Creates a new GroupMember in the DMS.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostGroupMemberAsync([FromBody] GroupMember groupMember)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _groupMemberService.CreateGroupMemberAsync(userIdClaimed.Value, groupMember);

            return Ok(postResult);
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<Right>>> GetGroupMembersAsync(
            [FromQuery] int? groupId, [FromQuery] int? memberId)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _groupMemberService.RetrieveGroupMemberAsync(userIdClaimed.Value, groupId, memberId));
        }

        /// <summary>
        /// Delets a GroupMember.
        /// </summary>
        [HttpDelete]
        public async Task<ActionResult<ManipulationResult>> DeleteGroupMemberAsync([FromQuery] int? groupId,
            [FromQuery] int? memberId)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            if (groupId is null || memberId is null)
                return BadRequest("groupId and memberId have to be specified");

            var deleteResult = await _groupMemberService.DeleteGroupMemberAsync(userIdClaimed.Value,
                groupId.Value, memberId.Value);

            if (deleteResult.ErrorCode == 10)
            {
                return StatusCode(StatusCodes.Status403Forbidden, deleteResult);
            }

            return Ok();
        }
    }
}
