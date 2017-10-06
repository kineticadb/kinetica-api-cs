using Avro.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

/// \mainpage Introduction
///
/// This is the client-side C# application programming interface (API) for Kinetica.
///
/// </br>
///
/// The source code can be found <a href="https://github.com/kineticadb/kinetica-api-cs">here</a>.
///
/// </br>
///
/// There are two projects here: Kinetica and Example.
/// </br>
///
/// The Kinetica project contains the main client source code in the kinetica namespace.
/// The <see cref="kinetica.Kinetica">Kinetica</see> class implements the interface for the API.  The Protocol
/// subdirectory contains classes for each endpoint of the database server.
///
/// </br>
///
/// The Example project contains a short example <see cref="Example.Example">here</see>.  The user
/// needs to specify the hostname of a database server (e.g. "127.0.0.1:9191") in the property
/// to properly run it.
///

namespace kinetica
{
    /// <summary>
    /// API to talk to Kinetica Database
    /// </summary>
    public partial class Kinetica
    {
        /// <summary>
        /// No Limit
        /// </summary>
        public const int END_OF_SET = -9999;

        /// <summary>
        /// Connection Options
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Optional: User Name for Kinetica security
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Optional: Password for user
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Use Snappy
            /// </summary>
            public bool UseSnappy { get; set; } = false;

            /// <summary>
            /// Thread Count
            /// </summary>
            public int ThreadCount { get; set; } = 1;
        }

        /// <summary>
        /// API Version
        /// </summary>
        /// <returns>Version String for API</returns>
        public static string GetApiVersion() { return API_VERSION; }

        /// <summary>
        /// URL for Kinetica Server (including "http:" and port)
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Optional: User Name for Kinetica security
        /// </summary>
        public string Username { get; private set; } = null;

        /// <summary>
        /// Optional: Password for user
        /// </summary>
        private string Password { get; set; } = null;

        /// <summary>
        /// Optional: Authorization for connections.
        /// </summary>
        private string Authorization { get; set; } = null;

        /// <summary>
        /// Use Snappy
        /// </summary>
        public bool UseSnappy { get; set; } = false;

        /// <summary>
        /// Thread Count
        /// </summary>
        public int ThreadCount { get; set; } = 1;

        // private string authorization;
        private volatile System.Collections.Concurrent.ConcurrentDictionary<string, KineticaType> knownTypes = new ConcurrentDictionary<string, KineticaType>();

        // private type label to type ID lookup table
        private Dictionary<string, string> typeNameLookup = new Dictionary<string, string>();

        // private object class type to KineticaType lookup table
        private Dictionary<Type, KineticaType> kineticaTypeLookup = new Dictionary<Type, KineticaType>();

        /// <summary>
        /// API Constructor
        /// </summary>
        /// <param name="url">URL for Kinetica Server (including "http:" and port)</param>
        /// <param name="options">Optional connection options</param>
        public Kinetica( string url, Options options = null )
        {
            Url = url;
            if ( null != options ) // If caller specified options
            {
                Username = options.Username;
                Password = options.Password;

                // Handle authorization
                if ( ( Username != null && ( Username.Length > 0 ) ) || ( Password != null && ( Password.Length > 0 ) ) )
                {
                    Authorization = ( "Basic " +
                                      Convert.ToBase64String( Encoding.GetEncoding( "ISO-8859-1" ).GetBytes( Username + ":" + Password ) ) );
                }


                UseSnappy = options.UseSnappy;
                ThreadCount = options.ThreadCount;
                // TODO: executor?
            }
        }


        /// <summary>
        /// Given a table name, add its record type to enable proper encoding of records
        /// for insertion or updates.
        /// </summary>
        /// <param name="table_name">Name of the table.</param>
        /// <param name="obj_type">The type associated with the table.</param>
        public void AddTableType( string table_name, Type obj_type )
        {
            try
            {
                // Get the type from the table
                KineticaType ktype = KineticaType.fromTable( this, table_name );
                if ( ktype.getTypeID() == null )
                    throw new KineticaException( $"Could not get type ID for table '{table_name}'" );
                this.knownTypes.TryAdd( ktype.getTypeID(), ktype );

                // Save a mapping of the object to the KineticaType
                if ( obj_type != null )
                    this.SetKineticaSourceClassToTypeMapping( obj_type, ktype );

            } catch ( KineticaException ex )
            {
                throw new KineticaException( "Error creating type from table", ex );
            }
        }  // end AddTableType

