using Cake.SqlServer;
using NUnit.Framework;
using System;
using FluentAssertions;

namespace Tests
{
    public class GuardTests
    {
        [TestCase((String)null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t")]
        [TestCase("\r\n")]
        public void String_Empty_ThrowsException(String value)
        {
            // Act
            Action act = () => Guard.ArgumentIsNotNull(value, "value");

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void NullObject_Throws_Exception()
        {
            //Arrange
            object value = null;

            Action act = () => Guard.ArgumentIsNotNull(value, "value");

            // Assert
            act.ShouldThrow<ArgumentNullException>();
        }
    }
}
