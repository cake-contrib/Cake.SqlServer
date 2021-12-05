using System;
using Cake.SqlServer;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    public class GuardTests
    {
        [TestCase((string?)null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("\r\n")]
        public void String_Empty_ThrowsException(string? value)
        {
            // Act
            Action act = () => Guard.ArgumentIsNotNull(value!, nameof(value));

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NullObject_Throws_Exception()
        {
            // Arrange
            const object? value = null;

            Action act = () => Guard.ArgumentIsNotNull(value!, "value");

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
