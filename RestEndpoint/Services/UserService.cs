using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SquareDMS.Core;
using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Services
{
    /// <summary>
    /// This service provides the functionality to 
    /// the usersContoller e.g. generating the JWT 
    /// and calling the necessary methods to hash 
    /// the password.
    /// </summary>
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserDispatcher _userDispatcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
            _userDispatcher = new UserDispatcher();
        }

        #region Authentication
        /// <summary>
        /// Authenticates the user and creates the auth. response.
        /// Returns null if authentication failed or user is not active.
        /// </summary>
        public async Task<Authentication.Response> Authenticate(Authentication.Request request)
        {
            Credential userCredential = CreateCredential(request.UserName, request.Password);

            var user = await _userDispatcher.CheckUserCredentialAsync(userCredential);

            // authentication failed if CheckUserCredentialAsync
            // didnt find the user or the pws dont match OR if the
            // user is not active.
            if (user is null || !user.Active.GetValueOrDefault())
                return null;

            var jwt = GenerateJwt(user.Id);

            return new Authentication.Response(user, jwt);
        }

        /// <summary>
        /// Generates a JwtToken for the given userId.
        /// </summary>
        private string GenerateJwt(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region CRUD-Operations
        /// <summary>
        /// Creates the user. Before doing that the password hash is computed and
        /// the plaintext password is deleted.
        /// </summary>
        public async Task<ManipulationResult> CreateUserAsync(int id, User user)
        {
            var userCredential = CreateCredential(user.UserName, user.Password);

            // set the password hash and clear the stored pw
            user.PasswordHash = userCredential.HashPassword();
            user.Password = string.Empty;

            return await _userDispatcher.PostUserAsync(id, user);
        }

        /// <summary>
        /// Passes the get user command with the given params to the UserDispatcher.
        /// </summary>
        public async Task<RetrievalResult<User>> RetrieveUserAsync(int id, int? userId = null,
            string lastName = null, string firstName = null,
            string userName = null, string email = null,
            bool? active = null)
        {
            return await _userDispatcher.GetUsersAsync(id, userId, lastName, firstName, userName,
                email, active);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        public async Task<ManipulationResult> UpdateUserAsync(int id, User patchedUser)
        {
            return await _userDispatcher.PatchUserAsync(id, patchedUser.Id, patchedUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ManipulationResult> DeleteUserAsync(int id, int deleteUserId)
        {
            return await _userDispatcher.DeleteUserAsync(id, deleteUserId);
        }
        #endregion

        #region Credential-Operations
        private Credential CreateCredential(string userName, string password)
        {
            try
            {
                return new Credential(userName, password,
                    int.Parse(_configuration["Pbkdf2:Iterations"]));
            }
            catch (ArgumentException argEx)
            {
                var innerEx = argEx.InnerException;
                // TODO: Log here
                return null;
            }
        }
        #endregion
    }
}
