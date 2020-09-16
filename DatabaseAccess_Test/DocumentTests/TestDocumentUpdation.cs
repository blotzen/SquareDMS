using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using System.Linq;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.DocumentTests
{
    [Collection("Sequential")]
    public class TestDocumentUpdation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentUpdation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Updates a document as Admin, sets it to locked.
        /// </summary>
        [Fact]
        public async void Admin_SetLockFlag_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(1, 2, locked: true);

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(Document), OperationType.Update));
        }

        /// <summary>
        /// Try to set an empty name as admin.
        /// </summary>
        [Fact]
        public async void Admin_SetInvalidName_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(1, 6, name: "");

            Assert.Equal(120, updationResult.ErrorCode);
        }

        /// <summary>
        /// Try to set with no rights.
        /// </summary>
        [Fact]
        public async void NoRight_SetDiscard_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(3, 1, discard: true);

            Assert.Equal(12, updationResult.ErrorCode);
        }

        /// <summary>
        /// Try to set with insufficient rights.
        /// </summary>
        [Fact]
        public async void InsufficientRight_SetDiscard_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(2, 11, discard: true);

            Assert.Equal(12, updationResult.ErrorCode);
        }

        /// <summary>
        /// Set valid name with Rights.
        /// </summary>
        [Fact]
        public async void HasRights_SetName_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(4, 5, name: "Integrationstest_geändert_1");

            Assert.Equal(0, updationResult.ErrorCode);
        }

        /// <summary>
        /// Updates a document as Creator of the document, sets it to unlocked.
        /// </summary>
        [Fact]
        public async void CreatorOfDoc_UnlockDoc_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(2, 4, locked: false);

            Assert.Equal(0, updationResult.ErrorCode);
        }

        /// <summary>
        /// Try to update document, set to unlocked, user has insufficient right.
        /// </summary>
        [Fact]
        public async void User_UnlockDoc_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(2, 2, locked: false);

            Assert.Equal(12, updationResult.ErrorCode);
        }

        /// <summary>
        /// Try to update discarded document.
        /// </summary>
        [Fact]
        public async void Admin_ModifyDiscardedDoc_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(1, 3, name: "Integrationstest_geändert_2");

            Assert.Equal(121, updationResult.ErrorCode);
        }

        /// <summary>
        /// Try to update locked document.
        /// </summary>
        [Fact]
        public async void Creator_ModifyLockedDoc_Async()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentAsync(1, 2, name: "Integrationstest_geändert_3");

            Assert.Equal(11, updationResult.ErrorCode);
        }
    }
}
