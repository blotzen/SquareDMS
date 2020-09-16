using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Linq;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    /// <summary>
    /// Mostly passes the operations from the
    /// UserController to the database. Does some checks 
    /// regarding the Credentials.
    /// </summary>
    public class UserDispatcher : Dispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public UserDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        /// <summary>
        /// Validates the given credentials. Checks if the password matches the one
        /// stored in the database. Also checks if the username is valid and has a matching
        /// userId in the database. Returns null if the credentials are invalid or wrong.
        /// </summary>
        public async Task<User> CheckUserCredentialAsync(Credential userCredential)
        {
            // gets the first result in the resultset, its only one user.
            var user = (await _squareDb.RetrieveUserByUserNameAsync(userCredential.UserName)).Resultset.FirstOrDefault();

            // user not found in db
            if (user is null)
                return null;

            var pwsMatch = Credential.MatchPasswordHashes(user.PasswordHash, userCredential.HashPassword());

            // passwords dont match
            if (!pwsMatch)
                return null;

            return user;
        }

        #region CRUD-Operationen
        /// <summary>
        /// Creates a new user in the database or returns an errorcode if this
        /// operation faulty.
        /// </summary>
        /// <param name="id">User that creates the user</param>
        /// <param name="user">User to be created</param>
        /// <returns>ManipulationResult with errorCode</returns>
        public async Task<ManipulationResult> CreateUserAsync(int id, User user)
        {
            return await _squareDb.CreateUserAsync(id, user);
        }

        /// <summary>
        /// Gets the users that match the params.
        /// </summary>
        public async Task<RetrievalResult<User>> RetrieveUsersAsync(int id, int? userId = null,
            string lastName = null, string firstName = null,
            string userName = null, string email = null,
            bool? active = null)
        {
            return await _squareDb.RetrieveUserAsync(id, userId, lastName, firstName, userName, email, active);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        public async Task<ManipulationResult> UpdateUserAsync(int id, int updateUserId, User patchedUser)
        {
            return await _squareDb.UpdateUserAsync(id, updateUserId,
                patchedUser.LastName, patchedUser.FirstName, patchedUser.UserName,
                patchedUser.Email, patchedUser.PasswordHash, patchedUser.Active);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        public async Task<ManipulationResult> DeleteUserAsync(int id, int deleteUserId)
        {
            return await _squareDb.DeleteUserAsync(id, deleteUserId);
        }
        #endregion
    }
}
