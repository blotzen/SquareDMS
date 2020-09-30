using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentService
    {
        private readonly DocumentDispatcher _documentDispatcher;

        /// <summary>
        /// Receives the dispatcher via DI.
        /// </summary>
        /// <param name="documentDispatcher"></param>
        public DocumentService(DocumentDispatcher documentDispatcher)
        {
            _documentDispatcher = documentDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Document>> CreateDocumentAsync(int userId, Document document)
        {
            return await _documentDispatcher.CreateDocumentAsync(userId, document);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<Document>> RetrieveDocumentAsync(int userId,
            int? docId, int? creator, int? docType, string name, bool? locked, bool? dicard)
        {
            return await _documentDispatcher.RetrieveDocumentAsync(userId, docId, creator, docType,
                name, locked, dicard);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Document>> UpdateDocumentAsync(int userId, int id, Document patchedDocument)
        {
            // id and creator cant be changed
            if (patchedDocument.Id is null && patchedDocument.Creator is null)
            {
                return await _documentDispatcher.UpdateDocumentAsync(userId, id, patchedDocument);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult<Document>> DeleteDocumentAsync(int userId, int id)
        {
            return await _documentDispatcher.DeleteDocumentAsync(userId, id);
        }
        #endregion
    }
}
