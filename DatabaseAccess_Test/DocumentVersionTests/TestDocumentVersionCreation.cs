using Microsoft.AspNetCore.Http;
using Moq;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using System;
using System.IO;
using Xunit;


namespace SquareDMS.DatabaseAccess_Tests.DocumentVersionTest
{
    [Collection("Sequential")]
    public class TestDocumentVersionCreation : IClassFixture<TestFixture>
    {
        private readonly SquareDbMsSql _squareDbMsSql = new SquareDbMsSql(Globals.SqlConnectionstring);
        private readonly TestFixture _fixture;
        private readonly byte[] _file = new byte[] { 1, 234, 1, 44, 41, 23, 123 };

        /// <param name="fixture">Class wide fixture</param>
        public TestDocumentVersionCreation(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Creates a mocked FormFile
        /// </summary>
        private IFormFile MockFormFile()
        {
            var mockedFormFile = new Mock<IFormFile>();

            mockedFormFile.Setup(mock => mock.OpenReadStream()).Returns(new MemoryStream(_file));
            mockedFormFile.Setup(mock => mock.ContentType).Returns("application/pdf");

            return mockedFormFile.Object;
        }

        /// <summary>
        /// Admin creates a valid document version
        /// </summary>
        [Fact]
        public async void Admin_Create_Valid_DocVersion()
        {
            var docVer = new DocumentVersion(1, 1);
            docVer.UploadFile = MockFormFile();

            var creationResult = await _squareDbMsSql.CreateDocumentVersionAsync(1, docVer);

            Assert.Equal(0, creationResult.ErrorCode);
            Assert.Equal(1, creationResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Create));
        }

        /// <summary>
        /// User without permissions tries to create a document version
        /// </summary>
        [Fact]
        public async void User_noPermissions_Create_Valid_DocVersion()
        {
            var docVer = new DocumentVersion(1, 1);
            docVer.UploadFile = MockFormFile();

            var creationResult = await _squareDbMsSql.CreateDocumentVersionAsync(10, docVer);

            Assert.Equal(12, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a documnt version without a valid document id.
        /// </summary>
        [Fact]
        public async void Admin_Create_DocVersion_invalidDocId()
        {
            var docVer = new DocumentVersion(99, 1);
            docVer.UploadFile = MockFormFile();

            var creationResult = await _squareDbMsSql.CreateDocumentVersionAsync(1, docVer);

            Assert.Equal(114, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a documnt version without a valid file format.
        /// </summary>
        [Fact]
        public async void Admin_Create_DocVersion_invalidFileFormat()
        {
            var docVer = new DocumentVersion(1, 99);
            docVer.UploadFile = MockFormFile();

            var creationResult = await _squareDbMsSql.CreateDocumentVersionAsync(1, docVer);

            Assert.Equal(118, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a documnt version with null payload
        /// </summary>
        [Fact]
        public async void Admin_Create_DocVersion_NullFile()
        {
            var docVer = new DocumentVersion(1, 1);
            docVer.UploadFile = null;

            var creationResult = await _squareDbMsSql.CreateDocumentVersionAsync(1, docVer);

            Assert.Equal(129, creationResult.ErrorCode);
            Assert.Equal(0, creationResult.ManipulatedAmount(typeof(DocumentVersion), OperationType.Create));
        }

        /// <summary>
        /// Admin tries to create a null documentVersion
        /// </summary>
        [Fact]
        public async System.Threading.Tasks.Task Admin_Create_DocVersion_NullDocVersion()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _squareDbMsSql.CreateDocumentVersionAsync(1, null));
        }
    }
}
