using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using System.Linq;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.RightTests
{
    [Collection("Sequential")]
    public class TestRightCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestRightCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a new right with accessLevel 200.
        /// </summary>
        [Fact]
        public async void Admin_CreateRight()
        {
            var right = new Right(4, 9, AccessLevel.Update);

            var creationResult = await _squareDbMsSql.CreateRightAsync(1, right);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create a right wihtout being admin.
        /// </summary>
        [Fact]
        public async void User_CreateRight()
        {
            var right = new Right(4, 9, AccessLevel.Update);

            var creationResult = await _squareDbMsSql.CreateRightAsync(3, right);

            Assert.Equal(10, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create a right with a non existing group.
        /// </summary>
        [Fact]
        public async void Admin_CreateRight_GroupInvalid()
        {
            var right = new Right(9999, 9, AccessLevel.Update);

            var creationResult = await _squareDbMsSql.CreateRightAsync(1, right);

            Assert.Equal(113, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create a right with a non existing document.
        /// </summary>
        [Fact]
        public async void Admin_CreateRight_DocInvalid()
        {
            var right = new Right(4, 9999, AccessLevel.Update);

            var creationResult = await _squareDbMsSql.CreateRightAsync(1, right);

            Assert.Equal(114, creationResult.ErrorCode);
        }

        /// <summary>
        /// Try to create a right which already exists.
        /// </summary>
        [Fact]
        public async void Admin_CreateExistingRight()
        {
            var right = new Right(2, 6, AccessLevel.Update);

            var creationResult = await _squareDbMsSql.CreateRightAsync(1, right);

            Assert.Equal(111, creationResult.ErrorCode);
        }
    }
}
