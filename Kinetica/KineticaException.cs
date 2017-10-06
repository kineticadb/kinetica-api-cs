using System;
using System.Runtime.Serialization;


namespace kinetica
{
    [Serializable()]
    public class KineticaException : System.Exception
    {
        private string message;

        public KineticaException() { }

        public KineticaException(string msg) : base ( msg ) { }

        public KineticaException( string msg, Exception innerException ) :
            base( msg, innerException ) { }

        protected KineticaException( SerializationInfo info, StreamingContext context )
            : base ( info, context ) { }

        public string what() { return message; }

        public override string ToString() { return "KineticaException: " + message; }
    }
}
