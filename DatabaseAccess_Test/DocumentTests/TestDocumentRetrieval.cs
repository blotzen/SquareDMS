using SquareDMS.DatabaseAccess;
using System.Linq;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.DocumentTests
{
    [Collection("Sequential")]
    public class TestDocumentRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin gets all documents. No restrictions.
        /// </summary>
        [Fact]
        public async void Admin_get_all_Docs()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(1);
            Assert.Equal(12, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets one document by id.
        /// </summary>
        [Fact]
        public async void Admin_get_one_Doc_by_id()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(1, docId: 1);
            var readDoc = readResult.Resultset.ToList()[0];

            Assert.Single(readResult.Resultset);

            Assert.Equal("2015_Book_ExpertSQLServerIn-MemoryOLTP", readDoc.Name);
            Assert.Equal(1, readDoc.Creator);
            Assert.False(readDoc.Locked);
            Assert.False(readDoc.Discard);

            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets one document by name.
        /// </summary>
        [Fact]
        public async void Admin_get_one_Doc_by_name()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(1, name: "Die Bibel");
            Assert.Single(readResult.Resultset);
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets one document by name and creator id
        /// </summary>
        [Fact]
        public async void Admin_get_one_Doc_by_name_and_creator()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(1, name: "Die Bibel", creator: 2);
            Assert.Single(readResult.Resultset);
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Quentin gets all documents he has access to
        /// </summary>
        [Fact]
        public async void Quentin_gets_all_documents()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(2);
            Assert.Equal(5, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Quentin gets all documents he has access to with at least write permission
        /// </summary>
        [Fact]
        public async void Quentin_gets_all_documents_with_write_permission()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(2, maxAccessLevel: 200);
            Assert.Equal(3, readResult.Resultset.Count());
            Assert.Equal(0, readResult.ErrorCode);
        }

        /// <summary>
        /// Benutzer NoRight gets all documents he has access to
        /// </summary>
        [Fact]
        public async void UserNoRights_gets_all_documents()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentsAsync(3);
            Assert.Empty(readResult.Resultset);
            Assert.Equal(0, readResult.ErrorCode);
        }
    }
}
