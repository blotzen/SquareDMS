using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.FileFormatTests
{
    [Collection("Sequential")]
    public class TestFileFormatCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestFileFormatCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a new File Format. Removes Dot and 
        /// makes every char of the extension upper case.
        /// </summary>
        [Fact]
        public async void Admin_CreateFileFormat()
        {
            var fileFormat = new FileFormat(".docx", "Office Open XML");

            var creationResult = await _squareDbMsSql.CreateFileFormatAsync(1, fileFormat);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Admin try to create empty extension file format
        /// </summary>
        [Fact]
        public async void Admin_CreateFileFormat_Empty_Extension()
        {
            var fileFormat = new FileFormat("", "Office Open XML");

            var creationResult = await _squareDbMsSql.CreateFileFormatAsync(1, fileFormat);

            Assert.Equal(107, creationResult.ErrorCode);
        }

        /// <summary>
        /// User try to create file format
        /// </summary>
        [Fact]
        public async void User_CreateFileFormat()
        {
            var fileFormat = new FileFormat("XML", "Extensible Markup Language");

            var creationResult = await _squareDbMsSql.CreateFileFormatAsync(2, fileFormat);

            Assert.Equal(10, creationResult.ErrorCode);
        }
    }
}
