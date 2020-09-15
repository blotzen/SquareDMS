using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.UserTests
{
    [Collection("Sequential")]
    public class TestUserDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestUserDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin deletes a user.
        /// </summary>
        [Fact]
        public async void Admin_DeletesUser()
        {
            var updationResult = await _squareDbMsSql.DeleteUserAsync(1, 4);

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Delete));
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(User), OperationType.Delete));
        }

        /// <summary>
        /// Admin tries to delete user that created documents.
        /// </summary>
        [Fact]
        public async void Admin_DeletesUser_with_Created_Doc()
        {
            var updationResult = await _squareDbMsSql.DeleteUserAsync(1, 2);

            Assert.Equal(127, updationResult.ErrorCode);
            Assert.Equal(0, updationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Delete));
            Assert.Equal(0, updationResult.ManipulatedAmount(typeof(User), OperationType.Delete));
        }

        /// <summary>
        /// User tries to delete user.
        /// </summary>
        [Fact]
        public async void User_DeletesUser()
        {
            var updationResult = await _squareDbMsSql.DeleteUserAsync(2, 4);

            Assert.Equal(10, updationResult.ErrorCode);
            Assert.Equal(0, updationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Delete));
            Assert.Equal(0, updationResult.ManipulatedAmount(typeof(User), OperationType.Delete));
        }
    }
}
