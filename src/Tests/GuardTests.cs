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
#pragma warning disable CS0436 // Type conflicts with imported type
            Action act = () => Guard.ArgumentIsNotNull(value, "value");
#pragma warning restore CS0436 // Type conflicts with imported type

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NullObject_Throws_Exception()
        {
            //Arrange
            object value = null;

#pragma warning disable CS0436 // Type conflicts with imported type
            Action act = () => Guard.ArgumentIsNotNull(value, "value");
#pragma warning restore CS0436 // Type conflicts with imported type

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
