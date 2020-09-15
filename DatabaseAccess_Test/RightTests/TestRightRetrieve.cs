using System.Linq;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.RightTests
{
    [Collection("Sequential")]
    public class TestRightRetrieve : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestRightRetrieve(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Gets all rights a Admin
        /// </summary>
        [Fact]
        public async void Admin_GetRights()
        {
            var readResult = await _squareDbMsSql.RetrieveRightsAsync(1);

            Assert.Equal(5, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Gets a single right as Admin.
        /// </summary>
        [Fact]
        public async void Admin_GetRight()
        {
            var readResult = await _squareDbMsSql.RetrieveRightsAsync(1, 2, 5);
            var readRight = readResult.Resultset.ToList()[0];

            Assert.Single(readResult.Resultset);

            Assert.Equal(2, readRight.GroupId);
            Assert.Equal(5, readRight.DocumentId);
            Assert.Equal(AccessLevel.Update, readRight.AccessLevel);

            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// User gets a right
        /// </summary>
        [Fact]
        public async void User_GetRight()
        {
            var readResult = await _squareDbMsSql.RetrieveRightsAsync(4, 2, 5);

            Assert.Single(readResult.Resultset);
            Assert.Equal(0, readResult.ErrorCode);
        }


    }
}
