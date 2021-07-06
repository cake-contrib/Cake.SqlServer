using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class LocalDBExecutionFailedException
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

        protected LocalDBExecutionFailedException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
