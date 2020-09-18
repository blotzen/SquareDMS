using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    public class DocumentTypeDispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public DocumentTypeDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operationen
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentTypeAsync(int userId, DocumentType documentType)
        {
            return await _squareDb.CreateDocumentTypeAsync(userId, documentType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="docTypeId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<RetrievalResult<DocumentType>> RetrieveDocumentTypeAsync(int userId, int? docTypeId,
            string name, string description)
        {
            return await _squareDb.RetrieveDocumentTypeAsync(userId, docTypeId, name, description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateDocumentTypeAsync(int userId, int id, DocumentType patchedDocumentType)
        {
            return await _squareDb.UpdateDocumentTypeAsync(userId, id, patchedDocumentType.Name, patchedDocumentType.Description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteDocumentTypeAsync(int userId, int id)
        {
            return await _squareDb.DeleteDocumentTypeAsync(userId, id);
        }
        #endregion
    }
}
