using System;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class ProviderFactoryTests
    {
        [Fact]
        public void KineticaProviderFactory_Instance_IsNotNull()
        {
            Assert.NotNull(KineticaProviderFactory.Instance);
        }

        [Fact]
        public void KineticaProviderFactory_CreateConnection_ReturnsKineticaConnection()
        {
            var factory = KineticaProviderFactory.Instance;
            var connection = factory.CreateConnection();

            Assert.IsType<KineticaConnection>(connection);
        }

        [Fact]
        public void KineticaProviderFactory_CreateCommand_ReturnsKineticaCommand()
        {
            var factory = KineticaProviderFactory.Instance;
            var command = factory.CreateCommand();

            Assert.IsType<KineticaCommand>(command);
        }

        [Fact]
        public void KineticaProviderFactory_CreateParameter_ReturnsKineticaParameter()
        {
            var factory = KineticaProviderFactory.Instance;
            var parameter = factory.CreateParameter();

            Assert.IsType<KineticaParameter>(parameter);
        }

        [Fact]
        public void KineticaProviderFactory_CreateConnectionStringBuilder_ReturnsBuilder()
        {
            var factory = KineticaProviderFactory.Instance;
            var builder = factory.CreateConnectionStringBuilder();

            Assert.NotNull(builder);
        }

        [Fact]
        public void KineticaProviderFactory_CanCreateDataSourceEnumerator_IsFalse()
        {
            var factory = KineticaProviderFactory.Instance;

            Assert.False(factory.CanCreateDataSourceEnumerator);
        }

        [Fact]
        public void KineticaProviderFactory_CreateDataSourceEnumerator_ThrowsNotSupported()
        {
            var factory = KineticaProviderFactory.Instance;

            Assert.Throws<NotSupportedException>(() => factory.CreateDataSourceEnumerator());
        }

        [Fact]
        public void KineticaProviderFactory_Instance_IsSingleton()
        {
            var instance1 = KineticaProviderFactory.Instance;
            var instance2 = KineticaProviderFactory.Instance;

            Assert.Same(instance1, instance2);
        }
    }
}
