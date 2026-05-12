using System;


namespace kinetica;

public class KineticaException : System.Exception
    {
        /// <summary>
        /// HTTP status code if this exception originated from an HTTP error response.
        /// Null if the exception is not HTTP-related.
        /// </summary>
        public int? StatusCode { get; }

        public KineticaException() { }

        public KineticaException(string msg) : base ( msg ) { }

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

        public string what() { return Message; }

        public override string ToString()
        {
            var baseMsg = "KineticaException: " + Message;
            return StatusCode.HasValue ? $"{baseMsg} (HTTP {StatusCode})" : baseMsg;
        }
    }
