using SquareDMS.CacheAccess;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Linq;
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

        /// <summary>
        /// Gets a document version by its id, fetches the payload from the cache or from the filestream
        /// </summary>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionAsync(int userId, int documentVersionId)
        {
            var metadataRetrievalResultSet = await _squareDb.RetrieveDocumentVersionsMetaDataAsync(userId, documentVersionId);

            // if error occured just return and dont populate cache
            if (metadataRetrievalResultSet.ErrorCode != 0)
                return metadataRetrievalResultSet;

            var retrievedDocumentVersion = metadataRetrievalResultSet.Resultset.FirstOrDefault();

            // no matching document version found
            if (retrievedDocumentVersion is null)
                return metadataRetrievalResultSet;

            // check if retrieved version is in the cache
            var cachedPayload = await _squareCache.RetrieveDocumentVersionPayloadAsync(retrievedDocumentVersion.Id);

            // if payload is not cached
            if (cachedPayload is null)
            {
                var filestreamRetrieve = await _squareDb.RetrieveDocumentVersionAsync(userId, retrievedDocumentVersion.Id);
                // fetch from filestream via db and set
                retrievedDocumentVersion.FilestreamData = filestreamRetrieve.Resultset.FirstOrDefault()?.FilestreamData;

                // fire and forget the task to populate the cache
                _squareCache.PutDocumentPayloadAsync(retrievedDocumentVersion.Id, retrievedDocumentVersion.FilestreamData).FireAndForget();
            }
            else
            {
                retrievedDocumentVersion.FilestreamData = cachedPayload;
            }

            return metadataRetrievalResultSet;
        }
    }
}