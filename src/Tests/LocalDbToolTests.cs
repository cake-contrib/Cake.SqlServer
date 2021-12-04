using Cake.SqlServer;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    public class LocalDbToolTests
    {
        [Test]
        public void V12_Create_CorrectParameters()
        {
            // Arrange
            var fixture = new LocalDbToolRunnerFixture
            {
                Action = LocalDbAction.Create,
                Version = LocalDbVersion.V12,
                InstanceName = "Cake-Test",
            };

            // Act
            var result = fixture.Run();

            // Assert
            result.Args.Should().Be(@"create ""Cake-Test"" 12.0 -s");
        }

        [Test]
        public void V11_Create_CorrectParameters()
        {
            // Arrange
            var fixture = new LocalDbToolRunnerFixture
            {
                Action = LocalDbAction.Create,
                Version = LocalDbVersion.V11,
                InstanceName = "Cake-Test",
            };

            // Act
            var result = fixture.Run();

            // Assert
            result.Args.Should().Be(@"create ""Cake-Test"" 11.0 -s");
        }

        [Test]
        public void Delete_Instance_Deletes()
        {
            // Arrange
            var fixture = new LocalDbToolRunnerFixture
            {
                Action = LocalDbAction.Delete,
                InstanceName = "Cake-Test",
                Version = LocalDbVersion.V12,
            };

            // Act
            var result = fixture.Run();

            // Assert
            result.Args.Should().Be(@"delete ""Cake-Test""");
        }

        [Test]
        public void Start_Instance_Starts()
        {
            // Arrange
            var fixture = new LocalDbToolRunnerFixture
            {
                Action = LocalDbAction.Start,
                InstanceName = "Cake-Test",
                Version = LocalDbVersion.V12,
            };

            // Act
            var result = fixture.Run();

            // Assert
            result.Args.Should().Be(@"start ""Cake-Test""");
        }

        [Test]
        public void Stop_Instance()
        {
            // Arrange
            var fixture = new LocalDbToolRunnerFixture
            {
                Action = LocalDbAction.Stop,
                InstanceName = "Cake-Test",
                Version = LocalDbVersion.V12,
            };

            // Act
            var result = fixture.Run();

            // Assert
            result.Args.Should().Be(@"stop ""Cake-Test""");
        }
    }
}
