using Avro;
using Avro.IO;
using System;
using System.Collections.Generic;
using System.IO;



namespace kinetica
{
    /// <summary>
    /// Convenience class for using Avro.Generic.GenericRecord objects.
    /// </summary>
    public class KineticaRecord : Avro.Generic.GenericRecord
    {
        public KineticaRecord( RecordSchema schema ) : base( schema ) {}

        /// <summary>
        /// Convert the contents of the record to a string.
        /// </summary>
        /// <returns>A string containing the field names and values.</returns>
        public string ContentsToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append( "Contents: " );
            sb.Append( "{ " );
            object value;
            string value_string;
            foreach ( Avro.Field field in this.Schema.Fields )
            //foreach ( KeyValuePair<string, object> kv in this.contents )
            {
                // Append the field name
                sb.Append( field.Name );
                sb.Append( ": " );

                // Get the field value (handle nulls)
                this.TryGetValue( field.Name, out value );
                if ( value != null )
                    value_string = $"{value}";
                else
                    value_string = "<null>";

                // Append the field value
                sb.Append( value_string );
                sb.Append( ", " );
            }
            sb.Remove( (sb.Length - 2), 2 ); // remove the trailing comma and space
            sb.Append( " }" );
            return sb.ToString();
        }  // end ContentsToString


        /// <summary>
        /// Decodes binary encoded data of a dynamically created table returned by the server.
        /// </summary>
        /// <param name="dynamic_table_schema_string">The schema string for the dynamically created table.</param>
        /// <param name="encoded_data">The binary encoded data.</param>
        /// <returns>A list of KineticaRecord objects with the decoded data.</returns>
        public static IList<KineticaRecord> DecodeDynamicTableRecords( string dynamic_table_schema_string, byte[] encoded_data )
        {
            // Get a record schema from the schema string
            Schema dynamic_table_schema;
            try
            {
                dynamic_table_schema = Avro.Schema.Parse( dynamic_table_schema_string );
            }
            catch ( Exception ex )
            {
                throw new KineticaException( ex.ToString() );
            }

            // The container for the decoded data (put into distinct records)
            IList<KineticaRecord> records = new List<KineticaRecord>();

            // Decode the dynamic table schema to extract the column names and types.
            // Then, decode each individual record.
            using ( var ms = new MemoryStream( encoded_data ) )
            {
                // Read the table schema into a new GenericRecord
                // ----------------------------------------------
                var reader = new Avro.Generic.DefaultReader(dynamic_table_schema, dynamic_table_schema);
                BinaryDecoder decoder = new BinaryDecoder( ms );
                Avro.Generic.GenericRecord obj = (Avro.Generic.GenericRecord) reader.Read( null, dynamic_table_schema, dynamic_table_schema, decoder );

                // Extract the column names from the encoded data
                object column_headers_0 = new object();
                Object[] column_headers = null;
                if ( obj.TryGetValue( "column_headers", out column_headers_0 ) ) // try to get the data out
                {
                    column_headers = ( Object[] ) column_headers_0;
                }

                // Extract the column types from the encoded data
                object column_types_0 = new object();
                Object[] column_types = null;
                if ( obj.TryGetValue( "column_datatypes", out column_types_0 ) ) // try to get the data out
                {
                    column_types = ( Object[] ) column_types_0;
                }

                // Find out how many columns are returned
                int num_columns = column_headers.Length;

                // Extract the column data from the encoded data (ignore the headers and types)
                // and create a list with only the record data
                Object[][] encoded_column_data = new Object[ num_columns ][];

                for ( int i = 0; i < num_columns; ++i )
                {
                    // Get the column name (e.g. the first column is titled "column_1")
                    string column_name = $"column_{i+1}";

                    // Get the column data out
                    object column_data_0 = new object();
                    Object[] column_data = null;
                    if ( obj.TryGetValue( column_name, out column_data_0 ) ) // try to get the data out
                    {
                        column_data = ( Object[] ) column_data_0;
                    }

                    // Save this column's data in the 2D array declared above
                    encoded_column_data[i] = column_data;
                }  // done separating the column data from the headers and type


                // Find out how many values per column are returned
                int num_records = encoded_column_data[0].Length;

                // Make sure that all the column data are of the same length
                foreach ( Object[] l in encoded_column_data )
                {
                    if ( l.Length != num_records )
                        throw new KineticaException( "Dynamic table has uneven column data lengths" );
                }

                // Based on the column headers and types, create a KineticaType
                KineticaType dynamic_record_type = KineticaType.fromDynamicSchema( dynamic_table_schema_string, column_headers, column_types );

                // Using the dynamic record type, create a RecordSchema object
                // ( off which we'll create the records)
                Avro.RecordSchema record_schema = (Avro.RecordSchema) dynamic_record_type.getSchema();

                // Create the records by decoding the binary encoded data
                // (column-major data into row-major data)
                for ( int record_idx = 0; record_idx < num_records; ++record_idx )
                {
                    // Create a GenericRecord object based on the KineticaType
                    KineticaRecord record = new KineticaRecord( record_schema );

                    // Go through each column, decode the next value and put it into the record
                    for ( int column_idx = 0; column_idx < num_columns; ++column_idx )
                    {
                        // Get the value to be put
                        var val = encoded_column_data[ column_idx ][ record_idx ];

                        // Get the property of the record into which the value need to saved
                        var field = record_schema.Fields[ column_idx ];

                        // Set the value
                        record.Add( field.Name, val );
                    }  // end inner for loop

                    // Save the record
                    records.Add( record );
                }  // end outer for loop
            }  // end decoding block

            return records;
        }  // end DecodeDynamicTableRecords



    }  // end class KineticaRecord 
}  // end namespace kinetica
