using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InstanceNameEmptyException
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

        protected InstanceNameEmptyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
