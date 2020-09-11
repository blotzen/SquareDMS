using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    public class UserDispatcher : Dispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public UserDispatcher()
        {
            _squareDb = new SquareDbMsSql(DbConnectionString);
        }

        //public async Task<Models::User> GetUserByUserNameAsync(string userName)
        //{
        //    var userEntity = await _squareDb.RetrieveUserByUserNameAsync(userName);

        //    var userModel = new Models.User();
        //    userModel.Id
        //}

        //public bool ComparePasswordToHash(string password, byte[] passwordHash)
        //{

        //}

        //private byte[] HashPassword(string password)
        //{

        //}








        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileForamtId"></param>
        /// <param name="patchedFileFormat"></param>
        /// <returns></returns>
        public async Task<ManipulationResult> PatchUserAsync(int userId, int updateUserId, User patchedUser)
        {
            return await _squareDb.UpdateUserAsync(userId, updateUserId,
                patchedUser.LastName, patchedUser.FirstName, patchedUser.UserName,
                patchedUser.Email, patchedUser.PasswordHash, patchedUser.Active);
        }

        //public async Task<RetrievalResult<User>> GetUsersAsync()

        // brauche methode um Passwort für benutzer abzurufen, ohne dass ich die userId habe.
        // methode die die passwörter dann vergleicht usw. sieht UserService.
    }
}
