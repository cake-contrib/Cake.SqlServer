using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Cake.SqlServer
{
    internal static class Guard
    {
        [DebuggerHidden]
        internal static void ArgumentIsNotNull(object value, string argumentName)
        {
            if (value is String)
            {
                var stringValue = value as String;
                if (String.IsNullOrWhiteSpace(stringValue))
                {
                    throw new ArgumentNullException(argumentName);
                }
            }

            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
