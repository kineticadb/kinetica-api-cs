namespace kinetica;

/// <summary>The exception thrown by the Kinetica client when a request fails or the server returns an error.</summary>
public class KineticaException : System.Exception
{
    /// <summary>
    /// HTTP status code if this exception originated from an HTTP error response.
    /// Null if the exception is not HTTP-related.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>Initializes a new exception with no message.</summary>
    public KineticaException() { }

    /// <summary>Initializes a new exception with the given error message.</summary>
    /// <param name="msg">The error message.</param>
    public KineticaException(string msg) : base ( msg ) { }

    /// <summary>Initializes a new exception with the given message and inner exception.</summary>
    /// <param name="msg">The error message.</param>
    /// <param name="innerException">The exception that caused this one.</param>
    public KineticaException( string msg, Exception innerException ) :
        base( msg, innerException ) { }

    /// <summary>
    /// Creates a KineticaException with an HTTP status code.
    /// </summary>
    /// <param name="msg">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="innerException">Inner exception (optional)</param>
    public KineticaException(string msg, int? statusCode, Exception? innerException = null)
        : base(msg, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>Returns the exception's message (provided for Java/C++ API parity).</summary>
    public string what() { return Message; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var baseMsg = "KineticaException: " + Message;
        return StatusCode.HasValue ? $"{baseMsg} (HTTP {StatusCode})" : baseMsg;
    }
}
