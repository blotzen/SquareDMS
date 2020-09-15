using System.Linq;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupTests
{
    [Collection("Sequential")]
    public class TestGroupRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin gets all Groups.
        /// </summary>
        [Fact]
        public async void Admin_GetAllGroups()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupAsync(1);

            Assert.Equal(4, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets all Admingroups.
        /// </summary>
        [Fact]
        public async void Admin_GetAllAdminGroups()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupAsync(1, admin: true);

            Assert.Single(readResult.Resultset);
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// User tries to get all groups.
        /// </summary>
        [Fact]
        public async void User_GetAllGroups()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupAsync(2, admin: true);

            Assert.Empty(readResult.Resultset);
            Assert.Equal(10, readResult.ErrorCode);
        }
    }
}
