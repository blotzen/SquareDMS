using NLog;
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
        private readonly Logger _logger;

        /// <summary>
        /// 
        /// </summary>
        public DocumentVersionDispatcher(ISquareDb squareDb, ISquareCache squareCache)
        {
            _squareDb = squareDb;
            _squareCache = squareCache;
            _logger = LogManager.GetLogger("DocumentVersionDispatcher");
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateDocumentVersionAsync(int userId, DocumentVersion documentVersion)
        {
            return await _squareDb.CreateDocumentVersionAsync(userId, documentVersion);
        }

        /// <summary>
        /// Gets a document version by its id or its document id. Only returns the metadata.
        /// </summary>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionMetadataAsync(int userId, int? documentVersionId,
            int? documentId)
        {
            return await _squareDb.RetrieveDocumentVersionsMetaDataAsync(userId, documentVersionId, documentId);
        }

        /// <summary>
        /// Gets a document version by its id, fetches the payload from the cache or from the filestream.
        /// </summary>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionPayloadAsync(int userId, int documentVersionId)
        {
            var metadataRetrievalResultSet = await _squareDb.RetrieveDocumentVersionsMetaDataAsync(userId, documentVersionId);

            // if error occured just return and dont populate cache
            if (metadataRetrievalResultSet.ErrorCode != 0)
                return metadataRetrievalResultSet;

            var retrievedDocumentVersion = metadataRetrievalResultSet.Resultset.FirstOrDefault();

            // get document version for the document
            var documentFileFormatRetrieval = _squareDb.RetrieveFileFormatsAsync(userId, retrievedDocumentVersion.DocumentId);

            // no matching document version found
            if (retrievedDocumentVersion is null)
                return metadataRetrievalResultSet;

            // check if retrieved version is in the cache
            //var cachedPayload = await _squareCache.RetrieveDocumentVersionPayloadAsync(retrievedDocumentVersion.Id);
            object cachedPayload = null;

            // if payload is not cached
            if (cachedPayload is null)
            {
                var filestreamRetrieve = await _squareDb.RetrieveDocumentVersionAsync(userId, retrievedDocumentVersion.Id);
                //// fetch from filestream via db and set
                retrievedDocumentVersion.DownloadFile = filestreamRetrieve.Resultset.FirstOrDefault()?.DownloadFile;

                //// fire and forget the task to populate the cache
                //_squareCache.PutDocumentPayloadAsync(retrievedDocumentVersion.Id, retrievedDocumentVersion.FilestreamData).FireAndForget();
            }
            else
            {
                //retrievedDocumentVersion.FilestreamData = cachedPayload;
            }

            // await document fileformat
            //var documentFileFormatRetrievalResult = await documentFileFormatRetrieval;

            //var documentFileFormat = documentFileFormatRetrievalResult.Resultset.FirstOrDefault();

            // use file format for content type
            //retrievedDocumentVersion.FormFile.ContentType = documentFileFormat.XXXXXXXXXXXXXXX
            //retrievedDocumentVersion.DownloadFile.GenerateMediaType("application/pdf");

            return metadataRetrievalResultSet;
        }
    }
}