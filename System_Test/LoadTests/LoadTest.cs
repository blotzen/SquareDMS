using SquareDMS.DataLibrary.Entities;
using SquareDMS.RestEndpoint.Authentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace SquareDMS.System_Test.WorkflowTests
{
    /// <summary>
    /// Load Tests for SquareDMS
    /// </summary>
    [Collection("Sequential")]
    public class LoadTest : TestHttpClient, IClassFixture<ResetDbFixture>
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
            Password = "sehrnice123",
            Active = true
        };

        /// <param name="clearDbFixture">Class wide fixture</param>
        public LoadTest(ResetDbFixture clearDbFixture)
        {
            _clearDbFixture = clearDbFixture;
        }

        /// <summary>
        /// Measures the time needed for creating 100 users in parallel.
        /// </summary>
        [Fact]
        public async void CreateNewUsers_valid()
        {
            string adminJwt = await LoginAdmin(_adminValidLoginRequest);

            var tasks = new List<Task>();

            // set the jwt once for the complete usage of the httpClient
            TestClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", adminJwt);

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(await Task.Factory.StartNew(async () =>
                 {
                     await CreateNewCreator(new User()
                     {
                         LastName = "Hauser",
                         FirstName = "Günter",
                         UserName = $"user_{i}",
                         Password = "sehrnice123",
                         Active = true
                     });
                 }));
            }

            Task.WaitAll(tasks.ToArray());

            sw.Stop();

            Assert.True(sw.ElapsedMilliseconds < 10_000);
        }

        private async Task<string> LoginAdmin(Request adminLoginRequest)
        {
            var loginResponseAdmin = await PostLoginAsync("api/v1/users/login", adminLoginRequest);
            var loginBodyAdmin = loginResponseAdmin.Item2;

            return loginBodyAdmin.Token;
        }

        private async Task CreateNewCreator(User user)
        {
            // create user
            var createUserResponse = await PostAsync<User>("api/v1/users", user);
            var createUserPostResult = createUserResponse.Item2;

            // put new user in the creators group
            await PostAsync<GroupMember>("api/v1/groupmembers",
                new GroupMember(3, createUserPostResult.ManipulatedEntity.Id));
        }
    }
}
