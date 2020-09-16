using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.GroupMemberTests
{
    [Collection("Sequential")]
    public class TestGroupMemberCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestGroupMemberCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin creates a Group Member.
        /// </summary>
        [Fact]
        public async void Admin_CreateGroupMember()
        {
            var groupMember = new GroupMember(4, 2);  // quentin in filmliebhaber

            var creationResult = await _squareDbMsSql.CreateGroupMemberAsync(1, groupMember);

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create existing group Member
        /// </summary>
        [Fact]
        public async void Admin_CreateGroupMember_AlreadyExists()
        {
            var groupMember = new GroupMember(2, 1);

            var creationResult = await _squareDbMsSql.CreateGroupMemberAsync(1, groupMember);

            Assert.Equal(117, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create with non existing groupId
        /// </summary>
        [Fact]
        public async void Admin_CreateGroupMember_NotExistingGroup()
        {
            var groupMember = new GroupMember(4000, 2);

            var creationResult = await _squareDbMsSql.CreateGroupMemberAsync(1, groupMember);

            Assert.Equal(113, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create with non existing memberId
        /// </summary>
        [Fact]
        public async void Admin_CreateGroupMember_NotExistingMember()
        {
            var groupMember = new GroupMember(4, 2000);

            var creationResult = await _squareDbMsSql.CreateGroupMemberAsync(1, groupMember);

            Assert.Equal(116, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Create));
        }

        /// <summary>
        /// User tries to create.
        /// </summary>
        [Fact]
        public async void User_CreateGroupMember()
        {
            var groupMember = new GroupMember(4, 2);

            var creationResult = await _squareDbMsSql.CreateGroupMemberAsync(2, groupMember);

            Assert.Equal(10, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(GroupMember), OperationType.Create));
        }
    }
}