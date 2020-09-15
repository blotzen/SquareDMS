using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupTests
{
    [Collection("Sequential")]
    public class TestGroupDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin deletes Group.
        /// </summary>
        [Fact]
        public async void Admin_DeleteGroup()
        {
            var deletionResult = await _squareDbMsSql.DeleteGroupAsync(1, 4);

            Assert.Equal(0, deletionResult.ErrorCode);
            Assert.Equal(2, deletionResult.ManipulatedAmount(typeof(Right), OperationType.Delete));
        }

        /// <summary>
        /// Admin tries to delete non empty Group.
        /// </summary>
        [Fact]
        public async void Admin_Delete_NotEmptyGroup()
        {
            var deletionResult = await _squareDbMsSql.DeleteGroupAsync(1, 1);

            Assert.Equal(105, deletionResult.ErrorCode);
        }

        /// <summary>
        /// User tries to delete Group.
        /// </summary>
        [Fact]
        public async void User_DeleteGroup()
        {
            var deletionResult = await _squareDbMsSql.DeleteGroupAsync(2, 4);

            Assert.Equal(10, deletionResult.ErrorCode);
        }
    }
}
