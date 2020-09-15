using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.DocumentTypeTest
{
    [Collection("Sequential")]
    public class TestDocumentTypeDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentTypeDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Deletes a document type as Admin.
        /// and doc version.
        /// </summary>
        [Fact]
        public async void Admin_DeleteDocType_Async()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentTypeAsync(1, 3);

            Assert.Equal(0, deletionResult.ErrorCode);
        }

        /// <summary>
        /// Try to delete a doc type currently used doc type.
        /// and doc version.
        /// </summary>
        [Fact]
        public async void Admin_DeleteDocType_In_Use_Async()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentTypeAsync(1, 1);

            Assert.Equal(108, deletionResult.ErrorCode);
        }

        /// <summary>
        /// Try to delete a doc type as user
        /// and doc version.
        /// </summary>
        [Fact]
        public async void User_DeleteDocType_In_Use_Async()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentTypeAsync(2, 1);

            Assert.Equal(10, deletionResult.ErrorCode);
        }
    }
}
