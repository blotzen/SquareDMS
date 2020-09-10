using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core = SquareDMS.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestEndpoint.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // TODO: Authentication

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userPatch"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchAsync(int id,
            [FromBody] JsonPatchDocument<Core::Models.User> userPatch)
        {
            if (userPatch is null)
                return BadRequest();

            // create empty user ..
            var patchedUser = new Core::Models.User();

            //HttpContext.Request.Headers[]

            // .. and apply patch on it
            userPatch.ApplyTo(patchedUser);

            if (!TryValidateModel(patchedUser))
                return BadRequest();

            // create dispatcher
            var userDispatcher = new Core::Dispatchers.UserDispatcher();
            await userDispatcher.PatchUserAsync(1, id, patchedUser);

            return NoContent();
        }


    }
}
