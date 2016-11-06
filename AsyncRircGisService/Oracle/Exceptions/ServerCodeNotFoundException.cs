using System;
using System.Runtime.Serialization;
namespace AsyncRircGisService.Oracle.Exceptions
{
    [Serializable]
    public class ServerCodeNotFoundException : Exception
    {
        public ServerCodeNotFoundException()
        {
        }

        public ServerCodeNotFoundException( string message ) : base( message )
        {
        }

        public ServerCodeNotFoundException( string message, Exception inner ) : base( message, inner )
        {
        }

        protected ServerCodeNotFoundException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}