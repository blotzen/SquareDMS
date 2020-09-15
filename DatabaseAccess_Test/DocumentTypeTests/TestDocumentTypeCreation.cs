using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.DocumentTypeTest
{
    [Collection("Sequential")]
    public class TestDocumentTypeCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentTypeCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a new document type.
        /// </summary>
        [Fact]
        public async void Admin_Create_DocumentType()
        {
            var docType = new DocumentType("Mitschrift", "Die Mitschrift eines Kundengesprächs");

            var creationResult = await _squareDbMsSql.CreateDocumentTypeAsync(1, docType);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try create as admin with invalid space name.
        /// </summary>
        [Fact]
        public async void Admin_Create_DocumentType_invalidSpaceName()
        {
            var docType = new DocumentType(" ", "Die Mitschrift eines Kundengesprächs");

            var creationResult = await _squareDbMsSql.CreateDocumentTypeAsync(1, docType);

            Assert.Equal(119, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try create as User.
        /// </summary>
        [Fact]
        public async void User_Create_DocumentType()
        {
            var docType = new DocumentType("Zusage", "Die Zusage für ein Meeting.");

            var creationResult = await _squareDbMsSql.CreateDocumentTypeAsync(2, docType);

            Assert.Equal(10, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try create already existing doctype.
        /// </summary>
        [Fact]
        public async void Admin_Create_Duplicate_DocumentType()
        {
            var docType = new DocumentType("E-Book", null);

            var creationResult = await _squareDbMsSql.CreateDocumentTypeAsync(1, docType);

            Assert.Equal(109, creationResult.ErrorCode);
        }
    }
}
