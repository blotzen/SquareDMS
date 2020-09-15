using Xunit;

namespace SquareDMS.DatabaseAccess_Tests.DocumentTypeTest
{
    [Collection("Sequential")]
    public class TestDocumentTypeUpdation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentTypeUpdation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Admin updates a doctype name.
        /// </summary>
        [Fact]
        public async void Admin_Update_DocTypeName()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentTypeAsync(1, 2, name: "Skripte");

            Assert.Equal(0, updationResult.ErrorCode);
            Assert.Equal(1, updationResult.ManipulatedAmount(typeof(DocumentType), OperationType.Update));
        }

        /// <summary>
        /// User try to updates a doctype name.
        /// </summary>
        [Fact]
        public async void User_Update_DocTypeName()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentTypeAsync(2, 2, name: "Skripte");

            Assert.Equal(10, updationResult.ErrorCode);
        }

        /// <summary>
        /// Admin try to update doctype so name matches an existing one.
        /// </summary>
        [Fact]
        public async void Admin_Update_Name_to_Duplicate()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentTypeAsync(1, 2, name: "Skript");

            Assert.Equal(109, updationResult.ErrorCode);
        }

        /// <summary>
        /// Admin updates nothing.
        /// </summary>
        [Fact]
        public async void Admin_Update_Nothing()
        {
            var updationResult = await _squareDbMsSql.UpdateDocumentTypeAsync(1, 2);

            Assert.Equal(0, updationResult.ErrorCode);
        }
    }
}
