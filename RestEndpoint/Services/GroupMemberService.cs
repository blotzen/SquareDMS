using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Services
{
    public class GroupMemberService
    {
        private readonly GroupMemberDispatcher _groupMemberDispatcher;

        /// <summary>
        /// 
        /// </summary>
        public GroupMemberService(GroupMemberDispatcher groupMemberDispatcher)
        {
            _groupMemberDispatcher = groupMemberDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateGroupMemberAsync(int userId, GroupMember groupMember)
        {
            return await _groupMemberDispatcher.CreateGroupMemberAsync(userId, groupMember);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<GroupMember>> RetrieveGroupMemberAsync(int userId, int? groupId, int? memberId)
        {
            return await _groupMemberDispatcher.RetrieveRightsAsync(userId, groupId, memberId);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteGroupMemberAsync(int userId, int groupId, int memberId)
        {
            return await _groupMemberDispatcher.DeleteGroupMemberAsync(userId, groupId, memberId);
        }
        #endregion
    }
}
