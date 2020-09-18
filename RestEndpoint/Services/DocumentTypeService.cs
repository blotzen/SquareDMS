using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentTypeService
    {
        private readonly DocumentTypeDispatcher _documentTypeDispatcher;

        /// <summary>
        /// Receives the dispatcher via DI.
        /// </summary>
        /// <param name="documentDispatcher"></param>
        public DocumentTypeService(DocumentTypeDispatcher documentTypeDispatcher)
        {
            _documentTypeDispatcher = documentTypeDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentTypeAsync(int userId, DocumentType documentType)
        {
            return await _documentTypeDispatcher.CreateDocumentTypeAsync(userId, documentType);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<DocumentType>> RetrieveDocumentTypeAsync(int userId, int? docTypeId,
            string name, string description)
        {
            return await _documentTypeDispatcher.RetrieveDocumentTypeAsync(userId, docTypeId, name, description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateDocumentTypeAsync(int userId, int id, DocumentType patchedDocumentType)
        {
            if (patchedDocumentType.Id is null)
            {
                return await _documentTypeDispatcher.UpdateDocumentTypeAsync(userId, id, patchedDocumentType);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteDocumentTypeAsync(int userId, int id)
        {
            return await _documentTypeDispatcher.DeleteDocumentTypeAsync(userId, id);
        }

        #endregion
    }
}
