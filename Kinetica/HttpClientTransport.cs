using System.Net;
using System.Net.Http.Headers;

namespace kinetica;

/// <summary>
/// <see cref="IHttpTransport"/> implementation backed by <see cref="HttpClient"/>
/// with <see cref="SocketsHttpHandler"/> for connection pooling and DNS refresh.
/// </summary>
internal sealed class HttpClientTransport : IHttpTransport, IDisposable
{
    private readonly HttpClient _client;
    private readonly bool _ownsClient;

    /// <summary>
    /// Creates a new HttpClientTransport with configurable timeout and connection pooling.
    /// </summary>
    /// <param name="timeout">HTTP request timeout (overall, per request)</param>
    /// <param name="pooledConnectionLifetime">Maximum lifetime of pooled connections (default: 2 minutes)</param>
    /// <param name="pooledConnectionIdleTimeout">Idle timeout for pooled connections (default: 2 minutes)</param>
    /// <param name="connectTimeout">
    /// Maximum time to establish a TCP connection to the server. Bounds the connection handshake
    /// specifically (e.g. when a host is unreachable/black-holed), independent of the overall
    /// <paramref name="timeout"/>. When <c>null</c>, the handler default (no connect-specific
    /// bound) is used and connection establishment is limited only by <paramref name="timeout"/>.
    /// </param>
    public HttpClientTransport(
        TimeSpan timeout,
        TimeSpan? pooledConnectionLifetime = null,
        TimeSpan? pooledConnectionIdleTimeout = null,
        TimeSpan? connectTimeout = null)
    {
        var handler = new SocketsHttpHandler
        {
            // Connection pooling with DNS refresh
            PooledConnectionLifetime    = pooledConnectionLifetime ?? TimeSpan.FromMinutes(2),
            PooledConnectionIdleTimeout = pooledConnectionIdleTimeout ?? TimeSpan.FromMinutes(2),

            // Kinetica-specific optimizations
            AutomaticDecompression      = DecompressionMethods.None,  // Kinetica uses Snappy
            UseCookies                  = false,                       // Not needed for API calls
            AllowAutoRedirect           = false,                       // API endpoints don't redirect
        };

        // Bound connection establishment when a connect timeout is supplied (e.g. from
        // Options.ServerConnectionTimeout). The SocketsHttpHandler default is infinite, so
        // without this an unreachable host is bounded only by the overall request timeout.
        if (connectTimeout.HasValue)
            handler.ConnectTimeout = connectTimeout.Value;

        _client = new HttpClient(handler, disposeHandler: true)
        {
            Timeout = timeout,
        };
        _ownsClient = true;
    }

    /// <summary>
    /// Injection constructor for tests. The caller owns the <paramref name="client"/>
    /// lifetime; this instance will not dispose it.
    /// </summary>
    internal HttpClientTransport(HttpClient client)
    {
        _client = client;
        _ownsClient = false;
    }

    /// <summary>
    /// Synchronous POST request.
    /// </summary>
    public byte[] Post(
        string url,
        byte[] body,
        string contentType,
        string? authorization,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        using var request = BuildRequest(url, body, contentType, authorization, userAgent);
        using var response = _client.Send(request, cancellationToken);
        return ReadOrThrow(response, cancellationToken);
    }

    /// <summary>
    /// Asynchronous POST request.
    /// </summary>
    public async Task<byte[]> PostAsync(
        string url,
        byte[] body,
        string contentType,
        string? authorization,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        using var request = BuildRequest(url, body, contentType, authorization, userAgent);
        using var response = await _client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        var bytes = await response.Content
            .ReadAsByteArrayAsync(cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
            return bytes;

        throw new KineticaTransportException((int)response.StatusCode, bytes);
    }

    private static HttpRequestMessage BuildRequest(
        string url,
        byte[] body,
        string contentType,
        string? authorization,
        string? userAgent)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new ByteArrayContent(body),
            // Force HTTP/1.1 to preserve existing behaviour — Kinetica servers
            // typically do not support HTTP/2 and mis-negotiation would break
            // the connection silently.
            Version = HttpVersion.Version11,
        };
        request.Content.Headers.ContentType  = MediaTypeHeaderValue.Parse(contentType);
        request.Content.Headers.ContentLength = body.Length;

        if (!string.IsNullOrEmpty(authorization))
        {
            var space = authorization.IndexOf(' ');
            if (space > 0)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    authorization[..space],
                    authorization[(space + 1)..]);
            }
            else
            {
                // Handle authorization without scheme (legacy compatibility)
                request.Headers.Add("Authorization", authorization);
            }
        }

        if (!string.IsNullOrEmpty(userAgent))
        {
            request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
        }

        return request;
    }

    private static byte[] ReadOrThrow(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        using var stream = response.Content.ReadAsStream(cancellationToken);
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        var bytes = buffer.ToArray();

        if (response.IsSuccessStatusCode)
            return bytes;

        // The Kinetica server encodes error bodies with the same Avro/JSON
        // envelope as success responses. Hand the raw bytes back to the
        // caller via KineticaTransportException so that SubmitRequestToUrlInternal
        // can decode the server's error message.
        throw new KineticaTransportException((int)response.StatusCode, bytes);
    }

    public void Dispose()
    {
        if (_ownsClient)
            _client.Dispose();
    }
}

/// <summary>
/// Thrown by <see cref="HttpClientTransport"/> when the server responds with
/// a non-2xx status code. The raw response body is preserved so that
/// <see cref="Kinetica"/> can decode the Kinetica error envelope.
/// </summary>
internal sealed class KineticaTransportException : Exception
{
    /// <summary>
    /// HTTP status code from the server response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Raw response body bytes (may contain Avro-encoded error message).
    /// </summary>
    public byte[] Body { get; }

    public KineticaTransportException(int statusCode, byte[] body)
        : base($"Kinetica server returned HTTP {statusCode}.")
    {
        StatusCode = statusCode;
        Body       = body;
    }
}
