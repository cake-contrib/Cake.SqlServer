using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class InstanceNameEmptyException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
        : Exception
    {
        public InstanceNameEmptyException()
        {
        }

        public InstanceNameEmptyException(string message)
            : base(message)
        {
        }

        public InstanceNameEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
