using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.RightTests
{
    [Collection("Sequential")]
    public class TestRightUpdation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestRightUpdation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin update a right to accessLevel 200.
        /// </summary>
        [Fact]
        public async void Admin_UpdateRightToUpdateLevel()
        {
            var updationResult = await _squareDbMsSql.UpdateRightAsync(1, 4, 10, AccessLevel.Update);

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(Right), OperationType.Update));
        }

        /// <summary>
        /// User try to update a right to accessLevel 200.
        /// </summary>
        [Fact]
        public async void User_UpdateRightToUpdateLevel()
        {
            var updationResult = await _squareDbMsSql.UpdateRightAsync(2, 4, 10, AccessLevel.Update);

            Assert.Equal(10, updationResult.ErrorCode);
        }

        /// <summary>
        /// Admin try to update a not existing right to accessLevel 200.
        /// </summary>
        [Fact]
        public async void Admin_UpdateNotExistingRight()
        {
            var updationResult = await _squareDbMsSql.UpdateRightAsync(1, 4000, 10, AccessLevel.Update);

            Assert.Equal(115, updationResult.ErrorCode);
        }
    }
}
