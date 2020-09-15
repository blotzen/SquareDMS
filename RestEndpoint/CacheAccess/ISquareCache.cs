using System;
using System.Threading.Tasks;

namespace SquareDMS.CacheAccess
{
    /// <summary>
    /// This interface contains all the necessary Methods
    /// for interaction with the Cache layer.
    /// </summary>
    public interface ISquareCache
    {
        /// <summary>
        /// Connects to the redis server.
        /// </summary>
        /// <exception cref="Exception">Error while connecting to the cache server</exception>
        Task ConnectAsync();

        /// <summary>
        /// Closes the connection to redis. (Allows pending operations to be finished)
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Gets the payload for the given document Version id 
        /// in the cache layer.
        /// Returns null if it is not present in the cache.
        /// </summary>
        Task<byte[]> RetrieveDocumentVersionPayloadAsync(int docVersionId);

        /// <summary>
        /// Puts the given payload in the cache. Uses the docVersionId as Id.
        /// If the given Id is already present in the cache, the payload will
        /// be replaced.
        /// </summary>
        Task<bool> PutDocumentPayloadAsync(int docVersionId, byte[] payload);

        /// <summary>
        /// Deletes the payload from the cache if its present.
        /// </summary>
        Task<bool> DeleteDocumentPayloadAsync(int docVersionId);
    }
}