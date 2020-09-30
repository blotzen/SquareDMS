using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    public class RightDispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public RightDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> CreateRightAsync(int userId, Right right)
        {
            return await _squareDb.CreateRightAsync(userId, right);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Right>> RetrieveRightsAsync(int userId, int? groupId, int? docId)
        {
            return await _squareDb.RetrieveRightsAsync(userId, groupId, docId);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> UpdateRightAsync(int userId, int groupId, int docId,
            Right patchedRight)
        {
            return await _squareDb.UpdateRightAsync(userId, groupId, docId, patchedRight.AccessLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> DeleteRightAsync(int userId, int groupId, int docId)
        {
            return await _squareDb.DeleteRightsAsync(userId, groupId, docId);
        }
        #endregion
    }
}
