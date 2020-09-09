using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SquareDMS.CacheAccess
{
    /// <summary>
    /// Implementation of the
    /// cache access for redis.
    /// </summary>
    public class SquareCacheRedis : ISquareCache
    {
        /// <summary>
        /// The connection Multiplexer obj is comlex and needs to be stored.
        /// </summary>
        private ConnectionMultiplexer _redisCon;

        private readonly string _server;
        private readonly int _port;

        /// <summary>
        /// Instantiates a new Redis cache. 
        /// Uses server and port to connect to it,
        /// when the method is called.
        /// </summary>
        public SquareCacheRedis(string server, int port)
        {
            _server = server;
            _port = port;
        }

        /// <summary>
        /// Connects to the redis server.
        /// </summary>
        /// <exception cref="Exception">Error while connecting to the cache server</exception>
        public async Task ConnectAsync()
        {
            try
            {
                _redisCon = await ConnectionMultiplexer.ConnectAsync($"{_server}:{_port}");
            }
            catch (Exception ex)
            {
                // log connection error
                throw;
            }
        }

        /// <summary>
        /// Closes the connection to redis. (Allows pending operations to be finished)
        /// </summary>
        public async Task DisconnectAsync()
        {
            await _redisCon.CloseAsync(true);
        }

        /// <summary>
        /// Gets the payload by the given docVersionId.
        /// </summary>
        /// <returns>Returns null if no value has been found.</returns>
        public async Task<byte[]> RetrieveDocumentVersionPayloadAsync(int docVersionId)
        {
            var redisDb = _redisCon.GetDatabase();

            return await redisDb.StringGetAsync(docVersionId.ToString());
        }

        /// <summary>
        /// Puts a payload in the cache.
        /// </summary>
        public async Task<bool> PutDocumentPayloadAsync(int docVersionId, byte[] payload)
        {
            var redisDb = _redisCon.GetDatabase();

            return await redisDb.StringSetAsync(docVersionId.ToString(), payload);
        }

        /// <summary>
        /// Deletes the payload from the cache.
        /// </summary>
        public async Task<bool> DeleteDocumentPayloadAsync(int docVersionId)
        {
            var redisDb = _redisCon.GetDatabase();

            return await redisDb.KeyDeleteAsync(docVersionId.ToString());
        }
    }
}