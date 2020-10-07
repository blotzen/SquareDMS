using Microsoft.Net.Http.Headers;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint.Authentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SquareDMS.System_Test.WorkflowTests
{
    /// <summary>
    /// Load Tests for SquareDMS
    /// </summary>
    [Collection("Sequential")]
    public class LoadTest : IClassFixture<ResetDbFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly ResetDbFixture _clearDbFixture;

        /// <summary>
        /// Credentials
        /// </summary>
        private readonly Request _adminValidLoginRequest = new Request
        {
            UserName = "admin",
            Password = "admin"
        };

        /// <param name="clearDbFixture">Class wide fixture</param>
        public LoadTest(ResetDbFixture clearDbFixture)
        {
            _clearDbFixture = clearDbFixture;
        }

        /// <summary>
        /// Creates 5 new Users, uses the users to create documents and retrieve them in parallel.
        /// </summary>
        [Theory]
        [InlineData(25)]
        public async void CreateAndRetrieveDocumentsMultiUser(int amountUsers)
        {
            var testHttpClient = new TestHttpClient();

            // login admin
            var loginResponseAdmin = await testHttpClient.PostLoginAsync(_adminValidLoginRequest);
            var loginBodyAdmin = loginResponseAdmin.Item2;

            var users = new List<User>();

            for (int i = 0; i < amountUsers; i++)
            {
                var user = new User()
                {
                    LastName = "Neben",
                    FirstName = "Lauf",
                    UserName = $"user_{i}",
                    Password = "sehrnice123089",
                    Active = true
                };

                await CreateNewCreator(testHttpClient, user, loginBodyAdmin.Token);

                users.Add(user);
            }

            // create new document type
            var createDocumentTypeResponse = await testHttpClient.PostAsync<DocumentType>("api/v1/documentTypes",
                new DocumentType("E-Book", null), loginBodyAdmin.Token);

            var createDocumentTypePostResult = createDocumentTypeResponse.Item2;

            var tasks = new List<Task>();
            var sw = new Stopwatch();
            sw.Start();

            // create http client for each user, create document and retrieve the document (Concurrent)
            foreach (var user in users)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var userTestHttpClient = new TestHttpClient();

                    // login new user
                    var loginResponseCreator = await userTestHttpClient.PostLoginAsync(new Request()
                    {
                        UserName = user.UserName,
                        Password = user.Password
                    });
                    var loginBodyCreator = loginResponseCreator.Item2;

                    // new user creates new document
                    return await userTestHttpClient.PostAsync<Document>("api/v1/documents",
                        new Document(createDocumentTypePostResult.ManipulatedEntity.Id.Value, "Der Duden"), loginBodyCreator.Token);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            sw.Stop();

            // admin checks the documents
            var retrieveDocumentResponse = await testHttpClient.GetAsync<Document>(@"api/v1/documents", loginBodyAdmin.Token);
            var retrievedDocumentGetResult = retrieveDocumentResponse.Item2;

            bool successful = true;

            // amountUsers because each user creates one document
            if (retrievedDocumentGetResult.ErrorCode != 0 || retrievedDocumentGetResult.Resultset.Count() != amountUsers)
            {
                successful = false;
            }

            Assert.True(successful);
        }

        private async Task CreateNewCreator(TestHttpClient testHttpClient, User user, string jwt)
        {
            // create user
            var createUserResponse = await testHttpClient.PostAsync<User>("api/v1/users", user, jwt);
            var createUserPostResult = createUserResponse.Item2;

            // put new user in the creators group
            await testHttpClient.PostAsync<GroupMember>("api/v1/groupmembers",
                new GroupMember(3, createUserPostResult.ManipulatedEntity.Id), jwt);
        }
    }
}
