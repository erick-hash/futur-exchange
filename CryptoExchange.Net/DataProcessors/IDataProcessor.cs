using CryptoExchange.Net.Objects;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoExchange.Net.DataProcessors
{
    /// <summary>
    /// Interface for a data converter. The converter is responsible for serializing/deserializing data between the client and server.
    /// </summary>
    public interface IDataConverter
    {
        /// <summary>
        /// Deserialize data directly from a stream into a model
        /// </summary>
        /// <typeparam name="T">Model type to deserialize into</typeparam>
        /// <param name="id">Request/Connection id</param>
        /// <param name="dataStream">The data stream</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<CallResult<T>> DeserializeAsync<T>(int id, Stream dataStream, CancellationToken ct);
        /// <summary>
        /// Deserialize a data string into a model
        /// </summary>
        /// <typeparam name="T">Model type to deserialize into</typeparam>
        /// <param name="id">Request/Connection id</param>
        /// <param name="dataString">The data string</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<CallResult<T>> DeserializeAsync<T>(int id, string dataString, CancellationToken ct);
        /// <summary>
        /// Deserialize a data string into a model
        /// </summary>
        /// <typeparam name="T">Model type to deserialize into</typeparam>
        /// <param name="id">Request/Connection id</param>
        /// <param name="dataString">The data string</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        CallResult<T> Deserialize<T>(int id, string dataString, CancellationToken ct);

        /// <summary>
        /// Serialize a model to string
        /// </summary>
        /// <typeparam name="T">The model being serialized</typeparam>
        /// <param name="id">Request/Connection id</param>
        /// <param name="data">The data</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        CallResult<string> Serialize<T>(int id, T data, CancellationToken ct);

        /// <summary>
        /// Serialize a model to string
        /// </summary>
        /// <typeparam name="T">The model being serialized</typeparam>
        /// <param name="id">Request/Connection id</param>
        /// <param name="data">The data</param>
        /// <param name="ct">Cancelation token</param>
        /// <returns></returns>
        Task<CallResult<string>> SerializeAsync<T>(int id, T data, CancellationToken ct);
    }
}
