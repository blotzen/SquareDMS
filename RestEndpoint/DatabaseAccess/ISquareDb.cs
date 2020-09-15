using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SquareDMS.DatabaseAccess
{
    /// <summary>
    /// This Interface contains all the necessary Methods
    /// for interaction with the database. This interface is 
    /// exposed by the Data Access Layer.
    /// </summary>
    public interface ISquareDb
    {

        #region Document-Operations
        /// <summary>
        /// Creates a document. Checks if the user is a Creator and therefore
        /// authorized to create a document.
        /// </summary>
        /// <param name="doc">Document to be created.</param>
        /// <returns>Result contains an Errorcode that is 0 if the operation
        /// succeeded.</returns>
        /// <exception cref="ArgumentNullException">Doc cant be null.</exception>
        Task<ManipulationResult> CreateDocumentAsync(Document doc);

        /// <summary>
        /// Gets a document or multiple documents, depending on the given paramters.
        /// </summary>
        /// <returns>The documents and a errorCode.</returns>
        Task<RetrievalResult<Document>> RetrieveDocumentsAsync(int userId, [Optional] int? maxAccessLevel,
            [Optional] int? docId, [Optional] int? creator, [Optional] int? docType,
            [Optional] string name, [Optional] bool? locked, [Optional] bool? discard);

        /// <summary>
        /// Updates a document. Locked doc can only be updated by the creator. An empty doc name is not permitted.
        /// </summary>
        /// <returns></returns>
        Task<ManipulationResult> UpdateDocumentAsync(int userId, int docId, [Optional] int? docType,
            [Optional] string name, [Optional] bool? locked, [Optional] bool? discard);

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
        Task<ManipulationResult> DeleteDocumentAsync(int userId, int docId);
        #endregion

        #region DocumentType-Operations
        /// <summary>
        /// Creates a document type. User has to be admin
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">DocType cant be null.</exception>
        Task<ManipulationResult> CreateDocumentTypeAsync(int userId, DocumentType docType);

        /// <summary>
        /// Gets one or multiple DocumentTypes depending on the param config.
        /// Admin and Users can retrieve all doc types.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<RetrievalResult<DocumentType>> RetrieveDocumentTypeAsync(int userId, [Optional] int? docTypeId,
           [Optional] string name, [Optional] string description);

        /// <summary>
        /// Updates a Document Type. Only admin can update.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> UpdateDocumentTypeAsync(int userId, int docTypeId,
           [Optional] string name, [Optional] string description);

        /// <summary>
        /// Deletes a Document Type. Only admin can delete and only 
        /// if no document uses the type currently.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteDocumentTypeAsync(int userId, int docTypeId);
        #endregion

        #region DocumentVersion-Operations
        /// <summary>
        /// Creates a new DocumentVersion. Uses the byte[] and filestream with System.IO.
        /// to place the file in the filestream area. Uses the SQL Server file handle.
        /// </summary>
        /// <returns>Return value with error Code</returns>
        /// <exception cref="Exception"></exception>
        Task<ManipulationResult> CreateDocumentVersionAsync(int userId, DocumentVersion docVersion);

        /// <summary>
        /// Gets a specific document version metadata.
        /// </summary>
        /// <returns>Return value with error Code</returns>
        Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionAsync(int userId, int docVerId);

        /// <summary>
        /// Gets the document versions for the given parameter, checks the permissions.
        /// Does not return the payload. This has to be done with <see cref="RetrieveDocumentVersionAsync(int, int)"/>
        /// </summary>
        /// <returns>Result with errorCode</returns>
        Task<RetrievalResult<DocumentVersion>> RetrieveDocumentVersionsMetaDataAsync(int userId, [Optional] int? docVerId, [Optional] int? docId);
        #endregion

        #region FileFormat-Operations
        /// <summary>
        /// Creates a new FileFormat
        /// </summary>
        /// <returns>Return value with error Code</returns>
        /// <exception cref="ArgumentNullException">FileFormat cant be null.</exception>
        Task<ManipulationResult> CreateFileFormatAsync(int userId, FileFormat fileFormat);

        /// <summary>
        /// Gets one or multiple FileFormats depending on the paramters.
        /// </summary>
        /// <returns>Collection of FileFormats and ErrorCode</returns>
        Task<RetrievalResult<FileFormat>> RetrieveFileFormatsAsync(int userId, [Optional] int? fileFormatId,
           [Optional] string extension, [Optional] string description);

        /// <summary>
        /// Updates a FileFromat if the user is admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> UpdateFileFormatAsync(int userId, int fileFormatId,
           [Optional] string extension, [Optional] string description);

        /// <summary>
        /// Deletes a file format, file format cant be used by any doc version.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteFileFormatAsync(int userId, int fileFormatId);
        #endregion

        #region Group-Operations
        /// <summary>
        /// Creates a Group. Only admins can.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Group cant be null.</exception>
        Task<ManipulationResult> CreateGroupAsync(int userId, Group group);

        /// <summary>
        /// Gets one or multiple Groups depending on the given params.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<RetrievalResult<Group>> RetrieveGroupAsync(int userId, [Optional] int? groupId,
           [Optional] string name, [Optional] string description, [Optional] bool? admin, [Optional] bool? creator);

        /// <summary>
        /// Updates a group. User has to be admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> UpdateGroupAsync(int userId, int groupId, [Optional] string name,
           [Optional] string description, [Optional] bool? admin, [Optional] bool? creator);

        /// <summary>
        /// Deletes Group if it has no Members and no Rights use it. 
        /// Only Admins can delete Groups.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteGroupAsync(int userId, int groupId);
        #endregion

        #region GroupMember-Operations
        /// <summary>
        /// Creates a Group Member. Possible if user is admin and the user isnt member of the 
        /// group already.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        /// <exception cref="ArgumentNullException">GroupMember cant be null.</exception>
        Task<ManipulationResult> CreateGroupMemberAsync(int userId, GroupMember groupMember);

        /// <summary>
        /// Gets one or more Group Members depending on the given paramters.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<RetrievalResult<GroupMember>> RetrieveGroupMemberAsync(int userId, [Optional] int? groupId,
           [Optional] int? memberId);

        /// <summary>
        /// Deletes a group Member if the user is admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteGroupMemberAsync(int userId, int groupId, int memberId);
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
        Task<ManipulationResult> CreateRightAsync(int userId, Right right);

        /// <summary>
        /// Gets one or more Rights depending on the given params.
        /// </summary>
        /// <returns>Collection of Rights and ErrorCode</returns>
        Task<RetrievalResult<Right>> RetrieveRightsAsync(int userId, [Optional] int? groupId,
           [Optional] int? docId);

        /// <summary>
        /// Updates a Right. Right has to exist and user has to be Admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> UpdateRightAsync(int userId, int groupId,
           int docId, AccessLevel accessLevel);

        /// <summary>
        /// Deletes one or more Rights. User has to be Admin.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteRightsAsync(int userId, int groupId, int docId);
        #endregion

        #region User-Operations
        /// <summary>
        /// Creates a user. Only admins are allowed to.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        /// <exception cref="ArgumentNullException">Right cant be null.</exception>
        Task<ManipulationResult> CreateUserAsync(int userId, User user);

        /// <summary>
        /// Gets one or multiple users depending on the paramters. If user 
        /// is no admin, he can only see himself.
        /// </summary>
        /// <param name="userId">The user that issues the request.</param>
        /// <param name="retrieveUserId">Id of the user that is being looked for.</param>
        /// <returns>Result with errorCode.</returns>
        Task<RetrievalResult<User>> RetrieveUserAsync(int userId, [Optional] int? retrieveUserId,
           [Optional] string lastName, [Optional] string firstName, [Optional] string userName,
           [Optional] string email, [Optional] bool? active);

        /// <summary>
        /// Gets a single user by his username. This method does not provide 
        /// authorization checks!
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<RetrievalResult<User>> RetrieveUserByUserNameAsync(string userName);

        /// <summary>
        /// Updates a user given by the parameters.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> UpdateUserAsync(int userId, int updateUserId, [Optional] string lastName,
           [Optional] string firstName, [Optional] string userName, [Optional] string email,
           [Optional] byte[] passwordHash, [Optional] bool? active);

        /// <summary>
        /// Deletes a user if the requesting user is a admin and 
        /// the user did not create any documents.
        /// </summary>
        /// <returns>Result with errorCode.</returns>
        Task<ManipulationResult> DeleteUserAsync(int userId, int deleteUserId);
        #endregion

        #region System-Operations
        /// <summary>
        /// Deletes all the data from the database and resets the identity counters.
        /// This operation CANT BE REVERSED. Use with CAUTION!
        /// </summary>
        /// <returns>ErrorCode</returns>
        public int SysDeleteAndResetAllData();

        /// <summary>
        /// Inserts test data for the integration tests.
        /// </summary>
        /// <returns>ErrorCode</returns>
        public int SysInsertSetupTestData();
        #endregion
    }
}
