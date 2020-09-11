﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.RestEndpoint.Services;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SquareDMS.RestEndpoint.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        /// <summary>
        /// Receives the UserService via Dependency Injection.
        /// </summary>
        public UsersController(UserService userService)
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
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] Authentication.Request authenticateRequest)
        {
            var authenticationResponse = await _userService.Authenticate(authenticateRequest);

            if (authenticationResponse is null)
            {
                return BadRequest("Username or password is incorrect");
            }

            return Ok(authenticationResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPatch"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchAsync(int id,
            [FromBody] JsonPatchDocument<User> userPatch)
        {
            if (userPatch is null)
                return BadRequest();

            // create empty user ..
            var patchedUser = new User();

            //HttpContext.Request.Headers[]

            // .. and apply patch on it
            userPatch.ApplyTo(patchedUser);

            if (!TryValidateModel(patchedUser))
                return BadRequest();

            // create dispatcher
            var userDispatcher = new UserDispatcher();
            await userDispatcher.PatchUserAsync(1, id, patchedUser);

            return NoContent();
        }


    }
}