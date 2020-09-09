using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupTests
{
    [Collection("Sequential")]
    public class TestGroupCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a Group.
        /// </summary>
        [Fact]
        public async void Admin_CreateGroup()
        {
            var group = new Group("Stammtisch",
                "DoHockaDeDachauWeisDoOiweSchoGhocktSan", false, false);

            var creationResult = await _squareDbMsSql.CreateGroupAsync(1, group);

            Assert.Equal(0, creationResult.ErrorCode);
        }

        /// <summary>
        /// User tries to create a Group.
        /// </summary>
        [Fact]
        public async void User_CreateGroup()
        {
            var group = new Group("Stammtisch",
                "DoHockaDeDachauWeisDoOiweSchoGhocktSan", false, false);

            var creationResult = await _squareDbMsSql.CreateGroupAsync(2, group);

            Assert.Equal(10, creationResult.ErrorCode);
        }

        /// <summary>
        /// Admin tries to creates a Group with empty name.
        /// </summary>
        [Fact]
        public async void Admin_CreateGroup_EmptyName()
        {
            var group = new Group("",
                "DoHockaDeDachauWeisDoOiweSchoGhocktSan", false, false);

            var creationResult = await _squareDbMsSql.CreateGroupAsync(1, group);

            Assert.Equal(122, creationResult.ErrorCode);
        }
    }
}
