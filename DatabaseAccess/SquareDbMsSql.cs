using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SquareDMS.DataLibrary;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SquareDMS.DatabaseAccess
{
    /// <summary>
    /// Used for Db-Access to a MS SQL Db.
    /// </summary>
    public class SquareDbMsSql : ISquareDb
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor for the MS SQL Db Access Class.
        /// </summary>
        /// <param name="connectonString">Connection String to the
        /// MS SQL DB.</param>
        public SquareDbMsSql(IConfiguration configuration)
        {
            _connectionString = configuration["MsSqlDb:ConnectionString"].ToString();
        }

        /// <summary>
        /// Constructor for testing where the conStr. is directly supplied.
        /// </summary>
        public SquareDbMsSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Document-Operations
        /// <summary>
        /// Creates a document. Checks if the user is a Creator and therefore
        /// authorized to create a document.
        /// </summary>
        /// <param name="doc">Document to be created.</param>
        /// <returns>Result contains an Errorcode that is 0 if the operation
        /// succeeded.</returns>
        /// <exception cref="ArgumentNullException">Doc cant be null.</exception>
        public async Task<ManipulationResult> CreateDocumentAsync(Document doc)
        {
            if (doc is null)
                throw new ArgumentNullException("doc", "Cant create null document.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", doc.Creator, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docType", doc.DocumentType, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@locked", doc.Locked, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@discard", doc.Discard, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@name", doc.Name, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdDocuments", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_document]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdDocuments = parameters.Get<int>("createdDocuments");

            return new ManipulationResult(errorCode, new Operation(typeof(Document), createdDocuments, OperationType.Create));
        }

        /// <summary>
        /// Gets a document or multiple documents, depending on the given paramters.
        /// </summary>
        /// <returns>The documents and a errorCode.</returns>
        public async Task<RetrievalResult<Document>> RetrieveDocumentsAsync(int userId, [Optional] int? maxAccessLevel,
            [Optional] int? docId, [Optional] int? creator, [Optional] int? docType,
            [Optional] string name, [Optional] bool? locked, [Optional] bool? discard)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@maxAccessLevel", maxAccessLevel ?? (object)DBNull.Value, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@creatorId", creator, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docTypeId", docType, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@locked", locked, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@discard", discard, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<Document> documents;

            using (var connection = new SqlConnection(_connectionString))
            {
                documents = await connection.QueryAsync<Document>("[proc_get_document_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return new RetrievalResult<Document>(parameters.Get<int>("errorCode"), documents);
        }

        /// <summary>
        /// Updates a document. Locked doc can only be updated by the creator. An empty doc name is not permitted.
        /// </summary>
        /// <returns></returns>
        public async Task<ManipulationResult> UpdateDocumentAsync(int userId, int docId, [Optional] int? docType,
            [Optional] string name, [Optional] bool? locked, [Optional] bool? discard)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docTypeId", docType, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@locked", locked, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@discard", discard, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedDocuments", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_update_document]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var updatedDocuments = parameters.Get<int>("updatedDocuments");

            return new ManipulationResult(errorCode, new Operation(typeof(Document), updatedDocuments, OperationType.Update));
        }

        /// <summary>
        /// Deletes a document and its depending values like document-versions and Rights.
        /// Checks if the user is Admin and has permission to delete the document.
        /// Document must not be locked.
        /// </summary>
        /// <param name="userId">User that deletes.</param>
        /// <param name="docId">Document to be deleted.</param>
        /// <returns>Result contains the amount of deleted rights,
        /// the amount of deleted document-versions and 
        /// and an Errorcode that is 0 if the operation
        /// succeeded.</returns>
        public async Task<ManipulationResult> DeleteDocumentAsync(int userId, int docId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@deletedRights", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedDocVersions", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedDocuments", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_delete_document]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedRights = parameters.Get<int>("deletedRights");
            var deletedDocVersions = parameters.Get<int>("deletedDocVersions");
            var deletedDocuments = parameters.Get<int>("deletedDocuments");

            return new ManipulationResult(errorCode, new Operation(typeof(Right), deletedRights, OperationType.Delete),
                new Operation(typeof(DocumentVersion), deletedDocVersions, OperationType.Delete),
                new Operation(typeof(Document), deletedDocuments, OperationType.Delete));
        }
        #endregion

        #region DocumentType-Operations
        /// <summary>
        /// Creates a document type. User has to be admin
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">DocType cant be null.</exception>
        public async Task<ManipulationResult> CreateDocumentTypeAsync(int userId, DocumentType docType)
        {
            if (docType is null)
                throw new ArgumentNullException("docType", "Cant create null docType.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", docType.Name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", docType.Description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdDocumentTypes", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_document_type]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdDocumentTypes = parameters.Get<int>("createdDocumentTypes");

            return new ManipulationResult(errorCode, new Operation(typeof(DocumentType), createdDocumentTypes, OperationType.Create));
        }

        /// <summary>
        /// Gets one or multiple DocumentTypes depending on the param config.
        /// Admin and Users can retrieve all doc types.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<RetrievalResult<DocumentType>> RetrieveDocumentTypeAsync(int userId, [Optional] int? docTypeId,
            [Optional] string name, [Optional] string description)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@docTypeId", docTypeId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<DocumentType> documentTypes;

            using (var connection = new SqlConnection(_connectionString))
            {
                documentTypes = await connection.QueryAsync<DocumentType>("[proc_get_document_type_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return new RetrievalResult<DocumentType>(parameters.Get<int>("errorCode"), documentTypes);
        }

        /// <summary>
        /// Updates a Document Type. Only admin can update.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> UpdateDocumentTypeAsync(int userId, int docTypeId,
            [Optional] string name, [Optional] string description)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docTypeId", docTypeId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedDocumentTypes", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<DocumentType>("[proc_update_document_type]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var updatedDocumentTypes = parameters.Get<int>("updatedDocumentTypes");

            return new ManipulationResult(errorCode, new Operation(typeof(DocumentType), updatedDocumentTypes, OperationType.Update));
        }

        /// <summary>
        /// Deletes a Document Type. Only admin can delete and only 
        /// if no document uses the type currently.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteDocumentTypeAsync(int userId, int docTypeId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docTypeId", docTypeId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedDocumentTypes", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<DocumentType>("[proc_delete_document_type]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedDocumentTypes = parameters.Get<int>("deletedDocumentTypes");

            return new ManipulationResult(errorCode, new Operation(typeof(DocumentType), deletedDocumentTypes, OperationType.Delete));
        }
        #endregion

        #region DocumentVersion-Operations
        /// <summary>
        /// Creates a new DocumentVersion. Uses the byte[] and filestream with System.IO.
        /// to place the file in the filestream area. Uses the SQL Server file handle.
        /// </summary>
        /// <returns>Return value with error Code</returns>
        /// <exception cref="Exception"></exception>
        public async Task<ManipulationResult> CreateDocumentVersionAsync(int userId, DocumentVersion docVersion)
        {
            if (docVersion is null)
                throw new ArgumentNullException("docVersion", "Cant create null docVersion");

            DynamicParameters parameters = new DynamicParameters();

            // Bug, see here: https://stackoverflow.com/questions/54557416/what-is-the-correct-usage-of-dynamicparameters-dapper-for-a-varbinary-datatype
            var template_1 = Array.Empty<byte>();
            var template_2 = string.Empty;

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docVersion.DocumentId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@fileFormatId", docVersion.FileFormatId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdDocumentVersions", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@transactionContext", template_1, DbType.Binary, direction: ParameterDirection.Output, size: 16);
            parameters.Add("@filePath", template_2, DbType.String, direction: ParameterDirection.Output);
            parameters.Add("@createdId", DbType.Int32, direction: ParameterDirection.Output);

            int errorCode = 0;
            int createdDocumentVersions = 0;
            int? createdId = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.QueryAsync("[proc_create_document_version]", parameters,
                        commandType: CommandType.StoredProcedure, transaction: transaction);

                    errorCode = parameters.Get<int>("errorCode");
                    createdDocumentVersions = parameters.Get<int>("createdDocumentVersions");

                    var transactionContext = parameters.Get<byte[]>("transactionContext");
                    var filePath = parameters.Get<string>("filePath");
                    createdId = parameters.Get<int?>("createdId");

                    bool payloadWriteResult = false;

                    // meta data insert successful
                    if (errorCode == 0)
                    {
                        payloadWriteResult = await WriteDocumentVersionPayloadAsync(filePath, transactionContext, docVersion.UploadFile);

                        // payload insert successful
                        if (payloadWriteResult)
                        {
                            await transaction.CommitAsync();
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            errorCode = 129;
                            createdDocumentVersions = 0; // when rolled back no documents have been created
                            createdId = null;
                        }
                    }
                }
            }

            var result = new ManipulationResult(errorCode, new Operation(typeof(DocumentVersion), createdDocumentVersions, OperationType.Create));
            // result.ManipulatedId = createdId; // for next feature: when the payload should be put into the cache after insertion.

            return result;
        }

        /// <summary>
        /// Gets a specific document version metadata.
        /// </summary>
        /// <returns>Return value with error Code</returns>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionAsync(int userId, int docVerId)
        {
            DynamicParameters parameters = new DynamicParameters();
            int errorCode;

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docVerId", docVerId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<DocumentVersion> documentVersions;
            DownloadFile dlFile = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    documentVersions = await connection.QueryAsync<DocumentVersion>("[proc_get_document_version_s]", parameters,
                        commandType: CommandType.StoredProcedure, transaction: transaction);

                    // first or default(null) from enumeration
                    var documentVersion = documentVersions.FirstOrDefault();
                    errorCode = parameters.Get<int>("errorCode");

                    // meta data retrieve successful
                    if (errorCode == 0 && documentVersion != null)
                    {
                        // retrieve the payload and populate the documentVersion with it (null in case of error)
                        var memoryStream = RetrieveDocumentVersionPayloadAsync(documentVersion.FilePath, documentVersion.TransactionId);

                        // creates the file from the memoryStream
                        dlFile = new DownloadFile(await memoryStream);

                        documentVersion.DownloadFile = dlFile;
                    }

                    transaction.Commit();   // always commit because its read only.
                }
            }

            // set errorCode if payload retrieve from fs failed
            if (dlFile is null)
                errorCode = 128;

            return new RetrievalResult<DocumentVersion>(errorCode, documentVersions);
        }

        /// <summary>
        /// Gets the document versions for the given parameter, checks the permissions.
        /// Does not return the payload. This has to be done with <see cref="RetrieveDocumentVersionAsync(int, int)"/>
        /// </summary>
        /// <returns>Result with errorCode</returns>
        public async Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionsMetaDataAsync(int userId, [Optional] int? docVerId, [Optional] int? docId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docVerId", docVerId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<DocumentVersion> documentVersions;

            using (var connection = new SqlConnection(_connectionString))
            {
                documentVersions = await connection.QueryAsync<DocumentVersion>("[proc_get_document_version_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");

            return new RetrievalResult<DocumentVersion>(errorCode, documentVersions);
        }

        /// <summary>
        /// Gets the payload for a given document version.
        /// </summary>
        /// <returns>In case of an IO Error, null will be returned.</returns>
        private async Task<Stream> RetrieveDocumentVersionPayloadAsync(string filePath, byte[] transactionId)
        {
            try
            {
                using (var sqlFileStream = new SqlFileStream(filePath, transactionId, FileAccess.Read))
                {
                    var memoryStream = new MemoryStream(new byte[sqlFileStream.Length]);
                    await sqlFileStream.CopyToAsync(memoryStream);

                    return memoryStream;
                }

            }
            catch (Exception ex)
            {
                // log here
                return null;
            }
        }

        /// <summary>
        /// Writes the given document payload.
        /// </summary>
        /// <returns>True if successful, false if not.</returns>
        private async Task<bool> WriteDocumentVersionPayloadAsync(string filePath, byte[] transactionId, IFormFile formFile)
        {
            if (filePath is null || transactionId is null || formFile is null)
                return false;

            try
            {
                using (var sqlFileStream = new SqlFileStream(filePath, transactionId, FileAccess.Write))
                {
                    await formFile.CopyToAsync(sqlFileStream);
                    //await sqlFileStream.WriteAsync(payload);
                }

                return true;
            }
            catch (Exception ex)
            {
                // log ex
                return false;
            }
        }
        #endregion

        #region FileFormat-Operations
        /// <summary>
        /// Creates a new FileFormat
        /// </summary>
        /// <returns>Return value with error Code</returns>
        /// <exception cref="ArgumentNullException">FileFormat cant be null.</exception>
        public async Task<ManipulationResult> CreateFileFormatAsync(int userId, FileFormat fileFormat)
        {
            if (fileFormat is null)
                throw new ArgumentNullException("fileFormat", "Cant create null fileFormat.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@extension", fileFormat.Extension, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", fileFormat.Description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdFileFormats", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_file_format]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdFileFormats = parameters.Get<int>("createdFileFormats");

            return new ManipulationResult(errorCode, new Operation(typeof(FileFormat), createdFileFormats, OperationType.Create));
        }

        /// <summary>
        /// Gets one or multiple FileFormats depending on the paramters.
        /// </summary>
        /// <returns>Collection of FileFormats and ErrorCode</returns>
        public async Task<RetrievalResult<FileFormat>> RetrieveFileFormatsAsync(int userId, [Optional] int? fileFormatId,
            [Optional] string extension, [Optional] string description)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@fileFormatId", fileFormatId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@extension", extension, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<FileFormat> fileFormats;

            using (var connection = new SqlConnection(_connectionString))
            {
                fileFormats = await connection.QueryAsync<FileFormat>("[proc_get_file_format_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return new RetrievalResult<FileFormat>(parameters.Get<int>("errorCode"), fileFormats);
        }

        /// <summary>
        /// Updates a FileFromat if the user is admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> UpdateFileFormatAsync(int userId, int fileFormatId,
            [Optional] string extension, [Optional] string description)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@fileFormatId", fileFormatId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@extension", extension, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedFileFormats", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<FileFormat>("[proc_update_file_format]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var updatedFileFormats = parameters.Get<int>("updatedFileFormats");

            return new ManipulationResult(errorCode, new Operation(typeof(FileFormat), updatedFileFormats, OperationType.Update));
        }

        /// <summary>
        /// Deletes a file format, file format cant be used by any doc version.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteFileFormatAsync(int userId, int fileFormatId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@fileFormatId", fileFormatId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedFileFormats", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<FileFormat>("[proc_delete_file_format]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedFileFormats = parameters.Get<int>("deletedFileFormats");

            return new ManipulationResult(errorCode, new Operation(typeof(FileFormat), deletedFileFormats, OperationType.Delete));
        }
        #endregion

        #region Group-Operations
        /// <summary>
        /// Creates a Group. Only admins can.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Group cant be null.</exception>
        public async Task<ManipulationResult> CreateGroupAsync(int userId, Group group)
        {
            if (group is null)
                throw new ArgumentNullException("group", "Cant create null group.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", group.Name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", group.Description, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@admin", group.Admin, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@creator", group.Creator, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdGroups", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_group]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdGroups = parameters.Get<int>("createdGroups");

            return new ManipulationResult(errorCode, new Operation(typeof(Group), createdGroups, OperationType.Create));
        }

        /// <summary>
        /// Gets one or multiple Groups depending on the given params.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<RetrievalResult<Group>> RetrieveGroupAsync(int userId, [Optional] int? groupId,
            [Optional] string name, [Optional] string description, [Optional] bool? admin, [Optional] bool? creator)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@admin", admin, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@creator", creator, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<Group> groups;

            using (var connection = new SqlConnection(_connectionString))
            {
                groups = await connection.QueryAsync<Group>("[proc_get_group_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");

            return new RetrievalResult<Group>(errorCode, groups);
        }

        /// <summary>
        /// Updates a group. User has to be admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> UpdateGroupAsync(int userId, int groupId, [Optional] string name,
            [Optional] string description, [Optional] bool? admin, [Optional] bool? creator)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@name", name, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@description", description, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@admin", admin, DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@creator", creator, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedGroups", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_update_group]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var manipulatedGroups = parameters.Get<int>("updatedGroups");

            return new ManipulationResult(errorCode, new Operation(typeof(Group), manipulatedGroups, OperationType.Update));
        }

        /// <summary>
        /// Deletes Group if it has no Members and no Rights use it. 
        /// Only Admins can delete Groups.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteGroupAsync(int userId, int groupId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@deletedRights", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedGroups", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_delete_group]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedRights = parameters.Get<int>("deletedRights");
            var deletedGroups = parameters.Get<int>("deletedGroups");    // either 1 or 0, cant delete multiple groups

            return new ManipulationResult(errorCode,
                new Operation(typeof(Right), deletedRights, OperationType.Delete),
                new Operation(typeof(Group), deletedGroups, OperationType.Delete));
        }
        #endregion

        #region GroupMember-Operations
        /// <summary>
        /// Creates a Group Member. Possible if user is admin and the user isnt member of the 
        /// group already.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        /// <exception cref="ArgumentNullException">GroupMember cant be null.</exception>
        public async Task<ManipulationResult> CreateGroupMemberAsync(int userId, GroupMember groupMember)
        {
            if (groupMember is null)
                throw new ArgumentNullException("groupMember", "Cant create null groupMember.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@memberId", groupMember.UserId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupMember.GroupId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdGroupMembers", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_group_member]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdGroupMembers = parameters.Get<int>("createdGroupMembers");

            return new ManipulationResult(errorCode, new Operation(typeof(GroupMember), createdGroupMembers, OperationType.Create));
        }

        /// <summary>
        /// Gets one or more Group Members depending on the given paramters.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<RetrievalResult<GroupMember>> RetrieveGroupMemberAsync(int userId, [Optional] int? groupId,
            [Optional] int? memberId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@memberId", memberId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<GroupMember> groupMembers;

            using (var connection = new SqlConnection(_connectionString))
            {
                groupMembers = await connection.QueryAsync<GroupMember>("[proc_get_group_member_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");

            return new RetrievalResult<GroupMember>(errorCode, groupMembers);
        }

        /// <summary>
        /// Deletes a group Member if the user is admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteGroupMemberAsync(int userId, int groupId, int memberId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@memberId", memberId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedGroupMembers", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<GroupMember>("[proc_delete_group_member_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedGroupMembers = parameters.Get<int>("deletedGroupMembers");

            return new ManipulationResult(errorCode, new Operation(typeof(GroupMember), deletedGroupMembers, OperationType.Delete));
        }
        #endregion

        #region Right-Operations
        /// <summary>
        /// Creats a Right. Possible if user is admin and the given group and doc 
        /// have no common right already.
        /// </summary>
        /// <param name="userId">User that creats the Right (has to be Admin)</param>
        /// <param name="right">Right to be created</param>
        /// <returns>Result with errorCode.</returns>
        /// <exception cref="ArgumentNullException">Right cant be null.</exception>
        public async Task<ManipulationResult> CreateRightAsync(int userId, Right right)
        {
            if (right is null)
                throw new ArgumentNullException("right", "Cant create null right.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", right.GroupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", right.DocumentId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@accessLevel", right.AccessLevel, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdRights", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_right]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdRights = parameters.Get<int>("createdRights");

            return new ManipulationResult(errorCode, new Operation(typeof(Right), createdRights, OperationType.Create));
        }

        /// <summary>
        /// Gets one or more Rights depending on the given params.
        /// </summary>
        /// <returns>Collection of Rights and ErrorCode</returns>
        public async Task<RetrievalResult<Right>> RetrieveRightsAsync(int userId, [Optional] int? groupId,
            [Optional] int? docId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<Right> rights;

            using (var connection = new SqlConnection(_connectionString))
            {
                rights = await connection.QueryAsync<Right>("[proc_get_right_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return new RetrievalResult<Right>(parameters.Get<int>("errorCode"), rights);
        }

        /// <summary>
        /// Updates a Right. Right has to exist and user has to be Admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> UpdateRightAsync(int userId, int groupId,
            int docId, AccessLevel accessLevel)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@accessLevel", accessLevel, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedRights", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_update_right]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var updatedRights = parameters.Get<int>("updatedRights");

            return new ManipulationResult(errorCode, new Operation(typeof(Right), updatedRights, OperationType.Update));
        }

        /// <summary>
        /// Deletes one or more Rights. User has to be Admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteRightsAsync(int userId, int groupId, int docId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@groupId", groupId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@docId", docId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedRights", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_delete_right_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedRights = parameters.Get<int>("deletedRights");

            return new ManipulationResult(errorCode, new Operation(typeof(Right), deletedRights, OperationType.Delete));
        }
        #endregion

        #region User-Operations
        /// <summary>
        /// Creates a user. Only admins are allowed to.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        /// <exception cref="ArgumentNullException">Right cant be null.</exception>
        public async Task<ManipulationResult> CreateUserAsync(int userId, User user)
        {
            if (user is null)
                throw new ArgumentNullException("user", "Cant create null user.");

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@lastName", user.LastName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@firstName", user.FirstName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@userName", user.UserName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@email", user.Email, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@passwordHash", user.PasswordHash, DbType.Binary, direction: ParameterDirection.Input);
            parameters.Add("@active", user.Active, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@createdUsers", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_create_user]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var createdUsers = parameters.Get<int>("createdUsers");

            return new ManipulationResult(errorCode, new Operation(typeof(User), createdUsers, OperationType.Create));
        }

        /// <summary>
        /// Gets one or multiple users depending on the paramters. If user 
        /// is no admin, he can only see himself.
        /// </summary>
        /// <param name="userId">The user that issues the request.</param>
        /// <param name="retrieveUserId">Id of the user that is being looked for.</param>
        /// <returns>Result with errorCode.</returns>
        public async Task<RetrievalResult<User>> RetrieveUserAsync(int userId, [Optional] int? retrieveUserId,
            [Optional] string lastName, [Optional] string firstName, [Optional] string userName,
            [Optional] string email, [Optional] bool? active)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@retrieveUserId", retrieveUserId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@lastName", lastName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@firstName", firstName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@userName", userName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@email", email, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@active", active, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<User> users;

            using (var connection = new SqlConnection(_connectionString))
            {
                users = await connection.QueryAsync<User>("[proc_get_user_s]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");

            return new RetrievalResult<User>(errorCode, users);
        }

        /// <summary>
        /// Gets a single user by his username. This method does not provide 
        /// authorization checks!
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<RetrievalResult<User>> RetrieveUserByUserNameAsync(string userName)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userName", userName, DbType.StringFixedLength, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            IEnumerable<User> users;

            using (var connection = new SqlConnection(_connectionString))
            {
                users = await connection.QueryAsync<User>("[proc_get_user_by_username]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");

            return new RetrievalResult<User>(errorCode, users);
        }

        /// <summary>
        /// Updates a user given by the parameters.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> UpdateUserAsync(int userId, int updateUserId, [Optional] string lastName,
            [Optional] string firstName, [Optional] string userName, [Optional] string email,
            [Optional] byte[] passwordHash, [Optional] bool? active)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@updateUserId", updateUserId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@lastName", lastName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@firstName", firstName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@userName", userName, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@email", email, DbType.StringFixedLength, direction: ParameterDirection.Input);
            parameters.Add("@passwordHash", passwordHash, DbType.Binary, direction: ParameterDirection.Input);
            parameters.Add("@active", active, DbType.Boolean, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@updatedUsers", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_update_user]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var updatedUsers = parameters.Get<int>("updatedUsers");

            return new ManipulationResult(errorCode, new Operation(typeof(User), updatedUsers, OperationType.Update));
        }

        /// <summary>
        /// Deletes a user if the requesting user is a admin and 
        /// the user did not create any documents.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        public async Task<ManipulationResult> DeleteUserAsync(int userId, int deleteUserId)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@userId", userId, DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@deleteUserId", deleteUserId, DbType.Int32, direction: ParameterDirection.Input);

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedGroupMembers", DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@deletedUsers", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync("[proc_delete_user]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            var errorCode = parameters.Get<int>("errorCode");
            var deletedUsers = parameters.Get<int>("deletedUsers");
            var deletedGroupMembers = parameters.Get<int>("deletedGroupMembers");

            return new ManipulationResult(errorCode,
                new Operation(typeof(GroupMember), deletedGroupMembers, OperationType.Delete),
                new Operation(typeof(User), deletedUsers, OperationType.Delete));
        }
        #endregion

        #region System-Operations
        /// <summary>
        /// Deletes all the data from the database and resets the identity counters.
        /// This operation CANT BE REVERSED. Use with CAUTION!
        /// </summary>
        /// <returns>ErrorCode</returns>
        public int SysDeleteAndResetAllData()
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query("[critical_proc_delete_reset_all_data]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return parameters.Get<int>("errorCode");
        }

        /// <summary>
        /// Inserts test data for the integration tests.
        /// </summary>
        /// <returns>ErrorCode</returns>
        public int SysInsertSetupTestData()
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@errorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Query("[critical_proc_setup_test_data]", parameters,
                    commandType: CommandType.StoredProcedure);
            }

            return parameters.Get<int>("errorCode");
        }
        #endregion
    }
}