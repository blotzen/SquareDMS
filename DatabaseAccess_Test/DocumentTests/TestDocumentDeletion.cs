using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.DocumentTests
{
    [Collection("Sequential")]
    public class TestDocumentDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Deletes a document as Admin. Also deletes the depending right 
        /// and doc version.
        /// </summary>
        [Fact]
        public async void Admin_DeleteDocAndRight()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentAsync(1, 6);

            Assert.Equal(0, deletionResult.ErrorCode);
            Assert.Equal(1, deletionResult.ManipulatedAmount(typeof(Right), OperationType.Delete));
            Assert.Equal(2, deletionResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Delete));
        }

        /// <summary>
        /// Try to delete a document as User.
        /// </summary>
        [Fact]
        public async void User_Delete()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentAsync(2, 7);

            Assert.Equal(10, deletionResult.ErrorCode);
        }

        /// <summary>
        /// Try to delete a locked doc as Admin.
        /// </summary>
        [Fact]
        public async void Admin_Delete_LockedDoc()
        {
            var deletionResult = await _squareDbMsSql.DeleteDocumentAsync(1, 8);

            Assert.Equal(11, deletionResult.ErrorCode);
        }
    }
}