using SquareDMS.DatabaseAccess;
using System;

namespace SquareDMS.System_Test
{
    /// <summary>
    /// Fixture that clears the db and sets the default admin and the default groups.
    /// Just like after a fresh installation of the database.
    /// </summary>
    public class ResetDbFixture : IDisposable
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);

        public ResetDbFixture()
        {
            ClearDbAndInsertDefaultAdminGroups();
        }

        public void Dispose()
        {
            ClearDbAndInsertDefaultAdminGroups();
        }

        // <summary>
        /// Clears Db 
        /// </summary>
        private void ClearDbAndInsertDefaultAdminGroups()
        {
            var deleteAllResult = _squareDbMsSql.SysDeleteAndResetAllData();

            if (deleteAllResult != 0)
            {
                throw new Exception("Tests cant be started, due to errors in deleting" +
                    $" data from the Database. Error-Code: {deleteAllResult}");
            }

            var insertDefaultAdminAndGroupsResult = _squareDbMsSql.SysInsertDefaultAdminAndGroups();

            if (insertDefaultAdminAndGroupsResult != 0)
            {
                throw new Exception("Tests cant be started, due to errors in inserting" +
                    $" default admin data in the db. Error-Code: {insertDefaultAdminAndGroupsResult}");
            }
        }
    }
}