        /// <summary>
        /// Saves an object class type to a KineticaType association.  If the class type already exists
        /// in the map, replaces the old KineticaType value.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="kineticaType">The associated KinetiaType object.</param>
        public void SetKineticaSourceClassToTypeMapping( Type objectType, KineticaType kineticaType )
        {
            this.kineticaTypeLookup.Add( objectType, kineticaType );
            return;
        }  // end SetKineticaSourceClassToTypeMapping



        /// <summary>
        /// Given a schema string for a certain record type, decode binary data into distinct
        /// records (objects).
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="schema_string">The schema for the records.</param>
        /// <param name="records_binary">The binary encoded data to be decoded.</param>
        /// <param name="records">The decoded objects/records.</param>
        public void DecodeRawBinaryDataUsingSchemaString<T>( string schema_string,
                                                             IList<byte[]> records_binary,
                                                             IList<T> records ) where T : new()
        {
            // Create a KineticaType object based on the schema string
            KineticaType ktype = new KineticaType( "", schema_string, null );

            // Using the KineticaType object, decode all the records from avro binary encoding
            foreach ( var bin_record in records_binary )
            {
                T obj = AvroDecode<T>( bin_record, ktype );
                records.Add( obj );
            }
        }  // DecodeRawBinaryDataUsingSchemaString

        /// <summary>
        /// Given a list of schema strings, decode binary data into distinct
        /// records (objects).
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="schema_strings">The schemas for the records.</param>
        /// <param name="lists_records_binary">The binary encoded data to be decoded (the data is
        /// in a 2D list).</param>
        /// <param name="record_lists">The decoded objects/records in a 2d list.</param>
        public void DecodeRawBinaryDataUsingSchemaString<T>( IList<string> schema_strings,
                                                             IList<IList<byte[]>> lists_records_binary,
                                                             IList<IList<T>> record_lists ) where T : new()
        {
            // Check that the list of schemas and list of binary encode data match in length
            if ( schema_strings.Count != lists_records_binary.Count )
                throw new KineticaException( "List of schemas and list of binary encoded data do not match in count." );

            // Using the KineticaType object, decode all the records from avro binary encoding
            for ( int i = 0; i < schema_strings.Count; ++i )
            {
                // Create a KineticaType object based on the schema string
                KineticaType ktype = new KineticaType( "", schema_strings[ i ], null );

                // Get the binary encoded data for this list
                IList<byte[]> records_binary = lists_records_binary[ i ];

                // Create a container to put the decoded records
                IList<T> records = new List<T>();

                // The inner list actually contains the binary data
                foreach ( var bin_record in records_binary )
                {
                    T obj = AvroDecode<T>( bin_record, ktype );
                    records.Add( obj );
                }
                // Add the records into the outgoing list
                record_lists.Add( records );
            }
        }  // DecodeRawBinaryDataUsingSchemaString


        /// <summary>
        /// Given IDs of records types registered with Kinetica, decode binary
        /// data into distinct records (objects).
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="type_ids">The IDs for each of the records' types.</param>
        /// <param name="records_binary">The binary encoded data to be decoded.</param>
        /// <param name="records">The decoded objects/records.</param>
        public void DecodeRawBinaryDataUsingTypeIDs<T>( IList<string> type_ids,
                                                        IList<byte[]> records_binary,
                                                        IList<T> records ) where T : new()
        {
            // Make sure that the length of the type IDs and records are the same
            if ( type_ids.Count != records_binary.Count )
                throw new KineticaException( "Unequal numbers of type IDs and binary encoded data objects provided." );

            // Decode all the records
            for ( int i = 0; i < records_binary.Count; ++i )
            {
                // Per object, use the respective type ID to create the appropriate KineticaType
                KineticaType ktype = KineticaType.fromTypeID( this, type_ids[ i ] );

                // Using the KineticaType object, decode the record.
                T obj = AvroDecode<T>( records_binary[ i ], ktype );
                records.Add( obj );
            }
        }  // DecodeRawBinaryDataUsingTypeIDs


