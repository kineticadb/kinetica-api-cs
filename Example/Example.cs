using System;
using System.Collections.Generic;

using Avro;
using kinetica;

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

            return ( value.Length <= maxLength ) ? value : value.Substring( 0, maxLength );
        }
    }


    class Example
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example C# Project - Running");
            Console.WriteLine();

            if ( args.Length < 1)
            {
                Console.WriteLine("Missing URL as command-line parameter.");
                Console.WriteLine("E.g., http://kinetica:9191");
                return;
            }

            try
            {
                string server_url = args[0];
                Console.WriteLine( "URL: {0}", server_url );
                Console.WriteLine();

                // Run the various example functions
                run_example( server_url );
                run_series_example( server_url );
                run_multihead_ingest_example( server_url );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught Exception: {0}", ex.ToString());
            }

            // We're done
            Console.WriteLine();
            Console.WriteLine("Example C# Project - Done");
            Console.WriteLine();
            Console.WriteLine("Enter any 7-digit prime number to continue: ");
            Console.ReadLine();
        }


        /// <summary>
        /// Example with a record type with nullable columns, primary keys and shard keys.
        /// </summary>
        /// <param name="server_url">The URL for the Kinetica server.</param>
        private static void run_example( string server_url )
        {
            // Establish a connection with Kinetica
            Kinetica kdb = new Kinetica( server_url );

            Console.WriteLine( "Example with a Record Type with Nullable Columns, Primary Keys and Shard Keys" );
            Console.WriteLine( "=============================================================================" );
            Console.WriteLine();

            string table_name = "csharp_example_table";

            // Create a type for our record_type_1 class
            Console.WriteLine( "Creating the type in kinetica..." );
            // Add some interesting properties for some of the data types
            IDictionary<string, IList<string>> column_properties = new Dictionary<string, IList<string>>();
            // Make a string char4 (and nullable)
            List<string> D_props = new List<string>();
            D_props.Add( ColumnProperty.CHAR4 );
            D_props.Add( ColumnProperty.NULLABLE );
            column_properties.Add( "D", D_props );
            // Let's try another nullable column
            List<string> E_props = new List<string>();
            E_props.Add( ColumnProperty.NULLABLE );
            column_properties.Add( "E", E_props );
            // And two primary keys (one nullable)
            List<string> A_props = new List<string>();
            A_props.Add( ColumnProperty.PRIMARY_KEY );
            column_properties.Add( "A", A_props );
            List<string> B_props = new List<string>();
            B_props.Add( ColumnProperty.PRIMARY_KEY );
            column_properties.Add( "B", B_props );
            // And a shard key (must be one of the primary keys, if specified--which we have)
            B_props.Add( ColumnProperty.SHARD_KEY );

            // Create the KineticaType object which facilitates creating types in the database
            KineticaType type1 = KineticaType.fromClass( typeof( record_type_1 ), column_properties );

            // Create the type in the database
            string type_id = type1.create( kdb );
            Console.WriteLine( "ID of the created type: " + type_id );
            Console.WriteLine();

            // Show the type information (fetched from Kinetica)
            Console.WriteLine( "Fetching the newly created type information..." );
            ShowTypesResponse rsp = kdb.showTypes( type_id, "" );
            Console.WriteLine( "Type properties: " );
            foreach ( var x in rsp.properties[0] )
                Console.WriteLine( x.Key + ": " + String.Join( ",", x.Value ) );
            Console.WriteLine();

            // Clear any previously made table with the same name
            Console.WriteLine( "Clearing any existing table named '{0}'", table_name );
            try { kdb.clearTable( table_name, null ); } catch ( Exception ex ) { /* I don't care if the table doesn't already exists! */ }
            Console.WriteLine();

            // Create a table of the given type
            Console.WriteLine( "Creating table named '{0}'", table_name );
            kdb.createTable( table_name, type_id );
            Console.WriteLine();

            // Call /show/table on the table just created
            Dictionary<string, string> show_table_options = new Dictionary<string, string>();
            show_table_options.Add( ShowTableRequest.Options.GET_SIZES, ShowTableRequest.Options.TRUE );
            Console.WriteLine( "Calling ShowTable on '{0}'", table_name );
            var response2 = kdb.showTable(table_name, show_table_options);
            Console.WriteLine( "Total size: {0}", response2.total_size );
            Console.WriteLine( "sizes: {0}", string.Join( ", ", response2.sizes ) );
            Console.WriteLine();

            // Create some data to be added to the table
            List<record_type_1> newData = new List<record_type_1>() {
                    new record_type_1() { A=99, B=11,  C="T0_lksdjfokdj92", D="D01",  E= 2.34F, F=null,   TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_1() { A=2,  B=3,   C="T1_asdfghjkl",    D=null,   E= 5.67F, F=null,   TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_1() { A=99, B=999, C="T2_oierlwk",      D="D244", E=-45.1F, F=9899.1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds }
                };

            // Insert the data into the table
            Console.WriteLine( "Inserting some data in '{0}'", table_name );
            var insertResponse = kdb.insertRecords( table_name, newData );
            Console.WriteLine( "Inserted {0} records", insertResponse.count_inserted + insertResponse.count_updated );
            Console.WriteLine();

            // Call /show/table on the table after adding data
            Console.WriteLine( "Calling ShowTable on '{0}' after adding data", table_name );
            var rsp2 = kdb.showTable(table_name, show_table_options);
            Console.WriteLine( "Total size: {0}", rsp2.total_size );
            Console.WriteLine( "sizes: {0}", string.Join( ", ", rsp2.sizes ) );
            Console.WriteLine();

            // Fetch the data back out of the DB using /get/records
            Console.WriteLine( "Getting records out" );
            var getRecordsResponse = kdb.getRecords<record_type_1>( table_name, 0, 100 );
            Console.WriteLine( "GetRecords got {0} records", getRecordsResponse.data.Count );
            foreach ( var r in getRecordsResponse.data ) Console.WriteLine( "\t" + r.ToString() );
            Console.WriteLine();

            // Do a filter operation
            Console.WriteLine( "Filtering data" );
            string view_name_1 = "csharp_view_01";
            string filter_expression = "E > 0";
            var filterResponse = kdb.filter( table_name, view_name_1, filter_expression );
            Console.WriteLine( "Filtered {0} records", filterResponse.count );
            Console.WriteLine();

            // Fetch the data from the filtered view using /get/records/fromcollection
            Console.WriteLine( "Getting records out (using /get/records/fromcollection) from view {0}", view_name_1 );
            IDictionary<string, string> getrecords_fc_options = new Dictionary<string, string>();
            getrecords_fc_options.Add( "return_record_ids", "true" );
            var request = new GetRecordsFromCollectionRequest( view_name_1, 0, 100, GetRecordsFromCollectionRequest.Encoding.BINARY, getrecords_fc_options );
            var getRecordsFromCollectionResponse = kdb.getRecordsFromCollection<record_type_1>( request );
            Console.WriteLine( "GetRecordsFromCollection got {0} records", getRecordsFromCollectionResponse.data.Count );
            foreach ( var r in getRecordsFromCollectionResponse.data ) Console.WriteLine( "\t" + r.ToString() );
            Console.WriteLine();

            // Do an /aggregate/groupby operation on the data
            Console.WriteLine( "Performing a group-by aggregate operation" );
            IList<string> column_names = new List<string>();
            column_names.Add( "A" );
            column_names.Add( "D" );
            var agbResponse = kdb.aggregateGroupBy( table_name, column_names, 0, 100 );
            Console.WriteLine( "Group by got {0} records", agbResponse.total_number_of_records );
            // Print the decoded data out to the console
            ( ( List<KineticaRecord> ) ( agbResponse.data ) ).ForEach( r => Console.WriteLine( r.ContentsToString() ) );
            Console.WriteLine();

            // Do an /aggregate/unique operation on column F
            string col_name = "F";
            Console.WriteLine( $"Performing a unique aggregate operation on column {col_name}" );
            var uniqueResponse = kdb.aggregateUnique( table_name, col_name, 0, 100 );
            // Print the decoded data out to the console
            ( ( List<KineticaRecord> ) ( uniqueResponse.data ) ).ForEach( r => Console.WriteLine( r.ContentsToString() ) );
            Console.WriteLine();

            // Fetch the data back out of the DB using /get/records/bycolumn
            Console.WriteLine( "Getting records out (using /get/records/bycolumn)" );
            IList<string> col_names = new List<string>();
            col_names.Add( "B" );
            col_names.Add( "C" );
            col_names.Add( "E" );
            var getRecordsByColumnResponse = kdb.getRecordsByColumn( table_name, col_names, 0, 100 );
            Console.WriteLine( "GetRecordsByColumn got {0} records", getRecordsByColumnResponse.data.Count );
            foreach ( var r in getRecordsByColumnResponse.data ) Console.WriteLine( "\t" + r.ContentsToString() );
            Console.WriteLine();

            Console.WriteLine();
        }  // end run_example



        /// <summary>
        /// Example showcasing a record with a series type column.
        /// </summary>
        /// <param name="server_url">The URL for the Kinetica server.</param>
        private static void run_series_example( string server_url )
        {
            // Establish a connection with Kinetica
            Kinetica kdb = new Kinetica( server_url );

            Console.WriteLine( "Example showcasing a record with a series type column" );
            Console.WriteLine( "======================================================" );
            Console.WriteLine();

            // Create the series type record in Kinetica
            KineticaType type_series = KineticaType.fromClass( typeof( record_type_series ) );
            string series_type_id = type_series.create( kdb );
            Console.WriteLine( "ID of the created series type: " + series_type_id );
            Console.WriteLine();

            // Clear any previously made table with the same name
            string table_name_series = "csharp_example_series_table";
            Console.WriteLine( "Clearing any existing table named '{0}'", table_name_series );
            try { kdb.clearTable( table_name_series, null ); } catch ( Exception ex ) { /* I don't care if the table doesn't already exists! */ }
            Console.WriteLine();

            // Create a table of the given type
            Console.WriteLine( "Creating table named '{0}'", table_name_series );
            kdb.createTable( table_name_series, series_type_id );
            Console.WriteLine();

            // Create some data to be added to the table
            string series_1 = "series_1";
            string series_2 = "series_2";
            List<record_type_series> series_data = new List<record_type_series>() {
                    // Five series points moving horizontally
                    new record_type_series() { x = 30, y = 40, TRACKID = series_1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = 35, y = 40, TRACKID = series_1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = 40, y = 40, TRACKID = series_1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = 45, y = 40, TRACKID = series_1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = 50, y = 40, TRACKID = series_1, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    // Five series points moving vertically
                    new record_type_series() { x = -30, y = -40, TRACKID = series_2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = -30, y = -45, TRACKID = series_2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = -30, y = -50, TRACKID = series_2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = -30, y = -55, TRACKID = series_2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                    new record_type_series() { x = -30, y = -60, TRACKID = series_2, TIMESTAMP= (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds },
                };

            // Insert the data into the table
            Console.WriteLine( "Inserting some data in '{0}'", table_name_series );
            var insertResponse2 = kdb.insertRecords( table_name_series, series_data );
            Console.WriteLine( "Inserted {0} records", insertResponse2.count_inserted + insertResponse2.count_updated );
            Console.WriteLine();

            // Fetch the data back out of the DB using /get/records
            Console.WriteLine( "Getting records out from {0}", table_name_series );
            var getRecordsResponse2 = kdb.getRecords<record_type_series>( table_name_series );
            Console.WriteLine( "GetRecords got {0} records", getRecordsResponse2.data.Count );
            foreach ( var r in getRecordsResponse2.data ) Console.WriteLine( "\t" + r.ToString() );
            Console.WriteLine();

            // Filter the series data such that only a few series point from only one of the series
            // are in the view
            Console.WriteLine( "Filtering data from {0} so that only some points from {1} to make it", table_name_series, series_1 );
            string view_name_2 = "csharp_view_02";
            // Only the first, second, and third records from the first series should survive this filter
            // (for track/series type records, we're filtering the "line segments", so records just outside the
            // filtered box will also be captured)
            var filterByBoxResp = kdb.filterByBox( table_name_series, view_name_2, "x", 33, 37, "y", 35, 45 );
            Console.WriteLine( "Filtered {0} records", filterByBoxResp.count );
            Console.WriteLine();
            // Show the filtered objects
            var getRecordsResponse3 = kdb.getRecords<record_type_series>( view_name_2 );
            Console.WriteLine( "GetRecords got {0} records from {1}", getRecordsResponse3.data.Count, view_name_2 );
            foreach ( var r in getRecordsResponse3.data ) Console.WriteLine( "\t" + r.ToString() );
            Console.WriteLine();


            // Extract the entire series from the source table (based on which ones are in the view)
            Console.WriteLine( "Getting records belonging to one series out from table '{0}' using the partial series in view '{1}'", table_name_series, view_name_2 );
            var getRecordsBySeriesResp = kdb.getRecordsBySeries<record_type_series>( view_name_2, table_name_series );
            Console.WriteLine( "GetRecordsBySeries got {0} list of records (should get one list of five records in it)", getRecordsBySeriesResp.data.Count );
            foreach ( var r_list in getRecordsBySeriesResp.data )
                foreach ( var r in r_list ) Console.WriteLine( "\t" + r.ToString() );
            Console.WriteLine();

            Console.WriteLine();
        }  // end run_series_example()


        /// <summary>
        /// Shows an example of how to use the multi-head ingestion feature
        /// of Kinetica.
        /// </summary>
        /// <param name="server_url">The URL for the Kinetica server.</param>
        private static void run_multihead_ingest_example( string server_url )
        {
            // Establish a connection with Kinetica
            Kinetica kdb = new Kinetica( server_url );

            Console.WriteLine( "\n\n" );
            Console.WriteLine( "Example showcasing multihead ingestion(one shard key, two primary keys)" );
            Console.WriteLine( "=======================================================================" );
            Console.WriteLine();

            // Create a type for our record_type_1 class
            // Add some interesting properties for some of the data types
            IDictionary<string, IList<string>> column_properties = new Dictionary<string, IList<string>>();
            // And two primary keys and one shard key
            List<string> A_props = new List<string>();
            A_props.Add( ColumnProperty.PRIMARY_KEY );
            column_properties.Add( "A", A_props );
            List<string> B_props = new List<string>();
            B_props.Add( ColumnProperty.PRIMARY_KEY );
            column_properties.Add( "B", B_props );
            // And a shard key (must be one of the primary keys, if specified--which we have)
            B_props.Add( ColumnProperty.SHARD_KEY );

            // Create the KineticaType object which facilitates creating types in the database
            KineticaType type1 = KineticaType.fromClass( typeof( record_type_1 ), column_properties );

            // Create the type in the database
            string type_id = type1.create( kdb );

            string table_name = "csharp_example_table_01";
            // Clear any previously made table with the same name
            Console.WriteLine( "Clearing any existing table named '{0}'", table_name );
            try { kdb.clearTable( table_name, null ); } catch ( Exception ex ) { /* I don't care if the table doesn't already exists! */ }
            Console.WriteLine();

            // Create a table of the given type
            Console.WriteLine( $"Creating table named '{table_name}'" );
            kdb.createTable( table_name, type_id );
            Console.WriteLine();

            // Create the ingestor (we're not giving any worker IP addresses; the ingestor class will figure it
            // out by itself)
            int batch_size = 100;
            KineticaIngestor<record_type_1> ingestor = new KineticaIngestor<record_type_1>( kdb, table_name, batch_size, type1 );

            // Generate data to be inserted
            int num_records = batch_size * 5;
            List<record_type_1> records = new List<record_type_1>();
            Random rng = new Random();
            double null_probability = 0.2;
            for ( int i = 0; i < num_records; ++i )
            {
                // Restricting string length to 256
                int max_str_len = rng.Next( 0, 256 );
                record_type_1 record = new record_type_1()
                {
                    A = rng.Next(),
                    B = rng.Next(),
                    C = System.IO.Path.GetRandomFileName().Truncate( max_str_len ),
                    D = System.IO.Path.GetRandomFileName().Truncate( max_str_len ),
                    E  = (float)(rng.NextDouble() * rng.Next()),
                    F  = ( rng.NextDouble() < null_probability ) ? null : ( double? ) ( rng.NextDouble() * rng.Next() ),
                    TIMESTAMP = (long) rng.Next()
                };

                records.Add( record );
            }  // end for loop

            Console.WriteLine( $"Generated {num_records} records." );

            // Insert the records into the ingestor
            Console.WriteLine( $"Inserting {num_records} records..." );
            ingestor.insert( records );

            // Flush the ingestor (which actually inserts the records)
            Console.WriteLine( "\nFlushing any remaining records." );
            ingestor.flush();

            Console.WriteLine();
            Console.WriteLine();
        }  // end run_multihead_ingest_example()


        private class record_type_1
        {
            public int A { get; set; }
            public int B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
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
        }  // end class record_type_1


        private class record_type_series
        {
            public double x { get; set; }
            public double y { get; set; }
            public string TRACKID { get; set; }
            public long TIMESTAMP { get; set; }

            public override string ToString()
            {
                return $"{{ x={x}, y={y}, TRACKID={TRACKID}, TIMESTAMP={TIMESTAMP} }}";
            }
        }  // end class record_type_series
    }
}
