using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupMemberTests
{
    [Collection("Sequential")]
    public class TestGroupMemberDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupMemberDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin deletes a Group Member
        /// </summary>
        [Fact]
        public async void Admin_DeleteGroupMember()
        {
            var deletionResult = await _squareDbMsSql.DeleteGroupMemberAsync(1, 1, 1);

            Assert.Equal(0, deletionResult.ErrorCode);
            Assert.Equal(1, deletionResult.ManipulatedAmount(typeof(GroupMember), OperationType.Delete));
        }

        /// <summary>
        /// User tries to delete a Group Member
        /// </summary>
        [Fact]
        public async void User_DeleteGroupMember()
        {
            var deletionResult = await _squareDbMsSql.DeleteGroupMemberAsync(2, 1, 1);

            Assert.Equal(10, deletionResult.ErrorCode);
            Assert.Equal(0, deletionResult.ManipulatedAmount(typeof(GroupMember), OperationType.Delete));
        }
    }
}
