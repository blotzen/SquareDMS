using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SquareDMS.Core.Dispatchers;
using SquareDMS.Core;

namespace SquareDMS.RestEndpoint.Services
{
    /// <summary>
    /// This service provides the functionality to 
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

        /// <summary>
        /// Authenticates the user and creates the auth. response.
        /// Returns null if authentication failed
        /// </summary>
        public async Task<Authentication.Response> Authenticate(Authentication.Request request)
        {
            Credential userCredential = null;

            try
            {
                userCredential = new Credential(request.UserName,
                    request.Password,
                    int.Parse(_configuration["Pbkdf2:Iterations"]));
            }
            catch (ArgumentException argEx)
            {
                var innerEx = argEx.InnerException;
                // TODO: Log here
                return null;
            }

            var user = await _userDispatcher.CheckUserInformationAsync(userCredential);

            // authentication failed
            if (user is null)
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
    }
}
