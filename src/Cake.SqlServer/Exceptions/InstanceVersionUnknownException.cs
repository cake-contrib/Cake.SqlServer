using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cake.SqlServer
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InstanceVersionUnknownException
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

        protected InstanceVersionUnknownException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}
