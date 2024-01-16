using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class LocalDBExecutionFailedException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
        : Exception
    {
        public LocalDBExecutionFailedException()
        {
        }

        public LocalDBExecutionFailedException(string message)
            : base(message)
        {
        }

        public LocalDBExecutionFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
