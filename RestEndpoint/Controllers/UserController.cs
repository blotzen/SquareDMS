using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using NLog;
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
        private readonly Logger _logger;

        /// <summary>
        /// Receives the UserService via Dependency Injection.
        /// </summary>
        public UserController(UserService userService)
        {
            _userService = userService;
            _logger = LogManager.GetLogger("UserController");
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
            _logger.Info("Login request from User: {0}", authenticateRequest.UserName);

            var authenticationResponse = await _userService.Authenticate(authenticateRequest);

            if (authenticationResponse is null)
            {
                _logger.Info("Username or password is incorrect or user is inactive.");
                return BadRequest("Username or password is incorrect or user is inactive.");
            }

            _logger.Info("User {0} successfully logged in", authenticateRequest.UserName);

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// Creates a new User in the DMS.
        /// </summary>
        /// <returns>Manipulationresult</returns>
        [HttpPost]
        public async Task<ActionResult<ManipulationResult<User>>> PostUserAsync([FromBody] User user)
        {
            _logger.Info("Startet creating a new User");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var postResult = await _userService.CreateUserAsync(userIdClaimed.Value, user);

            if (postResult is null)
            {
                _logger.Info("Cant create user, because username and/or password guidelines are not fulfilled");
                return BadRequest("Cant create user, because username and/or password guidelines are not fulfilled.");
            }

            _logger.Info("New user ({0}) created with ErrorCode {1}", user.UserName, postResult.ErrorCode);

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
            _logger.Info("Startet retrieving Users");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var retrievalResult = await _userService.RetrieveUserAsync(userIdClaimed.Value, userId, lastName,
                firstName, userName, email, active);

            _logger.Info("Users retrieved with ErrorCode {0}", retrievalResult.ErrorCode);

            return Ok(retrievalResult);
        }

        /// <summary>
        /// Patches (Updates) a user.
        /// </summary>
        /// <param name="id">Id of the user to be updated</param>
        /// <param name="userPatch">JSON Patch body</param>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ManipulationResult<User>>> PatchUserAsync(int id,
            [FromBody] JsonPatchDocument<User> userPatch)
        {
            _logger.Info("Startet updating a User");

            if (userPatch is null)
                return BadRequest("Patch body is empty");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            // create empty user ..
            var patchedUser = new User();

            // .. and apply patch on it
            userPatch.ApplyTo(patchedUser);

            if (!TryValidateModel(patchedUser))
            {
                _logger.Info("Patch syntax invalid");
                return BadRequest("Patch syntax invalid.");
            }

            var patchResult = await _userService.UpdateUserAsync(userIdClaimed.Value, id, patchedUser);

            if (patchResult is null)
            {
                _logger.Info("Tried to update non updateable attributes or userId is invalid.");
                return BadRequest("Tried to update non updateable attributesor userId is invalid.");
            }

            _logger.Info("User updated with ErrorCode {0}", patchResult.ErrorCode);

            return Ok(patchResult);
        }

        /// <summary>
        /// Deletes a user with the given id, if 
        /// the executing user has permissions.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ManipulationResult<User>>> DeleteUserAsync(int id)
        {
            _logger.Info("Startet deleting a User");

            var userIdClaimed = HttpContext.User.Identity.GetUserIdClaim();

            if (userIdClaimed is null)
            {
                _logger.Info("No userId has been found in the claims");
                return BadRequest("Incorrect JWT. No userId Claim found.");
            }

            var deletionResult = await _userService.DeleteUserAsync(userIdClaimed.Value, id);

            _logger.Info("User deleted with ErrorCode {0}", deletionResult.ErrorCode);

            return Ok(deletionResult);
        }
    }
}
