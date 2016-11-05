using NUnit.Framework;
using Cake.SqlServer;
using FluentAssertions;

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
    }
}
