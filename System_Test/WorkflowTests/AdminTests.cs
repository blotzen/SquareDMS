using SquareDMS.RestEndpoint.Authentication;
using Xunit;

namespace SquareDMS.System_Test.WorkflowTests
{
    [Collection("Sequential")]
    public class AdminTests : SystemTest, IClassFixture<ResetDbFixture>
    {
        private readonly ResetDbFixture _clearDbFixture;

        private readonly Request _adminValidLoginRequest = new Request
        {
            UserName = "admin",
            Password = "admin"
        };

        public AdminTests(ResetDbFixture clearDbFixture)
        {
            _clearDbFixture = clearDbFixture;
        }

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
