using SquareDMS.DatabaseAccess;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.Core.Dispatchers
{
    /// <summary>
    /// forwards fileFormat obj.
    /// </summary>
    public class FileFormatDispatcher : Dispatcher
    {
        private readonly ISquareDb _squareDb;

        /// <summary>
        /// 
        /// </summary>
        public FileFormatDispatcher(ISquareDb squareDb)
        {
            _squareDb = squareDb;
        }

        #region CRUD-Operations
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateFileFormatAsync(int userId, FileFormat fileFormat)
        {
            return await _squareDb.CreateFileFormatAsync(userId, fileFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<FileFormat>> RetrieveFileFormatsAsync(int userId, int? id,
            string extension, string description)
        {
            return await _squareDb.RetrieveFileFormatsAsync(userId, id, extension, description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateFileFormatAsync(int userId, int id, FileFormat patchedFileFormat)
        {
            return await _squareDb.UpdateFileFormatAsync(userId, id,
                patchedFileFormat.Extension, patchedFileFormat.Description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteFileFormatAsync(int userId, int id)
        {
            return await _squareDb.DeleteFileFormatAsync(userId, id);
        }
        #endregion
    }
}
