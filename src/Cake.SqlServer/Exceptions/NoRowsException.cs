using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class NoRowsException
        : Exception
    {
        public NoRowsException()
        {
        }

        public NoRowsException(string message)
            : base(message)
        {
        }

        public NoRowsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoRowsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