        /// <summary>
        /// Given IDs of records types registered with Kinetica, decode binary
        /// data into distinct records (objects).
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="type_ids">The IDs for each of the lists of records.</param>
        /// <param name="lists_records_binary">The binary encoded data to be decoded in a 2d list.</param>
        /// <param name="record_lists">The decoded objects/records in a 2d list.</param>
        public void DecodeRawBinaryDataUsingTypeIDs<T>( IList<string> type_ids,
                                                        IList<IList<byte[]>> lists_records_binary,
                                                        IList<IList<T>> record_lists ) where T : new()
        {
            // Make sure that the length of the type IDs and records are the same
            if ( type_ids.Count != lists_records_binary.Count )
                throw new KineticaException( "Unequal numbers of type IDs and binary encoded data objects provided." );

            // Decode all the records
            for ( int i = 0; i < lists_records_binary.Count; ++i )
            {
                // Per object, use the respective type ID to create the appropriate KineticaType
                KineticaType ktype = KineticaType.fromTypeID( this, type_ids[ i ] );

                // Get the binary encoded data for this list
                IList<byte[]> records_binary = lists_records_binary[ i ];

                // Create a container to put the decoded records
                IList<T> records = new List<T>();

                // The inner list actually contains the binary data
                foreach ( var bin_record in records_binary )
                {
                    // Using the KineticaType object, decode the record.
                    T obj = AvroDecode<T>( bin_record, ktype );
                    records.Add( obj );
                }
                // Add the records into the outgoing list
                record_lists.Add( records );
            }
        }  // DecodeRawBinaryDataUsingTypeIDs


        /// <summary>
        /// Send request object to Kinetica server, and get response
        /// </summary>
        /// <typeparam name="TResponse">Kinetica Response Object Type</typeparam>
        /// <param name="url">The specific URL to send the request to</param>
        /// <param name="request">Kinetica Request Object</param>
        /// <param name="enableCompression">Use Compression</param>
        /// <param name="avroEncoding">Use Avro Encoding</param>
        /// <returns>Response Object</returns>
        internal TResponse SubmitRequest<TResponse>( Uri url, object request, bool enableCompression = false, bool avroEncoding = true ) where TResponse : new()
        {
            // Get the bytes to send, encoded in the requested way
            byte[] requestBytes;
            if ( avroEncoding )
            {
                requestBytes = AvroEncode( request );
            }
            else // JSON
            {
                string str = JsonConvert.SerializeObject(request);
                requestBytes = Encoding.UTF8.GetBytes( str );
            }

            // Send request, and receive response
            KineticaResponse kineticaResponse = SubmitRequestRaw( url.ToString(), requestBytes, enableCompression, avroEncoding, false);

            // Decode response payload
            if ( avroEncoding )
            {
                return AvroDecode<TResponse>( kineticaResponse.data );
            }
            else // JSON
            {
                kineticaResponse.data_str = kineticaResponse.data_str.Replace( "\\U", "\\u" );
                return JsonConvert.DeserializeObject<TResponse>( kineticaResponse.data_str );
            }
        }  // end SubmitRequest( URL )


        /// <summary>
        /// Send request object to Kinetica server, and get response
        /// </summary>
        /// <typeparam name="TResponse">Kinetica Response Object Type</typeparam>
        /// <param name="endpoint">Kinetica Endpoint to call</param>
        /// <param name="request">Kinetica Request Object</param>
        /// <param name="enableCompression">Use Compression</param>
        /// <param name="avroEncoding">Use Avro Encoding</param>
        /// <returns>Response Object</returns>
        private TResponse SubmitRequest<TResponse>(string endpoint, object request, bool enableCompression = false, bool avroEncoding = true) where TResponse : new()
        {
            // Get the bytes to send, encoded in the requested way
            byte[] requestBytes;
            if (avroEncoding)
            {
                requestBytes = AvroEncode(request);
            }
            else // JSON
            {
                string str = JsonConvert.SerializeObject(request);
                requestBytes = Encoding.UTF8.GetBytes(str);
            }

            // Send request, and receive response
            KineticaResponse kineticaResponse = SubmitRequestRaw(endpoint, requestBytes, enableCompression, avroEncoding);

            // Decode response payload
            if (avroEncoding)
            {
                return AvroDecode<TResponse>(kineticaResponse.data);
            }
            else // JSON
            {
                kineticaResponse.data_str = kineticaResponse.data_str.Replace("\\U", "\\u");
                return JsonConvert.DeserializeObject<TResponse>(kineticaResponse.data_str);
            }
        }  // end SubmitRequest( endpoint )



