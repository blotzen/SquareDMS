using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Services
{
    public class RightService
    {
        private readonly RightDispatcher _rightDispatcher;

        /// <summary>
        /// 
        /// </summary>
        public RightService(RightDispatcher rightDispatcher)
        {
            _rightDispatcher = rightDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> CreateRightAsync(int userId, Right right)
        {
            return await _rightDispatcher.CreateRightAsync(userId, right);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Right>> RetrieveRightAsync(int userId, int? groupId, int? docId)
        {
            return await _rightDispatcher.RetrieveRightsAsync(userId, groupId, docId);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> UpdateRightAsync(int userId, int groupId, int docId,
            Right patchedRight)
        {
            // checks if the patch is applied only to editable attributes
            if (patchedRight.DocumentId is null && patchedRight.GroupId is null)
            {
                return await _rightDispatcher.UpdateRightAsync(userId, groupId, docId, patchedRight);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Right>> DeleteRightAsync(int userId, int groupId, int docId)
        {
            return await _rightDispatcher.DeleteRightAsync(userId, groupId, docId);
        }
        #endregion
    }
}
