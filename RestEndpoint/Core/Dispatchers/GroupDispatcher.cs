using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    public class GroupDispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public GroupDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operationen
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateGroupAsync(int userId, Group group)
        {
            return await _squareDb.CreateGroupAsync(userId, group);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Group>> RetrieveGroupAsync(int userId, int? groupId,
            string name, string description, bool? admin, bool? creator)
        {
            return await _squareDb.RetrieveGroupAsync(userId, groupId, name, description, admin, creator);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateGroupAsync(int userId, int id, Group patchedGroup)
        {
            if (patchedGroup.Id is null)
            {
                return await _squareDb.UpdateGroupAsync(userId, id, patchedGroup.Name, patchedGroup.Description,
                    patchedGroup.Admin, patchedGroup.Creator);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteGroupAsync(int userId, int id)
        {
            return await _squareDb.DeleteGroupAsync(userId, id);
        }
        #endregion
    }
}
