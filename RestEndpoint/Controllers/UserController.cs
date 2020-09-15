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
    /// Handles the HTTP-Operations for the 
    /// Ressource User.
    /// </summary>
    [Authorize]
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        /// <summary>
        /// Receives the UserService via Dependency Injection.
        /// </summary>
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Uses the UserService to authenticate the request.
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <returns>400 (BadRequest) if the authentication failed.
        /// 200 (OK) if the authentication was successful.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] Authentication.Request authenticateRequest)
        {
            if (authenticateRequest is null)
                return BadRequest("Auth Request body is empty");

            var authenticationResponse = await _userService.Authenticate(authenticateRequest);

            if (authenticationResponse is null)
            {
                return BadRequest("Username or password is incorrect or user is inactive.");
            }

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// Creates a new User in the DMS.
        /// </summary>
        /// <returns>Manipulationresult</returns>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult>> PostUserAsync([FromBody] User user)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            var postResult = await _userService.CreateUserAsync(userIdClaimed.Value, user);

            //if (postResult.ManipulatedAmount() == 1) { } // prüfung im Service machen und hier nur den Statuscode zurückgeben?

            return Ok(postResult);
        }

        /// <summary>
        /// Gets a User with the given params. Every parameter has to
        /// be supplied with query syntax (e.g. .../users/?UserId=7)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<RetrievalResult<User>>> GetUserAsync([FromQuery] int? userId = null,
            [FromQuery] string lastName = null, [FromQuery] string firstName = null,
            [FromQuery] string userName = null, [FromQuery] string email = null,
            [FromQuery] bool? active = null)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _userService.RetrieveUserAsync(userIdClaimed.Value, userId, lastName,
                firstName, userName, email, active));
        }

        /// <summary>
        /// Patches (Updates) a user.
        /// </summary>
        /// <param name="id">Id of the user to be updated</param>
        /// <param name="userPatch">JSON Patch body</param>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult>> PatchUserAsync(int id,
            [FromBody] JsonPatchDocument<User> userPatch)
        {
            if (userPatch is null)
                return BadRequest("Patch body is empty");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            // create empty user ..
            var patchedUser = new User();

            // .. and apply patch on it
            userPatch.ApplyTo(patchedUser);

            if (!TryValidateModel(patchedUser))
                return BadRequest("Patch syntax invalid");

            return Ok(await _userService.UpdateUserAsync(userIdClaimed.Value, patchedUser));
        }

        /// <summary>
        /// Deletes a user with the given id, if 
        /// the executing user has permissions.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult>> DeleteUserAsync(int id)
        {
            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
                return BadRequest();

            return Ok(await _userService.DeleteUserAsync(userIdClaimed.Value, id));
        }
    }
}
