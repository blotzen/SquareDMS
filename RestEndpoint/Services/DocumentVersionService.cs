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


        #endregion
    }
}
