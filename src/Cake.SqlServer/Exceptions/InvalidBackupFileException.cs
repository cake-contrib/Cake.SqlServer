using System;
using System.Diagnostics.CodeAnalysis;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class InvalidBackupFileException
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
        : Exception
    {
        public InvalidBackupFileException()
        {
        }

        public InvalidBackupFileException(string message)
            : base(message)
        {
        }

        public InvalidBackupFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
