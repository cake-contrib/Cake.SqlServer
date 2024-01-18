using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class InstanceVersionUnknownException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
        : Exception
    {
        public InstanceVersionUnknownException()
        {
        }

        public InstanceVersionUnknownException(string message)
            : base(message)
        {
        }

        public InstanceVersionUnknownException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
