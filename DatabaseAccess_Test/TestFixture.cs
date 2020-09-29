using SquareDMS.DatabaseAccess;
using System;

namespace SquareDMS.DatabaseAccess_Tests
{
    public class TestFixture : IDisposable
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);

        /// <summary>
        /// Constructor calls the ResetDbToTestValues() Function.
        /// </summary>
        public TestFixture()
        {
            ResetDbToTestValues();
        }

        public void Dispose()
        {
            ResetDbToTestValues();
        }

        /// <summary>
        /// Clears Db and inserts test data.
        /// </summary>
        private void ResetDbToTestValues()
        {
            var deleteAllResult = _squareDbMsSql.SysDeleteAndResetAllData();

            if (deleteAllResult != 0)
            {
                throw new Exception("Tests cant be started, due to errors in deleting" +
                    $" data from the Database. Error-Code: {deleteAllResult}");
            }

            var testSetupResult = _squareDbMsSql.SysInsertSetupTestData();

            if (testSetupResult != 0)
            {
                throw new Exception("Tests cant be started, due to errors in inserting" +
                    $" data in the Database. Error-Code: {testSetupResult}");
            }
        }
    }
}