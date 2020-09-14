﻿using SquareDMS.Core.Dispatchers;
using SquareDMS.DataLibrary.Entities;
using SquareDMS.DataLibrary.ProcedureResults;
using System.Threading.Tasks;

namespace SquareDMS.RestEndpoint.Services
{
    /// <summary>
    /// This services provides the functionality 
    /// for the fileFormatController.
    /// </summary>
    public class FileFormatService
    {
        private readonly FileFormatDispatcher _fileFormatDispatcher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileFormatDispatcher"></param>
        public FileFormatService(FileFormatDispatcher fileFormatDispatcher)
        {
            _fileFormatDispatcher = fileFormatDispatcher;
        }

        #region CRUD-Operationen
        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> CreateFileFormatAsync(int userId, FileFormat fileFormat)
        {
            return await _fileFormatDispatcher.CreateFileFormatAsync(userId, fileFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<RetrievalResult<FileFormat>> RetrieveFileFormatsAsync(int userId, int? id,
            string extension, string description)
        {
            return await _fileFormatDispatcher.RetrieveFileFormatsAsync(userId, id,
                extension, description);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> UpdateFileFormatAsync(int userId, int id,
            FileFormat patchedFileFormat)
        {
            return await _fileFormatDispatcher.UpdateFileFormatAsync(userId, id, patchedFileFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<ManipulationResult> DeleteFileFormatAsync(int userId, int id)
        {
            return await _fileFormatDispatcher.DeleteFileFormatAsync(userId, id);
        }
        #endregion
    }
}