using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.FileFormatTests
{
    [Collection("Sequential")]
    public class TestFileFormatUpdation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestFileFormatUpdation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin updates a FileFormat description
        /// </summary>
        [Fact]
        public async void Admin_UpdateFileFormat_Description()
        {
            var creationResult = await _squareDbMsSql.UpdateFileFormatAsync(1, 1, description: "Adobe PDF Format");

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Admin updates a FileFormat extension
        /// </summary>
        [Fact]
        public async void Admin_UpdateFileFormat_Extension()
        {
            var creationResult = await _squareDbMsSql.UpdateFileFormatAsync(1, 1, extension: "PDFS");

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(FileFormat), OperationType.Update));
        }

        /// <summary>
        /// User try update a FileFormat
        /// </summary>
        [Fact]
        public async void User_UpdateFileFormat()
        {
            var creationResult = await _squareDbMsSql.UpdateFileFormatAsync(2, 1, description: "Adobe PDF Format");

            Assert.Equal(10, creationResult.ErrorCode);
        }
    }
}