        /// <summary>
        /// Send encoded request to Kinetica server, and get Kinetica Response
        /// </summary>
        /// <param name="url">Kinetica Endpoint to call (with or without the full host path)</param>
        /// <param name="requestBytes">Binary data to send</param>
        /// <param name="enableCompression">Are we using compression?</param>
        /// <param name="avroEncoding">Use Avro encoding</param>
        /// <param name="only_endpoint_given">If true, prefix the given url
        /// with <member cref="Url" /></param>
        /// <returns>KineticaResponse Object</returns>
        private KineticaResponse SubmitRequestRaw(string url, byte[] requestBytes, bool enableCompression, bool avroEncoding, bool only_endpoint_given = true)
        {
            try
            {
                if ( only_endpoint_given )
                    url = (Url + url);
                var request = (HttpWebRequest)WebRequest.Create( url );
                request.Method = "POST";
                //request.UseDefaultCredentials = true;
                request.ContentType = avroEncoding ? "application/octet-stream" : "application/json";
                request.ContentLength = requestBytes.Length;

                // Handle the authorization
                if ( this.Authorization != null )
                {
                    request.Headers.Add( "Authorization", Authorization );
                }


                // Write the binary request data
                using ( var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(requestBytes, 0, requestBytes.Length);
                }

                // Send to the server and await a response
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Parse the response
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (avroEncoding)
                            {
                                return AvroDecode<KineticaResponse>(responseStream);
                            }
                            else // JSON
                            {
                                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                                {
                                    var responseString = reader.ReadToEnd();
                                    return JsonConvert.DeserializeObject<KineticaResponse>(responseString);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                // Skip trying parsing the message if not a protocol error
                if ( ex.Status != WebExceptionStatus.ProtocolError )
                    throw new KineticaException( ex.ToString(), ex );

                // Get the error message from the server response
                var response = ex.Response;
                var responseStream = response.GetResponseStream();
                string responseString;
                KineticaResponse serverResponse;
                // Decode the response packet
                if (avroEncoding)
                {
                    serverResponse = AvroDecode<KineticaResponse>(responseStream);
                }
                else // JSON
                {
                    using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        responseString = reader.ReadToEnd();
                        serverResponse = JsonConvert.DeserializeObject<KineticaResponse>(responseString);
                    }
                }
                // Throw the error message found within the response packet
                throw new KineticaException( serverResponse.message );
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString(), ex);
            }

            return null;
        }

        private void SetDecoderIfMissing(string typeId, string label, string schemaString, IDictionary<string, IList<string>> properties)
        {
            // If the table is a collection, it does not have a proper type so ignore it

            if (typeId == "<collection>")
            {
                return;
            }

            knownTypes.GetOrAdd(typeId, (s) =>
            {
                return new KineticaType(label, schemaString, properties);
            });
            typeNameLookup[label] = typeId;
        }


        /// <summary>
        /// Retrieve a KineticaType object by the type label.
        /// </summary>
        /// <param name="typeName">The label/name of the type.</param>
        /// <returns></returns>
        private KineticaType GetType(string typeName)
        {
            KineticaType type = null;
            string typeId;
            if (typeNameLookup.TryGetValue(typeName, out typeId))
            {
                knownTypes.TryGetValue(typeId, out type);
            }

            return type;
        }


        /// <summary>
        /// Given a class type, look up the associated KineticaType.  If none is found, return null.
        /// </summary>
        /// <param name="objectType">The type of the object whose associated KineticaType we need.</param>
        /// <returns></returns>
        private KineticaType lookupKineticaType( Type objectType )
        {
            if ( !this.kineticaTypeLookup.ContainsKey( objectType ) )
                return null; // none found

            return this.kineticaTypeLookup[ objectType ];
        }  // lookupKineticaType()


        /// <summary>
        /// Encode specified object using Avro
        /// </summary>
        /// <param name="obj">Object to encode</param>
        /// <returns>Byte array of binary Avro-encoded data</returns>
        internal byte[] AvroEncode(object obj)
        {
            // Create a stream that will allow us to view the underlying memory
            using ( var ms = new MemoryStream())
            {
                // Write the object to the memory stream
                // If obj is an ISpecificRecord, this is more efficient
                if ( obj is Avro.Specific.ISpecificRecord)
                {
                    var schema = (obj as Avro.Specific.ISpecificRecord).Schema;
                    Avro.Specific.SpecificDefaultWriter writer = new Avro.Specific.SpecificDefaultWriter(schema);
                    writer.Write(schema, obj, new BinaryEncoder(ms));
                }
                else // Not an ISpecificRecord - this way is less efficient
                {
                    // Get the KineticaType associated with the object to be encoded
                    Type obj_type = obj.GetType();
                    KineticaType ktype = lookupKineticaType( obj_type );
                    if ( ktype == null )
                    {
                        throw new KineticaException( "No known KineticaType associated with the given object.  " +
                                                     "Need a known KineticaType to encode the object." );
                    }

                    // Make a copy of the object to send as a GenericRecord, then write that to the memory stream
                    var schema = KineticaData.SchemaFromType( obj.GetType(), ktype );
                    var recordToSend = MakeGenericRecord( obj, ktype );
                    var writer = new Avro.Generic.DefaultWriter(schema);
                    writer.Write(schema, recordToSend, new BinaryEncoder(ms));
                }

                // Get the memory from the stream
                return ms.ToArray();
            }
        }  // end AvroEncode

