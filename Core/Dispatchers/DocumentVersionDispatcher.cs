using SquareDMS.CacheAccess;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentVersionDispatcher : Dispatcher
    {
        private readonly ISquareDb _squareDb;
        private readonly ISquareCache _squareCache;

        /// <summary>
        /// 
        /// </summary>
        public DocumentVersionDispatcher(ISquareDb squareDb, ISquareCache squareCache)
        {
            _squareDb = squareDb;
            _squareCache = squareCache;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentVersionAsync(int userId, DocumentVersion documentVersion)
        {
            return await _squareDb.CreateDocumentVersionAsync(userId, documentVersion);
        }
    }
}
