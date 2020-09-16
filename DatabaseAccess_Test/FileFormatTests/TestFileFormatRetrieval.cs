using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using System.Linq;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.FileFormatTests
{
    [Collection("Sequential")]
    public class TestFileFormatRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestFileFormatRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Gets all the File Formats as admin.
        /// </summary>
        [Fact]
        public async void Admin_GetAllFileFormats()
        {
            var readResult = await _squareDbMsSql.RetrieveFileFormatsAsync(1);

            Assert.Equal(2, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Gets all the File Formats as user.
        /// </summary>
        [Fact]
        public async void User_GetAllFileFormats()
        {
            var readResult = await _squareDbMsSql.RetrieveFileFormatsAsync(2);

            Assert.Equal(2, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }
    }
}
