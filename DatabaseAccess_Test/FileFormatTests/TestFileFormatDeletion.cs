using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.FileFormatTests
{
    [Collection("Sequential")]
    public class TestFileFormatDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestFileFormatDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin deletes a file format
        /// </summary>
        [Fact]
        public async void Admin_DeleteFileFormat()
        {
            var deletionResult = await _squareDbMsSql.DeleteFileFormatAsync(1, 2);

            Assert.Equal(0, deletionResult.ErrorCode);
            Assert.Equal(1, deletionResult.ManipulatedAmount(typeof(FileFormat), OperationType.Delete));
        }

        /// <summary>
        /// User try to delete a file format
        /// </summary>
        [Fact]
        public async void User_DeleteFileFormat()
        {
            var deletionResult = await _squareDbMsSql.DeleteFileFormatAsync(2, 2);

            Assert.Equal(10, deletionResult.ErrorCode);
        }

        /// <summary>
        /// Admin tries to delete file format in use
        /// </summary>
        [Fact]
        public async void Admin_DeleteFileFormat_InUse()
        {
            var deletionResult = await _squareDbMsSql.DeleteFileFormatAsync(1, 1);

            Assert.Equal(106, deletionResult.ErrorCode);
        }
    }
}
