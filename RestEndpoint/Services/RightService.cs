using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ManipulationResult> CreateRightAsync(int userId, Right right)
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
        public async Task<ManipulationResult> UpdateRightAsync(int userId, int groupId, int docId, 
            Right patchedRight)
        {
            return await _rightDispatcher.UpdateRightAsync(userId, groupId, docId, patchedRight);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteRightAsync(int userId, int groupId, int docId)
        {
            return await _rightDispatcher.DeleteRightAsync(userId, groupId, docId);
        }
        #endregion
    }
}
