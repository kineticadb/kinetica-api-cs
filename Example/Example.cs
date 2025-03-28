using System;
using System.Collections.Generic;
using JDBC.NET.Data;
using Avro;
using kinetica;
using System.Collections;
using System.Data.Common;
using System.Data;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Example
{
    /// <summary>
    /// Extension to string that has a truncate function.
    /// </summary>
    public static class StringExt
    {
        public static string Truncate( this string value, int maxLength )
        {
            if ( string.IsNullOrEmpty( value ) )
                return value;

            return ( value.Length <= maxLength ) ? value : value[..maxLength];
        }
    }


    class Example
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================================");
            Console.WriteLine("= Example C# Project - Running =");
            Console.WriteLine("================================");
            Console.WriteLine();

            if ( args.Length < 1)
            {
                Console.WriteLine("Missing URL as command-line parameter; e.g.: http://localhost:9191");
                return;
            }

            try
            {
                string serverUrl = args[0];
                Console.WriteLine($"URL: {serverUrl}\n");

                Kinetica.Options _ServerOptions = new()
                {
                    Username = args[1],
                    Password = args[2]
                };

                string JdbcDriverPath = "";
                if( args.Length > 3) {
                    JdbcDriverPath = args[3];
                }

                // Run the various example functions
                JdbcExample(JdbcDriverPath, serverUrl, _ServerOptions.Username, _ServerOptions.Password);
                RunExample( serverUrl, _ServerOptions );
                RunSeriesExample( serverUrl, _ServerOptions );
                RunMultiheadIngestExample( serverUrl, _ServerOptions );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught Exception: {ex} {ex.Message} {ex.StackTrace}\n");
            }

            // We're done
            Console.WriteLine("=============================");
            Console.WriteLine("= Example C# Project - Done =");
            Console.WriteLine("=============================");
        }

        #region Constants
        private const int columnWidth = 23;
        private const int displayLimit = 50;
        #endregion

        #region Console Method
        private static string AlignCenter(string text, int width)
        {
            text = text.Length > width ? string.Concat(text.AsSpan(0, width - 3), "...") : text;

            return !string.IsNullOrEmpty(text)
                ? text.PadRight(width - (width - text.Length) / 2).PadLeft(width)
                : new string(' ', width);
        }

        private static void PrintRow(string[] columns, bool header = false)
        {
            var row = "|";
            var line = "+";

            foreach (var column in columns)
            {
                row += AlignCenter(column, columnWidth) + "|";
                line += new string('-', columnWidth) + "+";
            }

            if (header)
            {
                Console.WriteLine(line);
                Console.WriteLine(row);
                Console.WriteLine(line.Replace('-', '='));
            }
            else
            {
                Console.WriteLine(row);
                Console.WriteLine(line);
            }
        }
        #endregion

        private static void PrintResult(DbDataReader reader)
        {
            var columns = new List<string>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }

            PrintRow([.. columns], true);

            while (reader.Read())
            {
                var items = new List<string>();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    items.Add(reader.GetString(i));
                }

                PrintRow([.. items]);
            }

            Console.WriteLine();
        }

        private static string GenerateRandomId()
        {
            Random random = new();
            int randomNumber = random.Next(1000, 9999); // Generates a random number between 1000 and 9999
            return $"ID_{randomNumber}";
        }
     
        private static string GenerateRandomUrl()
        {
            string[] domains = ["com", "net", "org", "io", "dev"];
            string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();

            string randomSubdomain = new(
                [.. new char[8].Select(_ => chars[random.Next(chars.Length)])]);

            string randomDomain = new(
                [.. new char[5].Select(_ => chars[random.Next(chars.Length)])]);

            string randomPath = new(
                [.. new char[6].Select(_ => chars[random.Next(chars.Length)])]);

            string domainExtension = domains[random.Next(domains.Length)];

            return $"https://{randomSubdomain}.{randomDomain}.{domainExtension}/{randomPath}";
        }

        private static string GenerateRandomName()
        {
            string[] firstNames = ["Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Helen", "Ivy", "Jack"];
            string[] lastNames = ["Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez"];

            Random random = new();
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];

            return $"{firstName} {lastName}";
        }

        private static string GenerateGeometry() {
            GeometryFactory factory = new();

            // Create a Polygon
            Polygon polygon = factory.CreatePolygon(
            [
                new Coordinate(0, 0),
                new Coordinate(10, 0),
                new Coordinate(10, 10),
                new Coordinate(0, 10),
                new Coordinate(0, 0) // Closing the polygon
            ]);
            return polygon.AsText();

        }

        private static string GenerateRandomPolygon()
        {
            GeometryFactory factory = new();
            Random rand = new();

            // Generate a random number of vertices (between 3 and 10)
            int numVertices = rand.Next(3, 11);

            // Generate random coordinate bounds
            double minX = rand.NextDouble() * 50;   // Min X in range [0, 50]
            double maxX = minX + rand.NextDouble() * 50; // Max X in range [minX, minX + 50]
            double minY = rand.NextDouble() * 50;   // Min Y in range [0, 50]
            double maxY = minY + rand.NextDouble() * 50; // Max Y in range [minY, minY + 50]

            Coordinate[] coordinates = new Coordinate[numVertices + 1];

            for (int i = 0; i < numVertices; i++)
            {
                double x = rand.NextDouble() * (maxX - minX) + minX;
                double y = rand.NextDouble() * (maxY - minY) + minY;
                coordinates[i] = new Coordinate(x, y);
            }

            // Ensure the polygon is closed (first and last point must be the same)
            coordinates[numVertices] = coordinates[0];

            // Create LinearRing and Polygon
            LinearRing ring = factory.CreateLinearRing(coordinates);
            Polygon polygon = factory.CreatePolygon(ring);

            return polygon.AsText();
        }

        private static DateTime GenerateRandomDate(DateTime start, DateTime end)
        {
            Random random = new();
            int range = (int)(end - start).TotalSeconds; // Get total seconds between the two dates
            return start.AddSeconds(random.Next(range));
        }

        /// <summary>
        /// Example integrating the Kinetica JDBC driver with a C# JDBC bridge.
        /// 
        /// Builds one object:
        /// * csharp_example_jdbc
        /// </summary>
        /// <param name="driverPath">The local path to the Kinetica JDBC driver.</param>
        /// <param name="serverUrl">The URL for the Kinetica server.</param>
        /// <param name="username">Username for the Kinetica server.</param>
        /// <param name="password">Password for the Kinetica server.</param>
        private static void JdbcExample(string driverPath, string serverUrl, string username, string password)
        {
            Console.WriteLine();
            Console.WriteLine("Example Using a JDBC Bridge Library and the Kinetica JDBC Driver");
            Console.WriteLine("================================================================");
            Console.WriteLine();

            if ( driverPath.Length == 0)
            {
                Console.WriteLine("No Kinetica JDBC driver given; e.g.:  kinetica-jdbc-7.2.2.8-jar-with-dependencies.jar\n");
                return;
            }

            var builder = new JdbcConnectionStringBuilder
            {
                DriverPath = driverPath,
                DriverClass = "com.kinetica.jdbc.Driver",
                JdbcUrl = $"jdbc:kinetica:URL={serverUrl};UID={username};PWD={password}"
            };

            using var connection = new JdbcConnection(builder);
            Console.WriteLine("Establishing connection...");
            connection.Open();
            Console.WriteLine("Connection established!\n");
            
            using JdbcCommand command = connection.CreateCommand();

            Console.WriteLine("Creating table...");
            try
            {
                string ddl = @"CREATE OR REPLACE TABLE csharp_example_jdbc 
                (
                    object_id VARCHAR NOT NULL,
                    name VARCHAR NOT NULL,
                    url VARCHAR NOT NULL,
                    bounding_box GEOMETRY NOT NULL,
                    created_at DATETIME NOT NULL,
                    updated_at DATETIME NOT NULL,
                    PRIMARY KEY (object_id)
                )
                ";

                command.CommandText = ddl;

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating table: {e}\n");
                return;
            }

            Console.WriteLine("Inserting data...");
            try
            {
                command.CommandText = "INSERT INTO csharp_example_jdbc VALUES (@ObjectId, @Name, @Url, @BoundingBox, @CreatedAt, @UpdatedAt)";
                command.Parameters.Clear();

                int rowsInserted = 0;
                for (int i = 0; i < 10; i++)
                {
                    command.Parameters.AddWithValue("@ObjectId", GenerateRandomId());
                    command.Parameters.AddWithValue("@Name", GenerateRandomName());
                    command.Parameters.AddWithValue("@Url", GenerateRandomUrl());
                    command.Parameters.AddWithValue("@BoundingBox", GenerateRandomPolygon());
                    command.Parameters.AddWithValue("@CreatedAt", GenerateRandomDate(new DateTime(2024, 1, 1), DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@UpdatedAt", GenerateRandomDate(new DateTime(2024, 1, 1), DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"));

                    int rowsAffected = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    rowsInserted += rowsAffected + 1;
                }
                Console.WriteLine($"Rows inserted: {rowsInserted}\n");
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"Error inserting data: {ex}\n");
                return;
            }

            Console.WriteLine("Executing query...");
            try
            {
                command.CommandText = "SELECT * FROM csharp_example_jdbc ORDER BY object_id";
                command.Parameters.Clear();

                using var reader = command.ExecuteReader();

                PrintResult(reader);
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"Error executing query: {ex}\n");
                return;
            }

            Console.WriteLine("Updating data...");
            try
            {
                command.CommandText = "UPDATE csharp_example_jdbc SET url = 'https://www.kinetica.com' WHERE object_id = (SELECT MIN(object_id) FROM csharp_example_jdbc)";
                int rowsUpdated = command.ExecuteNonQuery();
                Console.WriteLine($"Rows updated: {rowsUpdated}\n");
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"Error updating data: {ex}\n");
                return;
            }

            Console.WriteLine("Deleting data...");
            try
            {
                command.CommandText = "DELETE FROM csharp_example_jdbc WHERE object_id = (SELECT MAX(object_id) FROM csharp_example_jdbc)";
                int rowsDeleted = command.ExecuteNonQuery();
                Console.WriteLine($"Rows deleted: {rowsDeleted}\n");
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"Error updating data: {ex}\n");
            }

            Console.WriteLine("Checking data after modifications...");
            try
            {
                command.CommandText = "SELECT * FROM csharp_example_jdbc ORDER BY object_id";
                command.Parameters.Clear();

                using var reader = command.ExecuteReader();

                PrintResult(reader);
            }
            catch (System.Data.Common.DbException ex)
            {
                Console.WriteLine($"Error checking data: {ex}\n");
                return;
            }
        }

        /// <summary>
        /// Example with a record type with nullable columns, primary keys and shard keys.
        /// 
        /// Builds two objects:
        /// * csharp_example_table
        /// * csharp_example_filter
        /// </summary>
        /// <param name="serverUrl">The URL for the Kinetica server.</param>
        /// <param name="serverOptions">Username & password for the Kinetica server.</param>
        private static void RunExample(string serverUrl, Kinetica.Options serverOptions)
        {
            Console.WriteLine();
            Console.WriteLine("Example with a Record Type with Nullable Columns, Primary Keys and Shard Keys");
            Console.WriteLine("=============================================================================");
            Console.WriteLine();

            // Establish a connection with Kinetica
            Kinetica kdb = new(serverUrl, serverOptions);

            string tableName = "csharp_example_table";

            // Create a type for our exampleRecord class
            Console.WriteLine("Creating the type in kinetica...");
            // Add some interesting properties for some of the data types
            Dictionary<string, IList<string>> columnProperties = new()
            {
                // And a composite primary key on two columns
                {"A", [ColumnProperty.PRIMARY_KEY]},
                {"B", [ColumnProperty.PRIMARY_KEY, ColumnProperty.SHARD_KEY]},
                {"D", [ColumnProperty.CHAR4, ColumnProperty.NULLABLE]},
                {"E", [ColumnProperty.NULLABLE]}
            };

            // Create the KineticaType object which facilitates creating types in the database
            KineticaType exampleType = KineticaType.fromClass( typeof( ExampleRecord ), columnProperties );

            // Create the type in the database
            string exampleTypeId = exampleType.create(kdb);
            Console.WriteLine($"ID of the created type: {exampleTypeId}\n");

            // Show the type information (fetched from Kinetica)
            Console.WriteLine("Fetching the newly created type information...");
            ShowTypesResponse responseShowTypes = kdb.showTypes(exampleTypeId, "");
            Console.WriteLine("Type properties:");
            foreach ( var x in responseShowTypes.properties[0] )
                Console.WriteLine($"\t{x.Key}: {String.Join(",", x.Value)}");
            Console.WriteLine();

            // Clear any previously made table with the same name
            Console.WriteLine($"Clearing any existing table named '{tableName}'...\n");
            kdb.clearTable(tableName, "", new Dictionary<string, string> { [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE });

            // Create a table of the given type
            Console.WriteLine($"Creating table named '{tableName}'...\n");
            kdb.createTable( tableName, exampleTypeId );

            // Call /show/table on the table just created
            Dictionary<string, string> optionsShowTable = new() { [ShowTableRequest.Options.GET_SIZES] = ShowTableRequest.Options.TRUE };
            Console.WriteLine($"Calling ShowTable on '{tableName}'...");
            var responseShowTable = kdb.showTable(tableName, optionsShowTable);
            Console.WriteLine($"Total size: {responseShowTable.total_size}");
            Console.WriteLine($"Sizes: {string.Join(", ", responseShowTable.sizes)}\n");

            // Create some data to be added to the table
            List<ExampleRecord> newData = [
                new ExampleRecord() { A=99, B=11,  C="T0_lksdjfokdj92", D="D01",  E= 2.34F, F=null,   TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new ExampleRecord() { A=2,  B=3,   C="T1_asdfghjkl",    D=null,   E= 5.67F, F=null,   TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new ExampleRecord() { A=99, B=999, C="T2_oierlwk",      D="D244", E=-45.1F, F=9899.1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds }
            ];

            // Insert the data into the table
            Console.WriteLine($"Inserting some data in '{tableName}'...");
            var responseInsert = kdb.insertRecords( tableName, newData );
            Console.WriteLine($"Inserted {responseInsert.count_inserted + responseInsert.count_updated} records\n");

            // Call /show/table on the table after adding data
            Console.WriteLine($"Calling ShowTable on '{tableName}' after adding data...");
            responseShowTable = kdb.showTable(tableName, optionsShowTable);
            Console.WriteLine($"Total size: {responseShowTable.total_size}");
            Console.WriteLine($"Sizes: {string.Join( ", ", responseShowTable.sizes )}\n");

            // Fetch the data back out of the DB using /get/records
            Console.WriteLine($"Getting records from table '{tableName}'...");
            var responseGetRecords = kdb.getRecords<ExampleRecord>( tableName, 0, 100 );
            Console.WriteLine($"GetRecords got {responseGetRecords.data.Count} records:");
            foreach ( var r in responseGetRecords.data )
                Console.WriteLine($"\t{r}");
            Console.WriteLine();

            // Do a filter operation
            Console.WriteLine($"Filtering data from table '{tableName}'...");
            string filterName = "csharp_example_filter";
            string filterExpression = "E > 0";
            var filterResponse = kdb.filter( tableName, filterName, filterExpression );
            Console.WriteLine($"Filtered {filterResponse.count} records into '{filterName}'\n");

            // Do an /aggregate/groupby operation on the data
            Console.WriteLine($"Performing a group-by aggregate operation on table '{tableName}'...");
            IList<string> columnNames = ["A", "D"];
            var responseGroupBy = kdb.aggregateGroupBy( tableName, columnNames, 0, 100 );
            Console.WriteLine($"Group by got {responseGroupBy.total_number_of_records} records:");
            // Print the decoded data out to the console
            ( ( List<KineticaRecord> ) ( responseGroupBy.data ) ).ForEach( r => Console.WriteLine($"\t{r.ContentsToString()}") );
            Console.WriteLine();

            // Do an /aggregate/unique operation on column F
            string columnName = "F";
            Console.WriteLine($"Performing a unique aggregate operation on column '{tableName}.{columnName}'...");
            var uniqueResponse = kdb.aggregateUnique( tableName, columnName, 0, 100 );
            // Print the decoded data out to the console
            ( ( List<KineticaRecord> ) ( uniqueResponse.data ) ).ForEach( r => Console.WriteLine($"\t{r.ContentsToString()}") );
            Console.WriteLine();

            // Fetch the data back out of the DB using /get/records/bycolumn
            Console.WriteLine($"Getting records out (using /get/records/bycolumn) on table '{tableName}'...");
            columnNames = ["B", "C", "E"];
            var responseGetRecordsByColumn = kdb.getRecordsByColumn( tableName, columnNames, 0, 100 );
            Console.WriteLine($"GetRecordsByColumn got {responseGetRecordsByColumn.data.Count} records:");
            foreach ( var r in responseGetRecordsByColumn.data )
                Console.WriteLine($"\t{r.ContentsToString()}");
            Console.WriteLine("\n");
        }  // end RunExample



        /// <summary>
        /// Example showcasing a record with a series type column.
        /// 
        /// Builds two objects:
        /// * csharp_example_series_table
        /// * csharp_example_series_filter
        /// </summary>
        /// <param name="serverUrl">The URL for the Kinetica server.</param>
        /// <param name="serverOptions">Username & password for the Kinetica server.</param>
        private static void RunSeriesExample( string serverUrl, Kinetica.Options serverOptions )
        {
            Console.WriteLine();
            Console.WriteLine("Example showcasing a record with a series type column");
            Console.WriteLine("=====================================================");
            Console.WriteLine();

            // Establish a connection with Kinetica
            Kinetica kdb = new( serverUrl, serverOptions );

            // Create the series type record in Kinetica
            KineticaType seriesType = KineticaType.fromClass( typeof( SeriesRecord ) );
            string seriesTypeId = seriesType.create( kdb );
            Console.WriteLine($"ID of the created series type: {seriesTypeId}\n");

            // Clear any previously made table with the same name
            string tableName = "csharp_example_series_table";
            Console.WriteLine($"Clearing any existing table named '{tableName}'...\n");
            kdb.clearTable(tableName, "", new Dictionary<string, string> { [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE });

            // Create a table of the given type
            Console.WriteLine($"Creating table named '{tableName}'...\n");
            kdb.createTable( tableName, seriesTypeId );

            // Create some data to be added to the table
            string series1 = "series_1";
            string series2 = "series_2";
            List<SeriesRecord> seriesData =
            [
                // Five series points moving horizontally
                new SeriesRecord() { x = 30, y = 40, TRACKID = series1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = 35, y = 40, TRACKID = series1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = 40, y = 40, TRACKID = series1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = 45, y = 40, TRACKID = series1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = 50, y = 40, TRACKID = series1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                // Five series points moving vertically
                new SeriesRecord() { x = -30, y = -40, TRACKID = series2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = -30, y = -45, TRACKID = series2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = -30, y = -50, TRACKID = series2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = -30, y = -55, TRACKID = series2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                new SeriesRecord() { x = -30, y = -60, TRACKID = series2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
            ];

            // Insert the data into the table
            Console.WriteLine($"Inserting some data in '{tableName}'...");
            var responseInsert = kdb.insertRecords(tableName, seriesData);
            Console.WriteLine($"Inserted {responseInsert.count_inserted + responseInsert.count_updated} records\n");


            // Fetch the data back out of the DB using /get/records
            Console.WriteLine($"Getting records from table '{tableName}'...");
            var responseGetRecords = kdb.getRecords<SeriesRecord>(tableName);
            Console.WriteLine($"GetRecords got {responseGetRecords.data.Count} records:");
            foreach (var r in responseGetRecords.data)
                Console.WriteLine($"\t{r}");
            Console.WriteLine();


            // Filter the series data such that only a few series point from only one of the series
            // are in the view
            Console.WriteLine($"Filtering data from {tableName} so that only some points from {series1} make it...");
            string filterName = "csharp_example_series_filter";
            // Only the first, second, and third records from the first series should survive this filter
            // (for track/series type records, we're filtering the "line segments", so records just outside the
            // filtered box will also be captured)
            var responseFilter = kdb.filterByBox( tableName, filterName, "x", 33, 37, "y", 35, 45 );
            Console.WriteLine($"Filtered {responseFilter.count} records into '{filterName}'\n");

            // Show the filtered objects
            responseGetRecords = kdb.getRecords<SeriesRecord>( filterName );
            Console.WriteLine($"GetRecords got {responseGetRecords.data.Count} records from '{filterName}':");
            foreach ( var r in responseGetRecords.data )
                Console.WriteLine($"\t{r}");
            Console.WriteLine();


            // Extract the entire series from the source table (based on which ones are in the view)
            Console.WriteLine($"Getting records belonging to one series out from table '{tableName}' using the partial series in filter '{filterName}'...");
            var getRecordsBySeriesResp = kdb.getRecordsBySeries<SeriesRecord>( filterName, tableName );
            Console.WriteLine($"GetRecordsBySeries got {getRecordsBySeriesResp.data.Count} list of records (should get one list of five records in it):");
            foreach ( var rSeries in getRecordsBySeriesResp.data )
                foreach ( var r in rSeries ) Console.WriteLine($"\t{r}");
            Console.WriteLine("\n");
        }  // end RunSeriesExample()


        /// <summary>
        /// Shows an example of how to use the multi-head ingestion feature of Kinetica.
        /// 
        /// Builds one object:
        /// * csharp_example_multihead_table
        /// </summary>
        /// <param name="serverUrl">The URL for the Kinetica server.</param>
        /// <param name="serverOptions">Username & password for the Kinetica server.</param>
        private static void RunMultiheadIngestExample( string serverUrl, Kinetica.Options serverOptions )
        {
            Console.WriteLine();
            Console.WriteLine("Example showcasing multihead ingestion(one shard key, two primary keys)");
            Console.WriteLine("=======================================================================");
            Console.WriteLine();

            // Establish a connection with Kinetica
            Kinetica kdb = new( serverUrl, serverOptions );

            // Create a type for our exampleRecord class
            // Add some interesting properties for some of the data types
            Dictionary<string, IList<string>> columnProperties = new()
            {
                // Add a primary key
                { "A", [ColumnProperty.PRIMARY_KEY] },

                // And a shard key (must be part of the primary keys, if specified--which we have)
                { "TIMESTAMP", [ColumnProperty.PRIMARY_KEY, ColumnProperty.SHARD_KEY, ColumnProperty.TIMESTAMP] }
            };

            // Create the KineticaType object which facilitates creating types in the database
            KineticaType exampleType = KineticaType.fromClass( typeof( ExampleRecord ), columnProperties );

            // Create the type in the database
            string exampleTypeId = exampleType.create( kdb );

            // Clear any previously made table with the same name
            string tableName = "csharp_example_multihead_table";
            Console.WriteLine($"Clearing any existing table named '{tableName}'...\n");
            kdb.clearTable(tableName, "", new Dictionary<string, string> { [ClearTableRequest.Options.NO_ERROR_IF_NOT_EXISTS] = ClearTableRequest.Options.TRUE });

            // Create a table of the given type
            Console.WriteLine($"Creating table named '{tableName}'...\n");
            kdb.createTable( tableName, exampleTypeId );

            // Create the ingestor (we're not giving any worker IP addresses; the ingestor class will figure it
            // out by itself)
            int batchSize = 100;
            KineticaIngestor<ExampleRecord> ingestor = new( kdb, tableName, batchSize, exampleType );

            // Generate data to be inserted
            int totalRecords = batchSize * 5;
            List<ExampleRecord> records = [];
            Random rng = new();
            double nullProbability = 0.2;
            for ( int i = 0; i < totalRecords; ++i )
            {
                // Restricting string length to 256
                int maxStringLength = rng.Next( 0, 256 );
                ExampleRecord record = new ()
                {
                    A = rng.Next(),
                    B = rng.Next(),
                    C = System.IO.Path.GetRandomFileName().Truncate( maxStringLength ),
                    D = System.IO.Path.GetRandomFileName().Truncate( maxStringLength ),
                    E  = (1.0F / rng.Next()),
                    F  = ( rng.NextDouble() < nullProbability ) ? null : ( double? ) ( rng.NextDouble() * (1.0F / rng.Next()) ),
                    TIMESTAMP = (long)rng.Next( -306102240, 293795424 ) * rng.Next( 100000 )
                };

                records.Add( record );
            }  // end for loop

            Console.WriteLine( $"Generated {totalRecords} records.\n" );

            // Insert the records into the ingestor
            Console.WriteLine( $"Inserting {totalRecords} records..." );
            ingestor.insert( records );

            // Flush the ingestor (which actually inserts the records)
            Console.WriteLine( "Flushing any remaining records..." );
            ingestor.flush();

            // Call /show/table on the table after adding data
            Console.WriteLine($"Calling ShowTable on '{tableName}' after adding data...");
            var responseShowTable = kdb.showTable(tableName, new Dictionary<string,string> { [ShowTableRequest.Options.GET_SIZES] = ShowTableRequest.Options.TRUE });
            Console.WriteLine($"Total size: {responseShowTable.total_size}");

            Console.WriteLine("\n");
        }  // end run_multihead_ingest_example()


        private class ExampleRecord
        {
            public int A { get; set; }
            public int B { get; set; }
            public string? C { get; set; }
            public string? D { get; set; }
            public float E { get; set; }
            public double? F { get; set; }
            public long TIMESTAMP { get; set; }

            public override string ToString()
            {
                string f;
                if ( F != null )
                    f = $"{F}";
                else
                    f = "<null>";
                return $"{{ A={A}, B={B}, C={C}, D={D}, E={E}, F={f}, TIMESTAMP={TIMESTAMP} }}";
            }
        }  // end class exampleRecord


        private class SeriesRecord
        {
            public double x { get; set; }
            public double y { get; set; }
            public string? TRACKID { get; set; }
            public long TIMESTAMP { get; set; }

            public override string ToString()
            {
                return $"{{ x={x}, y={y}, TRACKID={TRACKID}, TIMESTAMP={TIMESTAMP} }}";
            }
        }  // end class record_type_series
    }
}
