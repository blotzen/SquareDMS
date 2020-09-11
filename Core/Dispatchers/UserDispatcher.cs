using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Linq;
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

        /// <summary>
        /// Validates the given credentials. Checks if the password matches the one
        /// stored in the database. Also checks if the username is valid and has a matching
        /// userId in the database. Returns null if the credentials are invalid or wrong.
        /// </summary>
        public async Task<User> CheckUserCredentialAsync(Credential userCredential)
        {
            // gets the first result in the resultset, its only one user.
            var user = (await _squareDb.RetrieveUserByUserNameAsync(userCredential.UserName)).Resultset.ToList()[0];

            // user not found in db
            if (user is null)
                return null;

            var pwsMatch = Credential.MatchPasswordHashes(user.PasswordHash, userCredential.HashPassword());

            // passwords dont match
            if (!pwsMatch)
                return null;

            return user;
        }

        /// <summary>
        /// Creates a new user in the database or returns an errorcode if this
        /// operation faulty.
        /// </summary>
        /// <param name="userId">User that creates the user</param>
        /// <param name="user">User to be created</param>
        /// <returns>ManipulationResult with errorCode</returns>
        public async Task<ManipulationResult> CreateUserAsync(int userId, User user)
        {
            return await _squareDb.CreateUserAsync(userId, user);
        }

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

    }
}
