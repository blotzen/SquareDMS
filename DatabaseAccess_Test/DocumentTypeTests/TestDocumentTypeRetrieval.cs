using System.Linq;
using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.DocumentTypeTest
{
    [Collection("Sequential")]
    public class TestDocumentTypeRetrieval : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentTypeRetrieval(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin gets all the document types.
        /// </summary>
        [Fact]
        public async void Admin_Get_All_DocumentTypes()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentTypeAsync(1);

            Assert.Equal(3, readResult.Resultset.Count());
        }

        /// <summary>
        /// Admin gets doc type by id.
        /// </summary>
        [Fact]
        public async void Admin_Get_One_DocumentTypes()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentTypeAsync(1, docTypeId: 1);

            Assert.Single(readResult.Resultset);
        }

        /// <summary>
        /// Admin gets doc type by name. (also checks case insensitivity of the name)
        /// </summary>
        [Fact]
        public async void Admin_Get_One_DocumentTypes_by_name()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentTypeAsync(1, name: "dokumentation");

            Assert.Single(readResult.Resultset);
        }

        /// <summary>
        /// User gets all the document types.
        /// </summary>
        [Fact]
        public async void User_Get_All_DocumentTypes()
        {
            var readResult = await _squareDbMsSql.RetrieveDocumentTypeAsync(2);

            Assert.Equal(3, readResult.Resultset.Count());
        }
    }
}
