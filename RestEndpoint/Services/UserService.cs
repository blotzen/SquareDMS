using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core = SquareDMS.Core.Dispatchers;

namespace SquareDMS.RestEndpoint.Services
{
    /// <summary>
    /// This service provides the functionality to 
    /// </summary>
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private readonly Core::UserDispatcher _userDispatcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public UserService(IConfiguration configuration, Core::UserDispatcher userDispatcher)
        {
            _configuration = configuration;
            _userDispatcher = userDispatcher;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Authentication.Response> Authenticate(Authentication.Request request)
        {
            var userName = request.UserName;
            var password = request.Password;


            //var user = await _userDispatcher.


            //bool match = _userDispatcher.ComparePasswords(userName, password)

            // if match return response......

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a JwtToken for the given userId.
        /// </summary>
        private string GenerateJwtToken(int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt: SecretKey"]));
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
