using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Services
{
    public class GroupService
    {
        private readonly GroupDispatcher _groupDispatcher;

        /// <summary>
        /// Receives the dispatcher via DI.
        /// </summary>
        public GroupService(GroupDispatcher groupDispatcher)
        {
            _groupDispatcher = groupDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateGroupAsync(int userId, Group group)
        {
            return await _groupDispatcher.CreateGroupAsync(userId, group);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Group>> RetrieveGroupAsync(int userId, int? groupId,
            string name, string description, bool? admin, bool? creator)
        {
            return await _groupDispatcher.RetrieveGroupAsync(userId, groupId, name, description, admin, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateGroupAsync(int userId, int id, Group patchedGroup)
        {
            if (patchedGroup.Id is null)
            {
                return await _groupDispatcher.UpdateGroupAsync(userId, id, patchedGroup);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteGroupAsync(int userId, int id)
        {
            return await _groupDispatcher.DeleteGroupAsync(userId, id);
        }
        #endregion
    }
}