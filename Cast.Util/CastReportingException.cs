using System;
using System.Runtime.Serialization;

namespace Cast.Util
{
    [SerializableAttribute]
    public class CastReportingException : Exception
    {
        public CastReportingException(string message): base(message) { }
        public CastReportingException(string message, Exception innerException): base(message, innerException) { }

        protected CastReportingException(SerializationInfo info, StreamingContext context): base(info, context) { }
    }
}
