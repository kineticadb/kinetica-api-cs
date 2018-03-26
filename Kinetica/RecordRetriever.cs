using Avro.IO;
using System;
using System.Collections.Generic;


namespace kinetica
{
    /// <summary>
    /// Manages the insertion into GPUdb of large numbers of records in bulk,
    /// with automatic batch management and support for multi-head ingest.
    /// Use the <see cref="insert(record)"/> and <see cref="insert(List)"/>
    /// methods to queue records for insertion, and the <see cref="flush"/>
    /// method to ensure that all queued records have been inserted.
    /// </summary>
    /// <typeparam name="T">The type of object being inserted.</typeparam>
    public class RecordRetriever<T> where T : new()
    {

        public Kinetica kineticaDB { get; }
        public string table_name { get; }
        private KineticaType ktype;
        private Utils.RecordKeyBuilder<T> shard_key_builder;
        private IList<int> routing_table;
        private IList<Utils.WorkerQueue<T>> worker_queues;
        private Random random;


        /// <summary>
        /// Create a RecordRetriever object with the given parameters.
        /// </summary>
        /// <param name="kdb"></param>
        /// <param name="table_name"></param>
        /// <param name="ktype"></param>
        /// <param name="workers"></param>
        public RecordRetriever( Kinetica kdb, string table_name,
                                KineticaType ktype,
                                Utils.WorkerList workers = null)
        {
            this.kineticaDB = kdb;
            this.table_name = table_name;
            this.ktype = ktype;

            // Set up the shard key builder
            // ----------------------------
            this.shard_key_builder = new Utils.RecordKeyBuilder<T>(false, this.ktype);
            // Check if there is shard key for T
            if (!this.shard_key_builder.hasKey())
                this.shard_key_builder = null;


            // Set up the worker queues
            // -------------------------
            this.worker_queues = new List<Utils.WorkerQueue<T>>();
            try
            {
                // If no workers are given, try to get them from Kinetica
                if ((workers == null) || (workers.Count == 0))
                {
                    workers = new Utils.WorkerList(kdb);
                }

                // If we end up with multiple workers, either given by the
                // user or obtained from Kinetica, then use those
                if ((workers != null) && (workers.Count > 0))
                {
                    // Add worker queues per worker
                    foreach (System.Uri worker_url in workers)
                    {
                        string get_records_worker_url_str = (worker_url.ToString() + "get/records");
                        System.Uri url = new System.Uri( get_records_worker_url_str );
                        Utils.WorkerQueue<T> worker_queue = new Utils.WorkerQueue<T>( url );
                        this.worker_queues.Add( worker_queue );
                    }

                    // Get the worker rank information from Kinetica
                    this.routing_table = kdb.adminShowShards().rank;
                    // Check that enough worker URLs are specified
                    for (int i = 0; i < routing_table.Count; ++i)
                    {
                        if (this.routing_table[i] > this.worker_queues.Count)
                            throw new KineticaException("Not enough worker URLs specified.");
                    }
                }
                else // multihead-ingest is NOT turned on; use the regular Kinetica IP address
                {
                    string get_records_url_str = ( kdb.URL.ToString() + "get/records" );
                    System.Uri url = new System.Uri( get_records_url_str );
                    Utils.WorkerQueue<T> worker_queue = new Utils.WorkerQueue<T>( url );
                    this.worker_queues.Add(worker_queue);
                    this.routing_table = null;
                }
            }
            catch (Exception ex)
            {
                throw new KineticaException(ex.ToString());
            }

            // Create the random number generator
            this.random = new Random((int)DateTime.Now.Ticks);
        }   // end constructor RecordRetriever


        /// <summary>
        /// Retrieves records for a given shard key, optionally further limited by an
        /// additional expression. All records matching the key and satisfying the
        /// expression will be returned, up to the system-defined limit. For
        /// multi-head mode the request will be sent directly to the appropriate
        /// worker.
        /// <br />
        /// All fields in both the shard key and the expression must have defined
        /// attribute indexes, unless the shard key is also a primary key and all
        /// referenced fields are in the primary key. The expression must be
        /// limited to basic equality and inequality comparisons that can be
        /// evaluated using the attribute indexes.
        /// </summary>
        /// 
        /// <param name="record">The record based on whose shard column values
        /// records will be fetched from the table.</param>
        /// <param name="expression">An optional expression.  Default is
        /// null.</param>
        /// 
        /// <returns>A GetRecordsResponse object with the decoded retrieved
        /// values.</returns>
        public GetRecordsResponse<T> getRecordsByKey( T record,
                                                      string expression = null )
        {
            if ( this.shard_key_builder == null)
                throw new KineticaException( "Cannot get by key from unsharded table: " + this.table_name );

            try
            {
                // Build the expression
                string full_expression = this.shard_key_builder.buildExpression( record );
                if ( full_expression == null )
                    throw new KineticaException( "No expression could be made from given record." );
                if ( expression != null )
                    full_expression = (full_expression + " and (" + expression + ")");

                // Create the options map for the /get/records call
                IDictionary<string, string> options = new Dictionary<string, string>();
                options[GetRecordsRequest.Options.EXPRESSION] = full_expression;
                options[GetRecordsRequest.Options.FAST_INDEX_LOOKUP] = GetRecordsRequest.Options.TRUE;

                // Create a /get/records request packet
                GetRecordsRequest request = new GetRecordsRequest( this.table_name,
                                                                   0, Kinetica.END_OF_SET,
                                                                   options );

                // Submit the /get/records request
                if ( this.routing_table == null )
                {   // No routing information is available; talk to rank-0
                    return kineticaDB.getRecords<T>( request );
                }
                else // Talk to the appropriate worker rank
                {
                    // Create the appropriate response objects
                    RawGetRecordsResponse raw_response     = new RawGetRecordsResponse();
                    GetRecordsResponse<T> decoded_response = new GetRecordsResponse<T>();

                    // Find the appropriate worker rank
                    Utils.RecordKey shard_key = this.shard_key_builder.build( record );
                    System.Uri url = this.worker_queues[ shard_key.route( this.routing_table ) ].url;
                    // Make the call
                    raw_response = this.kineticaDB.SubmitRequest<RawGetRecordsResponse>( url, request );

                    // Set up the values of the decoded response properly
                    decoded_response.table_name = raw_response.table_name;
                    decoded_response.type_name = raw_response.type_name;
                    decoded_response.type_schema = raw_response.type_schema;
                    decoded_response.has_more_records = raw_response.has_more_records;
                    decoded_response.total_number_of_records = raw_response.total_number_of_records;

                    // Decode the records
                    kineticaDB.DecodeRawBinaryDataUsingRecordType( ktype,
                                                                   raw_response.records_binary,
                                                                   decoded_response.data );
                    return decoded_response;
                }
            } catch ( KineticaException ex )
            {
                throw new KineticaException( "Error in retrieving records by key: ", ex );
            } catch ( Exception ex )
            {
                throw new KineticaException( "Error in retrieving records by key: ", ex );
            }
        }  // end getRecordsByKey()

    }   // end class RecordRetriever

}   // end namespace kinetica
