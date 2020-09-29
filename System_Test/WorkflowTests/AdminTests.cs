using SquareDMS.DataLibrary.Entities;
using SquareDMS.RestEndpoint.Authentication;
using System.Net;
using Xunit;

namespace SquareDMS.System_Test.WorkflowTests
{
    [Collection("Sequential")]
    public class AdminTests : SystemTest, IClassFixture<ResetDbFixture>
    {
        private readonly ResetDbFixture _clearDbFixture;

        /// <summary>
        /// Credentials
        /// </summary>
        private readonly Request _adminValidLoginRequest = new Request
        {
            UserName = "admin",
            Password = "admin"
        };

        /// <summary>
        /// User to be created
        /// </summary>
        private readonly User _user = new User
        {
            LastName = "Hauser",
            FirstName = "Günter",
            UserName = "bgboss",
            Password = "sehrnice123"
        };


        /// <param name="clearDbFixture">Class wide fixture</param>
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
            var loginResponse = await PostLoginAsync("api/v1/users/login", _adminValidLoginRequest);
            var loginStatusCode = loginResponse.Item1;
            var loginBody = loginResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, loginStatusCode);
            Assert.Equal(1, loginBody?.Id);
            Assert.NotNull(loginBody.Token);
            Assert.Equal("admin", loginBody.UserName);
        }

        /// <summary>
        /// Admin creates a valid User.
        /// </summary>
        [Fact]
        public async void CreateNewUser_valid()
        {
            // login admin
            var loginResponse = await PostLoginAsync("api/v1/users/login", _adminValidLoginRequest);
            var loginBody = loginResponse.Item2;

            // create user
            var response = await PostCRUDAsync("api/v1/users", _user, loginBody.Token);
            var statusCode = response.Item1;
            var postResult = response.Item2;

            Assert.Equal(HttpStatusCode.OK, statusCode);
            Assert.Equal(0, postResult.ErrorCode);
            Assert.Equal(1, postResult.ManipulatedAmount(typeof(User), DataLibrary.OperationType.Create));

            
            // put new user in the users group
            
            // need user id...........

        }
    }
}
