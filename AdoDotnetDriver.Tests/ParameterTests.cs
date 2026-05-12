using System;
using System.Data;
using Xunit;
using KineticaAdo;

namespace AdoDotnetDriver.Tests
{
    public class ParameterTests
    {
        [Fact]
        public void KineticaParameter_DefaultValues()
        {
            var param = new KineticaParameter();

            Assert.Equal(DbType.String, param.DbType);
            Assert.Equal(ParameterDirection.Input, param.Direction);
            Assert.False(param.IsNullable);
            Assert.Equal(string.Empty, param.ParameterName);
            Assert.Equal(0, param.Size);
            Assert.Null(param.Value);
        }

        [Fact]
        public void KineticaParameter_SetProperties()
        {
            var param = new KineticaParameter
            {
                ParameterName = "@id",
                Value = 123,
                DbType = DbType.Int32,
                Direction = ParameterDirection.Input,
                IsNullable = true,
                Size = 4
            };

            Assert.Equal("@id", param.ParameterName);
            Assert.Equal(123, param.Value);
            Assert.Equal(DbType.Int32, param.DbType);
            Assert.Equal(ParameterDirection.Input, param.Direction);
            Assert.True(param.IsNullable);
            Assert.Equal(4, param.Size);
        }

        [Fact]
        public void KineticaParameter_ResetDbType()
        {
            var param = new KineticaParameter { DbType = DbType.Int32 };
            param.ResetDbType();

            Assert.Equal(DbType.String, param.DbType);
        }

        [Fact]
        public void KineticaParameterCollection_AddAndRetrieve()
        {
            var collection = new KineticaParameterCollection();
            var param1 = new KineticaParameter { ParameterName = "@id", Value = 1 };
            var param2 = new KineticaParameter { ParameterName = "@name", Value = "test" };

            collection.Add(param1);
            collection.Add(param2);

            Assert.Equal(2, collection.Count);
            Assert.True(collection.Contains("@id"));
            Assert.True(collection.Contains("@name"));
            Assert.Equal(0, collection.IndexOf("@id"));
            Assert.Equal(1, collection.IndexOf("@name"));
        }

        [Fact]
        public void KineticaParameterCollection_Remove()
        {
            var collection = new KineticaParameterCollection();
            var param = new KineticaParameter { ParameterName = "@id" };
            collection.Add(param);

            collection.Remove(param);

            Assert.Equal(0, collection.Count);
            Assert.False(collection.Contains("@id"));
        }

        [Fact]
        public void KineticaParameterCollection_Clear()
        {
            var collection = new KineticaParameterCollection();
            collection.Add(new KineticaParameter { ParameterName = "@id" });
            collection.Add(new KineticaParameter { ParameterName = "@name" });

            collection.Clear();

            Assert.Equal(0, collection.Count);
        }

        [Fact]
        public void KineticaParameterCollection_RemoveAt()
        {
            var collection = new KineticaParameterCollection();
            collection.Add(new KineticaParameter { ParameterName = "@id" });
            collection.Add(new KineticaParameter { ParameterName = "@name" });

            collection.RemoveAt(0);

            Assert.Equal(1, collection.Count);
            Assert.False(collection.Contains("@id"));
            Assert.True(collection.Contains("@name"));
        }

        [Fact]
        public void KineticaParameterCollection_RemoveAtByName()
        {
            var collection = new KineticaParameterCollection();
            collection.Add(new KineticaParameter { ParameterName = "@id" });
            collection.Add(new KineticaParameter { ParameterName = "@name" });

            collection.RemoveAt("@id");

            Assert.Equal(1, collection.Count);
            Assert.False(collection.Contains("@id"));
        }

        [Fact]
        public void KineticaParameterCollection_Insert()
        {
            var collection = new KineticaParameterCollection();
            collection.Add(new KineticaParameter { ParameterName = "@first" });
            collection.Add(new KineticaParameter { ParameterName = "@last" });

            collection.Insert(1, new KineticaParameter { ParameterName = "@middle" });

            Assert.Equal(3, collection.Count);
            Assert.Equal(1, collection.IndexOf("@middle"));
        }

        [Fact]
        public void KineticaParameterCollection_AddRange()
        {
            var collection = new KineticaParameterCollection();
            var params_ = new[]
            {
                new KineticaParameter { ParameterName = "@p1" },
                new KineticaParameter { ParameterName = "@p2" },
                new KineticaParameter { ParameterName = "@p3" }
            };

            collection.AddRange(params_);

            Assert.Equal(3, collection.Count);
        }
    }
}
