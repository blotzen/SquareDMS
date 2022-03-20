using Microsoft.AspNetCore.Mvc;
using NLog;
using SquareDMS.CacheAccess;
using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System;
using System.IO;
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
        public async Task<ManipulationResult<DocumentVersion>> CreateDocumentVersionAsync(int userId, DocumentVersion documentVersion)
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
            {
                _logger.Error("An error occured while trying to retrieve the documentVersion MetaData (Error-Code: {0})",
                    metadataRetrievalResultSet.ErrorCode);
                return metadataRetrievalResultSet;
            }

            var retrievedDocumentVersion = metadataRetrievalResultSet.Resultset.FirstOrDefault();

            // no matching document version found
            if (retrievedDocumentVersion is null)
            {
                _logger.Info("The document Version was not found (ID: {0})", documentVersionId);
                return metadataRetrievalResultSet;
            }

            // get corresponding document for document version
            var correspondingDocumentRetrieval = _squareDb.RetrieveDocumentsAsync(userId, docId: retrievedDocumentVersion.DocumentId);

            // get file format for document
            var docFileFormatRetrieval = _squareDb.RetrieveFileFormatsAsync(userId, retrievedDocumentVersion.FileFormatId);

            // payload of the file (rawFile)
            byte[] payload = null;

            //try
            //{
            //    payload = await _squareCache.RetrieveDocumentVersionPayloadAsync(retrievedDocumentVersion.Id.Value);
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error("An error occured while trying to retrieve a playload from redis: {0}", ex.Message);
            //}

            // if payload is not cached
            if (payload is null)
            {
                // fetch from filestream via db and set to rawFile property (byte[])
                var filestreamRetrieve = await _squareDb.RetrieveDocumentVersionAsync(userId, retrievedDocumentVersion.Id.Value);
                payload = filestreamRetrieve.Resultset.FirstOrDefault()?.RawFile;

                try
                {
                    // populate the cache
                    //await _squareCache.PutDocumentPayloadAsync(retrievedDocumentVersion.Id.Value, payload);
                }
                catch (Exception ex)
                {
                    _logger.Error("An error occured while trying to put a playload into redis: {0}", ex.Message);
                }
            }

            // await document fileformat
            var documentFileFormatRetrievalResult = await docFileFormatRetrieval;
            var documentFileFormat = documentFileFormatRetrievalResult.Resultset.FirstOrDefault();

            // file format may be null (race condition)
            if (documentFileFormat is null)
            {
                _logger.Error("File-Format for document (ID: {0}) was not found", retrievedDocumentVersion.DocumentId);
                return metadataRetrievalResultSet;
            }

            try
            {
                // create downloadFile from rawFile and media type (MIME type)
                retrievedDocumentVersion.DownloadFile = new FileStreamResult(new MemoryStream(payload),
                    $"application/{documentFileFormat.Extension}");

                // await the corresponding document at last
                var correspondingDocument = (await correspondingDocumentRetrieval).Resultset.FirstOrDefault();

                retrievedDocumentVersion.DownloadFile.FileDownloadName = correspondingDocument?.Name ?? $"Document_Version_{DateTime.Now}";

            }
            catch (Exception ex)
            {
                _logger.Error("Error while creating the DownloadFile, maybe the MIME type is wrong? {0}", ex.Message);
            }

            return metadataRetrievalResultSet;
        }
    }
}