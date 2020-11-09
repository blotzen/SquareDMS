using Microsoft.Net.Http.Headers;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using SquareDMS.RestEndpoint.Authentication;
using System;
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
        /// Creates amountUsers new Users, uses the users to create documents and
        /// documentVersions and retrieves them in parallel.
        /// </summary>
        [Theory]
        [InlineData(50)]
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

            // create new file format
            var createFileFormat = await testHttpClient.PostAsync<FileFormat>("api/v1/fileformats",
                new FileFormat("PDF", "Portable Document Format"), loginBodyAdmin.Token);

            var createFileFormatPostResult = createFileFormat.Item2;

            var tasks = new List<Task>();
            var sw = new Stopwatch();
            sw.Start();

            // create http client for each user, create document and document version (Concurrent)
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
                    var createdDocumentPostResult = await userTestHttpClient.PostAsync<Document>("api/v1/documents",
                        new Document(createDocumentTypePostResult.ManipulatedEntity.Id.Value, "Der Duden"), loginBodyCreator.Token);

                    // new user creates new documentVersion
                    await userTestHttpClient.PostDocumentVersionAsync("api/v1/documentversions", new DocumentVersion()
                    {
                        FileFormatId = createFileFormatPostResult.ManipulatedEntity.Id.Value,
                        DocumentId = createdDocumentPostResult.Item2.ManipulatedEntity.Id.Value,
                        //RawFile = new byte[] { 23, 33, 11, 3, 212, 2, 4, 1 }
                        RawFile = new byte[500_000]
                    }, loginBodyCreator.Token);

                    var retrievedDocmentVersionMetadata = await userTestHttpClient.GetAsync<DocumentType>($"api/v1/documentversions?documentId={createdDocumentPostResult.Item2.ManipulatedEntity.Id.Value}", 
                        loginBodyCreator.Token);

                    await userTestHttpClient.GetDocumentVersionPayloadAsync($"api/v1/documentversions/" +
                        $"{retrievedDocmentVersionMetadata.Item2.Resultset.FirstOrDefault().Id.Value}/payload", loginBodyCreator.Token);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            sw.Stop();
            sw.Reset();

            // admin checks the documents
            var retrieveDocumentResponse = await testHttpClient.GetAsync<Document>(@"api/v1/documents", loginBodyAdmin.Token);
            var retrievedDocumentGetResult = retrieveDocumentResponse.Item2;

            bool successful = true;

            // amountUsers because each user creates one document and a document version
            if (retrievedDocumentGetResult.ErrorCode != 0 || retrievedDocumentGetResult.Resultset.Count() != amountUsers)
            {
                successful = false;
            }

            //GC.Collect();

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
