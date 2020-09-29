using SquareDMS.RestEndpoint.Authentication;
using Xunit;

namespace System_Test
{
    [Collection("Sequential")]
    public class AdminTests : SystemTest
    {
        private readonly Request _adminValidLoginRequest = new Request
        {
            UserName = "admin",
            Password = "admin"
        };

        /// <summary>
        /// Admin logs in with valid credentials.
        /// </summary>
        [Fact]
        public async void Login_valid()
        {
            var response = await GetLoginResponseAsync(_adminValidLoginRequest);

            Assert.Equal(1, response?.Id);
            Assert.NotNull(response.Token);
            Assert.Equal("admin", response.UserName);
        }
    }
}
