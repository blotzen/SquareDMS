using SquareDMS.DatabaseAccess;
using SquareDMS.DatabaseAccess.ProcedureResults;
using System.Threading.Tasks;
using Models = SquareDMS.Core.Models;

namespace SquareDMS.Core.Dispatchers
{
    /// <summary>
    /// forwards fileFormat obj.
    /// </summary>
    public class FileFormatDispatcher : Dispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileForamtId"></param>
        /// <param name="patchedFileFormat"></param>
        /// <returns></returns>
        public async Task<ManipulationResult> PatchFileFormatAsync(int userId, int fileForamtId, Models::FileFormat patchedFileFormat)
        {
            ISquareDb squareDb = new SquareDbMsSql(base.DbConnectionString);

            return await squareDb.UpdateFileFormatAsync(userId, fileForamtId,
                patchedFileFormat.Extension, patchedFileFormat.Description);
        }
    }
}
