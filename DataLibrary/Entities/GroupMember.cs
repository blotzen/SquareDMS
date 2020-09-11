namespace SquareDMS.DataLibrary.Entities
{
    /// <summary>
    /// Users are members in groups. This entity links them.
    /// </summary>
    public class GroupMember : IDataTransferObject
    {
        /// <summary>
        /// Constructor for dapper
        /// </summary>
        public GroupMember() { }

        /// <summary>
        /// Creates a new GroupMember
        /// </summary>
        public GroupMember(int groupId, int userId)
        {
            GroupId = groupId;
            UserId = userId;
        }

        public int GroupId { get; private set; }

        public int UserId { get; private set; }
    }
}