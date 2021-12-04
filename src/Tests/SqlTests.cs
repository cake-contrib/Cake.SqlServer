using Cake.SqlServer;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    public class SqlTests
    {
        [TestCase("hello", "[hello]")]
        [TestCase("hello]", "[hello]]]")]
        [TestCase("[hello]", "[[hello]]]")]
        [TestCase("he]]o", "[he]]]]o]")]
        [TestCase("][][][]", "[]][]][]][]]]")]
        public void EscapeName_Doubles_Brackets(string incoming, string expected)
        {
            var result = Sql.EscapeName(incoming);

            result.Should().Be(expected);
        }

        [TestCase("hello", "'hello'")]
        [TestCase("hello'", "'hello'''")]
        [TestCase("'hello'", "'''hello'''")]
        [TestCase("hell'o", "'hell''o'")]
        [TestCase("hell''o", "'hell''''o'")]
        public void EscapeNameQuotes_Doubles_Quotes(string incoming, string expected)
        {
            var result = Sql.EscapeNameQuotes(incoming);

            result.Should().Be(expected);
        }
    }
}
