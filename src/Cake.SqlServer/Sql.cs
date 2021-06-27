#if NET5_0
using System;
#endif
using System.Text;

namespace Cake.SqlServer
{
    //Idea is taken from here: https://gist.github.com/jnm2/9664d7bb31b8ea1933b5412c0811858d
    internal static class Sql
    {
        /// <summary>
        /// Performance-optimized SQL-safe name.
        /// </summary>
        /// <param name="name">Name to escape.</param>
        internal static string EscapeName(string? name)
        {
            var sb = new StringBuilder();
            sb.Append('[');

#if NET5_0
            var lastQuote = name?.IndexOf(']', StringComparison.OrdinalIgnoreCase) ?? -1;
#else
            var lastQuote = name?.IndexOf(']') ?? -1;
#endif
            if (lastQuote == -1)
            {
                return sb.Append(name).Append(']').ToString();
            }

            sb.Append(name, 0, lastQuote + 1);

            while (true)
            {
                var nextQuote = name?.IndexOf(']', lastQuote + 1) ?? -1;
                if (nextQuote == -1)
                {
                    break;
                }
                sb.Append(name, lastQuote, nextQuote - lastQuote + 1);
                lastQuote = nextQuote;
            }

            return sb.Append(name, lastQuote, (name?.Length ?? 0) - lastQuote).Append(']').ToString();
        }



        internal static string EscapeNameQuotes(string name)
        {
#if NET5_0
            var result = name.Replace("'", "''", StringComparison.OrdinalIgnoreCase);
#else
            var result = name.Replace("'", "''");
#endif

            return $"'{result}'";
        }
    }
}
