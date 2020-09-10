using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.ProcedureResults;
using System.Threading.Tasks;
using Models = SquareDMS.Core.Models;

namespace SquareDMS.Core.Dispatchers
{
    public class UserDispatcher : Dispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileForamtId"></param>
        /// <param name="patchedFileFormat"></param>
        /// <returns></returns>
        public async Task<ManipulationResult> PatchUserAsync(int userId, int updateUserId, Models::User patchedUser)
        {
            ISquareDb squareDb = new SquareDbMsSql(DbConnectionString);

            return await squareDb.UpdateUserAsync(userId, updateUserId,
                patchedUser.LastName, patchedUser.FirstName, patchedUser.UserName,
                patchedUser.Email, patchedUser.PasswordHash, patchedUser.Active);
        }
    }
}
