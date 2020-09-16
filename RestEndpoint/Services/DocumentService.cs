using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ManipulationResult> CreateDocumentAsync(int userId, Document document)
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
        public async Task<ManipulationResult> UpdateDocumentAsync(int userId, int id, Document patchedDocument)
        {
            return await _documentDispatcher.UpdateDocumentAsync(userId, id, patchedDocument);
        }
        #endregion
    }
}