        /// <summary>
        /// Make a copy of an object as an Avro GenericRecord
        /// </summary>
        /// <param name="obj">Original object</param>
        /// <param name="ktype">An associated KineticaType object that
        /// describes the original object.</param>
        /// <returns>GenericRecord object which is a copy of the specified object</returns>
        private Avro.Generic.GenericRecord MakeGenericRecord( object obj, KineticaType ktype )
        {
            // Get the schema
            var schema = KineticaData.SchemaFromType( obj.GetType(), ktype );

            // Create a new GenericRecord for this schema
            var recordToSend = new Avro.Generic.GenericRecord(schema);

            // Copy each field from obj to recordToSend
            foreach ( var field in schema.Fields)
            {
                var property = obj.GetType()
                                .GetProperties()
                                .FirstOrDefault(prop => prop.Name.ToLowerInvariant() == field.Name.ToLowerInvariant());

                if (property == null) continue;

                recordToSend.Add(field.Name, property.GetValue(obj, null));
            }

            // Return the newly created object
            return recordToSend;
        }

        /// <summary>
        /// Decode binary Avro data into an object.
        /// </summary>
        /// <typeparam name="T">Type of expected object</typeparam>
        /// <param name="bytes">Binary Avro data</param>
        /// <param name="ktype">An optional KineticaType object to help in decoding the object.</param>
        /// <returns>New object</returns>
        private T AvroDecode<T>(byte[] bytes, KineticaType ktype = null) where T : new()
        {
            // Get the schema
            var schema = KineticaData.SchemaFromType( typeof(T), ktype );

            // Create a stream to read the binary data
            using (var ms = new MemoryStream(bytes))
            {
                // Create a new object to return
                T obj = new T();
                if (obj is Avro.Specific.ISpecificRecord)
                {
                    var reader = new Avro.Specific.SpecificDefaultReader(schema, schema);
                    reader.Read(obj, new BinaryDecoder(ms));
                }
                else
                {
                    // Not ISpecificRecord, so first read into a new GenericRecord
                    var reader = new Avro.Generic.DefaultReader(schema, schema);
                    Avro.Generic.GenericRecord recordToReceive = new Avro.Generic.GenericRecord(schema);
                    reader.Read(recordToReceive, new BinaryDecoder(ms));

                    // Now, copy all the fields from the GenericRecord to obj
                    foreach (var field in schema.Fields)
                    {
                        var property = obj.GetType()
                                        .GetProperties()
                                        .FirstOrDefault(prop => prop.Name.ToLowerInvariant() == field.Name.ToLowerInvariant());

                        if (property == null) continue;

                        object val;
                        // Try to get the property
                        if (recordToReceive.TryGetValue(field.Name, out val))
                        {
                            // If successful, write the property to obj
                            property.SetValue(obj, val);
                        }
                    }  // end foreach
                }  // end if-else

                // Return the new object
                return obj;
            }  // end using
        }  // end AvroDecode<T>


        /// <summary>
        /// Decode binary Avro data from a stream into an object
        /// </summary>
        /// <typeparam name="T">Type of expected object</typeparam>
        /// <param name="stream">Stream to read for object data</param>
        /// <returns>New object</returns>
        private T AvroDecode<T>(Stream stream) where T : Avro.Specific.ISpecificRecord, new()
        {
            // T obj = new T(); // Activator.CreateInstance<T>();
            var schema = KineticaData.SchemaFromType( typeof(T), null );
            var reader = new Avro.Specific.SpecificReader<T>(schema, schema);
            return reader.Read(default(T), new BinaryDecoder(stream));
        }
        /*
        private T AvroDecode<T>(string str) where T : new()
        {
            return AvroDecode<T>(Encoding.UTF8.GetBytes(str));
        }
        */
    }  // end class Kinetica
}  // end namespace kinetica


