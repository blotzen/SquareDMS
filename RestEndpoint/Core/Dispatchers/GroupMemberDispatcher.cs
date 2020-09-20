using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;
namespace SquareDMS.Core.Dispatchers
{
    public class GroupMemberDispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public GroupMemberDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateGroupMemberAsync(int userId, GroupMember groupMember)
        {
            return await _squareDb.CreateGroupMemberAsync(userId, groupMember);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<GroupMember>> RetrieveRightsAsync(int userId, int? groupId, int? memberId)
        {
            return await _squareDb.RetrieveGroupMemberAsync(userId, groupId, memberId);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteGroupMemberAsync(int userId, int groupId, int memberId)
        {
            return await _squareDb.DeleteGroupMemberAsync(userId, groupId, memberId);
        }
        #endregion
    }
}
