using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace RestEndpoint.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentVersionService
    {
        private readonly DocumentVersionDispatcher _documentVersionDispatcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentVersionDispatcher"></param>
        public DocumentVersionService(DocumentVersionDispatcher documentVersionDispatcher)
        {
            _documentVersionDispatcher = documentVersionDispatcher;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentVersionAsync(int userId, DocumentVersion documentVersion)
        {
            return await _documentVersionDispatcher.CreateDocumentVersionAsync(userId, documentVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionMetadataAsync(int userId, int? documentVersionId,
            int? documentId)
        {
            return await _documentVersionDispatcher.RetrieveDocumentVersionMetadataAsync(userId, documentVersionId, documentId);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionPayloadAsync(int userId, int documentVersionId)
        {
            return await _documentVersionDispatcher.RetrieveDocumentVersionPayloadAsync(userId, documentVersionId);
        }
        #endregion
    }
}
