/// <summary>
/// Schema Builder Demo
///
/// This example demonstrates different approaches for defining Kinetica
/// record schemas in C#:
///   1. Using KineticaType with column builders
///   2. Using SQL CREATE TABLE statements
///   3. Using Avro schema JSON directly
///
/// Run with:
///   dotnet run --project Example -- --schema-demo
/// </summary>

using System;
using System.Collections.Generic;
using kinetica;

namespace Example
{
    public static class SchemaBuilderDemo
    {
        public static void Run()
        {
            Console.WriteLine("=== Schema Builder Demo ===\n");

            // Example 1: Using KineticaType with column builders
            Console.WriteLine("1. KineticaType with Column Builders:");
            Console.WriteLine("   ---------------------------------");

            var simpleType = new KineticaType(
                "SimpleRecord",
                new List<KineticaType.Column>
                {
                    new("id", KineticaType.Column.ColumnType.INT, new List<string> { ColumnProperty.PRIMARY_KEY }),
                    new("name", KineticaType.Column.ColumnType.STRING, null),
                    new("value", KineticaType.Column.ColumnType.DOUBLE, null),
                }
            );

            Console.WriteLine($"   Schema: {simpleType.getSchemaString()}");
            Console.WriteLine();

            // Example 2: Complex record with nullable fields and various types
            Console.WriteLine("2. Complex Record with Nullable Fields:");
            Console.WriteLine("   ------------------------------------");

            var sensorType = new KineticaType(
                "SensorReading",
                new List<KineticaType.Column>
                {
                    new("sensor_id", KineticaType.Column.ColumnType.STRING,
                        new List<string> { ColumnProperty.PRIMARY_KEY, ColumnProperty.CHAR64 }),
                    new("timestamp_ms", KineticaType.Column.ColumnType.LONG, new List<string> { ColumnProperty.TIMESTAMP }),
                    new("temperature", KineticaType.Column.ColumnType.DOUBLE, new List<string> { ColumnProperty.NULLABLE }),
                    new("humidity", KineticaType.Column.ColumnType.DOUBLE, new List<string> { ColumnProperty.NULLABLE }),
                    new("raw_data", KineticaType.Column.ColumnType.BYTES, null),
                    new("is_valid", KineticaType.Column.ColumnType.INT, new List<string> { ColumnProperty.INT8 }),
                }
            );

            Console.WriteLine($"   Schema: {sensorType.getSchemaString()}");
            Console.WriteLine();

            // Example 3: Record with shard key
            Console.WriteLine("3. Record with Shard Key:");
            Console.WriteLine("   -----------------------");

            var shardedType = new KineticaType(
                "ShardedRecord",
                new List<KineticaType.Column>
                {
                    new("tenant_id", KineticaType.Column.ColumnType.INT, new List<string> { ColumnProperty.SHARD_KEY }),
                    new("record_id", KineticaType.Column.ColumnType.LONG, new List<string> { ColumnProperty.PRIMARY_KEY }),
                    new("data", KineticaType.Column.ColumnType.STRING, null),
                }
            );

            Console.WriteLine($"   Schema: {shardedType.getSchemaString()}");
            Console.WriteLine();

            // Example 4: Using SQL CREATE TABLE (recommended for production)
            Console.WriteLine("4. SQL CREATE TABLE Approach (Recommended):");
            Console.WriteLine("   -----------------------------------------");
            Console.WriteLine("   Instead of manually building schemas, use SQL:");
            Console.WriteLine();
            Console.WriteLine(@"   CREATE TABLE sensors.readings (
       sensor_id VARCHAR(64) NOT NULL,
       location VARCHAR(128),
       timestamp_ms TIMESTAMP NOT NULL,
       temperature DOUBLE,
       humidity DOUBLE,
       raw_data BLOB,
       is_valid TINYINT DEFAULT 1,
       PRIMARY KEY (sensor_id, timestamp_ms),
       SHARD KEY (sensor_id)
   )");
            Console.WriteLine();

            // Example 5: All column types reference
            Console.WriteLine("5. Column Type Reference:");
            Console.WriteLine("   ----------------------");
            Console.WriteLine($"   {"C# Type",-20} {"Kinetica Type",-20} {"Column Property",-25}");
            Console.WriteLine($"   {new string('-', 65)}");
            Console.WriteLine($"   {"int",-20} {"INT",-20} {"INT8, INT16 for smaller",-25}");
            Console.WriteLine($"   {"long",-20} {"LONG",-20} {"TIMESTAMP for time",-25}");
            Console.WriteLine($"   {"float",-20} {"FLOAT",-20} {"",-25}");
            Console.WriteLine($"   {"double",-20} {"DOUBLE",-20} {"",-25}");
            Console.WriteLine($"   {"string",-20} {"STRING",-20} {"CHAR1-256, IPV4, etc.",-25}");
            Console.WriteLine($"   {"byte[]",-20} {"BYTES",-20} {"WKT, WKB for geospatial",-25}");
            Console.WriteLine();

            // Example 6: Column properties reference
            Console.WriteLine("6. Column Properties Reference:");
            Console.WriteLine("   ----------------------------");
            Console.WriteLine($"   {"Property",-25} {"Description",-50}");
            Console.WriteLine($"   {new string('-', 75)}");
            Console.WriteLine($"   {"PRIMARY_KEY",-25} {"Column is part of the primary key",-50}");
            Console.WriteLine($"   {"SHARD_KEY",-25} {"Column is used for data sharding",-50}");
            Console.WriteLine($"   {"NULLABLE",-25} {"Column can contain null values",-50}");
            Console.WriteLine($"   {"TIMESTAMP",-25} {"Long column stores timestamp (ms)",-50}");
            Console.WriteLine($"   {"CHAR1-CHAR256",-25} {"String with fixed max length",-50}");
            Console.WriteLine($"   {"INT8, INT16",-25} {"Smaller integer storage",-50}");
            Console.WriteLine($"   {"IPV4",-25} {"String stores IPv4 address",-50}");
            Console.WriteLine($"   {"DECIMAL",-25} {"String stores decimal number",-50}");
            Console.WriteLine($"   {"DATE",-25} {"String stores date (YYYY-MM-DD)",-50}");
            Console.WriteLine($"   {"TIME",-25} {"String stores time (HH:MM:SS)",-50}");
            Console.WriteLine($"   {"DATETIME",-25} {"String stores datetime",-50}");
            Console.WriteLine($"   {"WKT",-25} {"String stores geospatial WKT",-50}");
            Console.WriteLine();

            // Example 7: Best practices
            Console.WriteLine("7. Best Practices:");
            Console.WriteLine("   ----------------");
            Console.WriteLine("   * Use SQL CREATE TABLE for production - it's clearer and more maintainable");
            Console.WriteLine("   * Always define a PRIMARY KEY for uniqueness constraints");
            Console.WriteLine("   * Use SHARD KEY on high-cardinality columns for even distribution");
            Console.WriteLine("   * Use NULLABLE only when needed - non-nullable is more efficient");
            Console.WriteLine("   * Use CHAR(N) instead of STRING when max length is known");
            Console.WriteLine("   * Use TIMESTAMP for time columns to enable time-based queries");
            Console.WriteLine();

            Console.WriteLine("=== Demo Complete ===");
        }
    }
}
