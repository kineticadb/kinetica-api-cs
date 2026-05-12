using System.Threading;
using System.Threading.Tasks;

namespace kinetica;

/// <summary>
/// Abstraction over the raw HTTP POST layer. Default implementation uses
    /// <see cref="HttpClientTransport"/>; tests can inject a fake.
    /// </summary>
    internal interface IHttpTransport
    {
        /// <summary>
        /// Synchronous POST. Blocks the calling thread until the response is received.
        /// </summary>
        /// <param name="url">Full URL to POST to</param>
        /// <param name="body">Request body bytes</param>
        /// <param name="contentType">Content-Type header value</param>
        /// <param name="authorization">Authorization header value (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response body bytes</returns>
        byte[] Post(string url, byte[] body, string contentType, string? authorization, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronous POST. Returns a task that completes when the response is received.
        /// <para>Cancellation is honoured at the socket level — the returned task throws
        /// <see cref="OperationCanceledException"/> when the token fires.</para>
        /// </summary>
        /// <param name="url">Full URL to POST to</param>
        /// <param name="body">Request body bytes</param>
        /// <param name="contentType">Content-Type header value</param>
        /// <param name="authorization">Authorization header value (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task that returns response body bytes</returns>
        Task<byte[]> PostAsync(string url, byte[] body, string contentType, string? authorization, CancellationToken cancellationToken);
    }
