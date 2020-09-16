using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    public class DocumentDispatcher : Dispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public DocumentDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operationen
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentAsync(int userId, Document document)
        {
            return await _squareDb.CreateDocumentAsync(userId, document);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Document>> RetrieveDocumentAsync(int userId,
            int? docId, int? creator, int? docType, string name, bool? locked, bool? dicard)
        {
            return await _squareDb.RetrieveDocumentsAsync(userId: userId, docId: docId, creator: creator,
                docType: docType, name: name, locked: locked, discard: dicard);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateDocumentAsync(int userId, int id, Document patchedDocument)
        {
            return await _squareDb.UpdateDocumentAsync(userId, id, patchedDocument.DocumentType, patchedDocument.Name,
                patchedDocument.Locked, patchedDocument.Discard);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteDocumentAsync(int userId, int id)
        {
            return await _squareDb.DeleteDocumentAsync(userId, id);
        }
        #endregion
    }
}
