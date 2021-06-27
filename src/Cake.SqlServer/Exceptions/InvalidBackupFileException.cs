using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InvalidBackupFileException
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

        protected InvalidBackupFileException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
