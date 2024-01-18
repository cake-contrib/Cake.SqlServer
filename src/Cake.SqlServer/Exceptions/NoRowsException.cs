using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class NoRowsException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
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
    }
}
