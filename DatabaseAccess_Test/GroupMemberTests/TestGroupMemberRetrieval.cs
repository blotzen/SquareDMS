using SquareDMS.DatabaseAccess;
using System.Linq;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupMemberTests
{
    [Collection("Sequential")]
    public class TestGroupMemberRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupMemberRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin gets all GroupMembers.
        /// </summary>
        [Fact]
        public async void Admin_GetAllGroupMembers()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupMemberAsync(1);

            Assert.Equal(5, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets group members with userId 2
        /// </summary>
        [Fact]
        public async void Admin_GetGroupMembers_Id2()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupMemberAsync(1, memberId: 2);

            Assert.Equal(2, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// User gets all group members
        /// </summary>
        [Fact]
        public async void User_GetAllGroupMembers()
        {
            var readResult = await _squareDbMsSql.RetrieveGroupMemberAsync(2);

            Assert.Equal(5, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }
    }
}
