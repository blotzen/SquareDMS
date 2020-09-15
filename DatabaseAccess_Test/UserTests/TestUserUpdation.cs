using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.UserTests
{
    [Collection("Sequential")]
    public class TestUserUpdation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestUserUpdation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin update a user.
        /// </summary>
        [Fact]
        public async void Admin_UpdateUser_Username()
        {
            var updationResult = await _squareDbMsSql.UpdateUserAsync(1, 2, userName: "Django");

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(User), OperationType.Update));
        }

        /// <summary>
        /// User tries to Update a user.
        /// </summary>
        [Fact]
        public async void User_UpdateOtherUser()
        {
            var updationResult = await _squareDbMsSql.UpdateUserAsync(2, 3, userName: "Django");

            Assert.Equal(15, updationResult.ErrorCode);
            Assert.Equal(0, updationResult.ManipulatedAmount(typeof(User), OperationType.Update));
        }

        /// <summary>
        /// User updates himself.
        /// </summary>
        [Fact]
        public async void User_UpdateHimself()
        {
            var updationResult = await _squareDbMsSql.UpdateUserAsync(2, 2, userName: "Django");

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(User), OperationType.Update));
        }
    }
}
