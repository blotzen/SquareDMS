using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.DocumentTests
{
    [Collection("Sequential")]
    public class TestDocumentCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Creates a document as Admin with valid parameters.
        /// </summary>
        [Fact]
        public async void Admin_ValidName_ValidDocType_Async()
        {
            var doc_valid = new Document(1, "Integrationstest_Dokument_1");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(1, doc_valid);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Creates a document as Creator with valid parameters.
        /// </summary>
        [Fact]
        public async void Creator_ValidName_ValidDocType_Async()
        {
            var doc_valid = new Document(1, "Integrationstest_Dokument_2");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(2, doc_valid);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create with invalid name (null)
        /// </summary>
        [Fact]
        public async void Creator_InvalidNameNull_ValidDocType_Async()
        {
            var doc_name_null = new Document(1, null);
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(1, doc_name_null);

            Assert.Equal(120, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create with invalid name (empty)
        /// </summary>
        [Fact]
        public async void Creator_InvalidNameEmpty_ValidDocType_Async()
        {
            var doc_name_empty = new Document(1, "");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(1, doc_name_empty);

            Assert.Equal(120, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create with not existing user
        /// </summary>
        [Fact]
        public async void UserNotExist_ValidName_ValidDocType_Async()
        {
            var doc_invalid_user = new Document(1, "Integrationstest_Dokument_3");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(123, doc_invalid_user);

            Assert.Equal(14, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create with insufficient rights (no admin and no creator)
        /// </summary>
        [Fact]
        public async void UserNotCreatorAndNotAdmin_ValidName_ValidDocType_Async()
        {
            var doc_invalid_user = new Document(1, "Integrationstest_Dokument_4");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(10, doc_invalid_user);

            Assert.Equal(14, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create with invalid docType
        /// </summary>
        [Fact]
        public async void Creator_ValidName_InvalidDocType_Async()
        {
            var doc_invalid_user = new Document(1023, "Integrationstest_Dokument_5");
            var creationResult = await _squareDbMsSql.CreateDocumentAsync(1, doc_invalid_user);

            Assert.Equal(110, creationResult.ErrorCode);
        }
    }
}