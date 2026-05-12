using System;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class ConnectionStringBuilderTests
    {
        [Fact]
        public void DefaultValues_AreCorrect()
        {
            var builder = new KineticaConnectionStringBuilder();

            Assert.Equal("http://127.0.0.1:9191", builder.Server);
            Assert.Equal("", builder.Username);
            Assert.Equal("", builder.Password);
            Assert.Equal("", builder.OAuthToken);
            Assert.Null(builder.Database);
            Assert.Equal(30, builder.ConnectionTimeout);
            Assert.True(builder.Pooling);
            Assert.Equal(100, builder.MaxPoolSize);
            Assert.Equal(0, builder.MinPoolSize);
        }

        [Fact]
        public void ParseConnectionString_ParsesAllProperties()
        {
            var connectionString = "Server=192.168.1.100:9191;Username=admin;Password=secret;Database=mydb;Connection Timeout=60;Pooling=false;Max Pool Size=50;Min Pool Size=5";
            var builder = new KineticaConnectionStringBuilder(connectionString);

            Assert.Equal("192.168.1.100:9191", builder.Server);
            Assert.Equal("admin", builder.Username);
            Assert.Equal("secret", builder.Password);
            Assert.Equal("mydb", builder.Database);
            Assert.Equal(60, builder.ConnectionTimeout);
            Assert.False(builder.Pooling);
            Assert.Equal(50, builder.MaxPoolSize);
            Assert.Equal(5, builder.MinPoolSize);
        }

        [Fact]
        public void ParseConnectionString_HandlesOauthToken()
        {
            var connectionString = "Server=localhost:9191;OauthToken=my-oauth-token";
            var builder = new KineticaConnectionStringBuilder(connectionString);

            Assert.Equal("localhost:9191", builder.Server);
            Assert.Equal("my-oauth-token", builder.OAuthToken);
        }

        [Fact]
        public void SetProperties_UpdatesConnectionString()
        {
            var builder = new KineticaConnectionStringBuilder
            {
                Server = "myserver:9191",
                Username = "testuser",
                Password = "testpass"
            };

            var connectionString = builder.ToString();

            Assert.Contains("Server=myserver:9191", connectionString);
            Assert.Contains("Username=testuser", connectionString);
            Assert.Contains("Password=testpass", connectionString);
        }

        [Fact]
        public void ParseConnectionString_HandlesCaseInsensitiveKeys()
        {
            var connectionString = "server=myserver:9191;USERNAME=admin;PASSWORD=secret";
            var builder = new KineticaConnectionStringBuilder(connectionString);

            Assert.Equal("myserver:9191", builder.Server);
            Assert.Equal("admin", builder.Username);
            Assert.Equal("secret", builder.Password);
        }

        [Fact]
        public void ParseConnectionString_HandlesEmptyString()
        {
            var builder = new KineticaConnectionStringBuilder("");

            // Should use default values
            Assert.Equal("http://127.0.0.1:9191", builder.Server);
        }

        [Fact]
        public void ParseConnectionString_HandlesNullString()
        {
            var builder = new KineticaConnectionStringBuilder(null!);

            // Should use default values
            Assert.Equal("http://127.0.0.1:9191", builder.Server);
        }

        [Fact]
        public void ParseConnectionString_HandlesValuesWithEquals()
        {
            var connectionString = "Server=myserver:9191;Password=pass=word";
            var builder = new KineticaConnectionStringBuilder(connectionString);

            Assert.Equal("myserver:9191", builder.Server);
            Assert.Equal("pass=word", builder.Password);
        }
    }
}
