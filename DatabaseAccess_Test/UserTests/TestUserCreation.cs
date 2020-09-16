using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using System.Linq;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.UserTests
{
    [Collection("Sequential")]
    public class TestUserCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        private readonly byte[] passwordHash = new byte[32]
        { 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2 };

        /// <param name="fixture">Class wide fixture</param>
        public TestUserCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a new User.
        /// </summary>
        [Fact]
        public async void Admin_CreateUser()
        {

            var user = new User("Hans", "Kraus", "ludwigthoma", null, passwordHash, true);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// Admin creates a new User with an empty firstname. Function
        /// takes care of lowering the username.
        /// </summary>
        [Fact]
        public async void Admin_CreateUser_EmptyFirstName()
        {
            var user = new User("Remi", null, "Gaillard", "rg@fun.fr", passwordHash, false);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// Admin creates a new User with an empty Lastname.
        /// </summary>
        [Fact]
        public async void Admin_CreateUser_InvalidLastName()
        {

            var user = new User(null, "", "superUser", null, passwordHash, false);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(123, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// Admin creates a new User with an empty firstname. Function
        /// takes care of lowering the username.
        /// </summary>
        [Fact]
        public async void Admin_CreateUser_InvalidUserName()
        {

            var user = new User("Remi", null, null, null, passwordHash, false);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(124, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a duplicate user.
        /// </summary>
        [Fact]
        public async void Admin_CreateUserDuplicateUser()
        {

            var user = new User("Brandl", null, "blotzen", null, passwordHash, false);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(126, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a user with empty password.
        /// </summary>
        [Fact]
        public async void Admin_CreateUser_EmptyPassword()
        {

            var user = new User("Jolie", null, "mrssmith", null, null, false);

            var creationResult = await _squareDbMsSql.CreateUserAsync(1, user);

            Assert.Equal(125, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }

        /// <summary>
        /// User tries to create a user.
        /// </summary>
        [Fact]
        public async void User_CreateUser()
        {

            var user = new User("Jameson", "Jenna", "jenna", null, passwordHash, true);

            var creationResult = await _squareDbMsSql.CreateUserAsync(2, user);

            Assert.Equal(10, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(User), OperationType.Create));
        }
    }
}