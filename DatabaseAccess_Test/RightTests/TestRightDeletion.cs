using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.RightTests
{
    [Collection("Sequential")]
    public class TestRightDeletion : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestRightDeletion(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Deletes a right as Admin with valid parameters.
        /// </summary>
        [Fact]
        public async void Admin_Delete_Right_Async()
        {
            var deletionResult = await _squareDbMsSql.DeleteRightsAsync(1, 4, 11);

            Assert.Equal(0, deletionResult.ErrorCode);
            Assert.Equal(1, deletionResult.ManipulatedAmount(typeof(Right), OperationType.Delete));
        }

        /// <summary>
        /// Try to delete a right as User with valid parameters.
        /// </summary>
        [Fact]
        public async void User_Delete_Right_Async()
        {
            var deletionResult = await _squareDbMsSql.DeleteRightsAsync(2, 4, 11);

            Assert.Equal(10, deletionResult.ErrorCode);
            Assert.Equal(0, deletionResult.ManipulatedAmount(typeof(Right), OperationType.Delete));
        }
    }
}
