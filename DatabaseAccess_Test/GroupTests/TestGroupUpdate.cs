using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.GroupTests
{
    [Collection("Sequential")]
    public class TestGroupUpdate : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupUpdate(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin updates a FileFormat description
        /// </summary>
        [Fact]
        public async void Admin_UpdateGroup_Description()
        {
            var creationResult = await _squareDbMsSql.UpdateGroupAsync(1, 1, description: "Admins haben volle Rechte.");

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(Group), OperationType.Update));
        }

        /// <summary>
        /// Admin try update to empty name.
        /// </summary>
        [Fact]
        public async void Admin_UpdateGroup_EmptyName()
        {
            var creationResult = await _squareDbMsSql.UpdateGroupAsync(1, 1, name: "");

            Assert.Equal(122, creationResult.ErrorCode);
        }

        /// <summary>
        /// User tries to update.
        /// </summary>
        [Fact]
        public async void User_UpdateGroup_Description()
        {
            var creationResult = await _squareDbMsSql.UpdateGroupAsync(2, 1, description: "Admins haben volle Rechte.");

            Assert.Equal(10, creationResult.ErrorCode);
        }
    }
}
