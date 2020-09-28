using SquareDMS.DatabaseAccess;
using System.Linq;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.DocumentVersionTest
{
    [Collection("Sequential")]
    public class TestDocumentVersionRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentVersionRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin gets all document versions.
        /// This is not possible! If it should be
        /// the rights for every single document version
        /// have to checked. This is quite expensive!
        /// </summary>
        [Fact]
        public async void Admin_Get_all_DocVersions()
        {
            var retrievalResult = await _squareDbMsSql.RetrieveDocumentVersionsMetaDataAsync(1);

            Assert.Equal(1, retrievalResult.ErrorCode);
        }

        /// <summary>
        /// Admin gets a single document version without metadata.
        /// </summary>
        [Fact]
        public async void Admin_Get_single_DocVersion_NoMetaData()
        {
            var retrievalResult = await _squareDbMsSql.RetrieveDocumentVersionsMetaDataAsync(1, 2);

            Assert.Single(retrievalResult.Resultset);
            Assert.Null(retrievalResult.Resultset.ToList()[0].DownloadFile);
        }

        /// <summary>
        /// Admin gets a single document version with metadata.
        /// </summary>
        [Fact]
        public async void Admin_Get_single_DocVersion_MetaData()
        {
            var retrievalResult = await _squareDbMsSql.RetrieveDocumentVersionAsync(1, 2);

            Assert.Single(retrievalResult.Resultset);
            Assert.Equal(128, retrievalResult.Resultset.ToList()[0].RawFile.Length);
        }
    }
}
