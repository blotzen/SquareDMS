using SquareDMS.DataLibrary.Entities;
using SquareDMS.RestEndpoint.Authentication;
using System.Net;
using Xunit;

namespace SquareDMS.System_Test.WorkflowTests
{
    [Collection("Sequential")]
    public class AdminTests : IClassFixture<ResetDbFixture>
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
            var testHttpClient = new TestHttpClient();

            var loginResponse = await testHttpClient.PostLoginAsync(_adminValidLoginRequest);
            var loginStatusCode = loginResponse.Item1;
            var loginBody = loginResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, loginStatusCode);
            Assert.Equal(1, loginBody?.Id);
            Assert.NotNull(loginBody.Token);
            Assert.Equal("admin", loginBody.UserName);
        }

        /// <summary>
        /// Admin creates a valid User and puts him into the creators group. Creates a new document 
        /// as admin and gives the newly created user read rights on the document.
        /// </summary>
        [Fact]
        public async void CreateNewUser_valid()
        {
            var testHttpClient = new TestHttpClient();

            // login admin
            var loginResponseAdmin = await testHttpClient.PostLoginAsync(_adminValidLoginRequest);
            var loginBodyAdmin = loginResponseAdmin.Item2;

            // create user
            var createUserResponse = await testHttpClient.PostAsync<User>("api/v1/users", _user, loginBodyAdmin.Token);
            var createUserStatusCode = createUserResponse.Item1;
            var createUserPostResult = createUserResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, createUserStatusCode);
            Assert.Equal(0, createUserPostResult.ErrorCode);
            Assert.Equal(1, createUserPostResult.ManipulatedAmount(typeof(User), DataLibrary.OperationType.Create));
            Assert.NotNull(createUserPostResult.ManipulatedEntity.Id);
            //-----------------------------------------------------------

            // put new user in the creators group
            var createGroupMemberResponse = await testHttpClient.PostAsync<GroupMember>("api/v1/groupmembers",
                new GroupMember(3, createUserPostResult.ManipulatedEntity.Id),
                loginBodyAdmin.Token);

            var createGroupMemberStatusCode = createGroupMemberResponse.Item1;
            var createGroupMemberResult = createGroupMemberResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, createGroupMemberStatusCode);
            Assert.Equal(0, createGroupMemberResult.ErrorCode);
            Assert.Equal(1, createGroupMemberResult.ManipulatedAmount(typeof(GroupMember), DataLibrary.OperationType.Create));
            Assert.Equal(3, createGroupMemberResult.ManipulatedEntity.GroupId);
            //-----------------------------------------------------------

            // create new document type
            var createDocumentTypeResponse = await testHttpClient.PostAsync<DocumentType>("api/v1/documentTypes",
                new DocumentType("E-Book", null), loginBodyAdmin.Token);

            var createDocumentTypeStatusCode = createDocumentTypeResponse.Item1;
            var createDocumentTypePostResult = createDocumentTypeResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, createDocumentTypeStatusCode);
            Assert.Equal(0, createDocumentTypePostResult.ErrorCode);
            Assert.Equal(1, createDocumentTypePostResult.ManipulatedAmount(typeof(DocumentType), DataLibrary.OperationType.Create));
            Assert.NotNull(createDocumentTypePostResult.ManipulatedEntity.Id);
            //-----------------------------------------------------------

            // login new user
            var loginResponseCreator = await testHttpClient.PostLoginAsync(new Request()
            {
                UserName = _user.UserName,
                Password = _user.Password
            });
            var loginBodyCreator = loginResponseCreator.Item2;
            //-----------------------------------------------------------

            // new user creates new document
            var createDocumentResponse = await testHttpClient.PostAsync<Document>("api/v1/documents",
                new Document(createDocumentTypePostResult.ManipulatedEntity.Id.Value, "Der Duden"), loginBodyCreator.Token);

            var createDocumentStatusCode = createDocumentResponse.Item1;
            var createDocumentPostResult = createDocumentResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, createDocumentStatusCode);
            Assert.Equal(0, createDocumentPostResult.ErrorCode);
            Assert.Equal(1, createDocumentPostResult.ManipulatedAmount(typeof(Document), DataLibrary.OperationType.Create));
            Assert.NotNull(createDocumentPostResult.ManipulatedEntity.Id);
            //-----------------------------------------------------------

            // retrieve created document as created user
            var retrieveDocumentResponse = await testHttpClient.GetAsync<Document>(@$"api/v1/documents?docId={createDocumentPostResult.ManipulatedEntity.Id.Value}",
                loginBodyCreator.Token);

            var retrievedDocumentStatusCode = retrieveDocumentResponse.Item1;
            var retrievedDocumentGetResult = retrieveDocumentResponse.Item2;

            Assert.Equal(HttpStatusCode.OK, retrievedDocumentStatusCode);
            Assert.Equal(0, retrievedDocumentGetResult.ErrorCode);
            Assert.Single(retrievedDocumentGetResult.Resultset);
        }
    }
}
