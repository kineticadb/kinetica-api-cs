﻿using System;
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
    public class KineticaIngestor<T>
    {
        [Serializable]
        public class InsertException<T> : KineticaException
        {
            public Uri url { get; private set; }
            public IList<T> records { get; private set; }
            private string message;

            public InsertException( string msg ) : base( msg ) { }

            internal InsertException( Uri url_, IList<T> records_, string msg ) : base ( msg )
            {
                this.message = msg;
                this.url = url_;
                this.records = records_;
            }

            public override string ToString() { return "InsertException: " + message; }
        }  // end class InsertException




        // KineticaIngestor Members:
        // =========================
        public Kinetica kineticaDB { get; }
        public string table_name { get; }
        public int batch_size { get; }
        public IDictionary<string, string> options { get; }
        //public IReadOnlyDictionary<string, string> options { get; }
        public Int64 count_inserted;
        public Int64 count_updated;
        private KineticaType ktype;
        private Utils.RecordKeyBuilder<T> primary_key_builder;
        private Utils.RecordKeyBuilder<T> shard_key_builder;
        private IList<int> routing_table;
        private IList<Utils.WorkerQueue<T>> worker_queues;
        private Random random;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="kdb"></param>
        /// <param name="table_name"></param>
        /// <param name="batch_size"></param>
        /// <param name="ktype"></param>
        /// <param name="options"></param>
        /// <param name="workers"></param>
        public KineticaIngestor( Kinetica kdb, string table_name,
                                 int batch_size, KineticaType ktype,
                                 Dictionary<string, string> options = null,
                                 Utils.WorkerList workers = null )
        {
            this.kineticaDB = kdb;
            this.table_name = table_name;
            this.ktype = ktype;

            // Validate and save the batch size
            if ( batch_size < 1 )
                throw new KineticaException( $"Batch size must be greater than one; given {batch_size}." );
            this.batch_size = batch_size;

            // Save the options (make it read-only if it exists)
            if ( options != null )
            {
                this.options = options;
                //this.options = options.ToImmutableDictionary<string, string>();
            }
            else
            {
                this.options = null;
            }

            // Set up the primary and shard key builders
            // -----------------------------------------
            this.primary_key_builder = new Utils.RecordKeyBuilder<T>( true,  this.ktype );
            this.shard_key_builder   = new Utils.RecordKeyBuilder<T>( false, this.ktype );

            // Based on the Java implementation
            if ( this.primary_key_builder.hasKey() )
            {   // There is a primary key for the given T
                // Now check if there is a distinct shard key
                if ( !this.shard_key_builder.hasKey()
                     || this.shard_key_builder.hasSameKey( this.primary_key_builder ) )
                    this.shard_key_builder = this.primary_key_builder; // no distinct shard key
            }
            else  // there is no primary key for the given T
            {
                this.primary_key_builder = null;

                // Check if there is shard key for T
                if ( !this.shard_key_builder.hasKey() )
                    this.shard_key_builder = null;
            }  // done setting up the key builders


            // Set up the worker queues
            // -------------------------
            // Do we update records if there are matching primary keys in the
            // database already?
            bool update_on_existing_pk = ( (options != null)
                                           && options.ContainsKey( InsertRecordsRequest<T>.Options.UPDATE_ON_EXISTING_PK )
                                           && options[ InsertRecordsRequest<T>.Options.UPDATE_ON_EXISTING_PK ].Equals( InsertRecordsRequest<T>.Options.TRUE ) );
            // Do T type records have a primary key?
            bool has_primary_key = (this.primary_key_builder != null);
            this.worker_queues = new List<Utils.WorkerQueue<T>>();
            try
            {
                // If no workers are given, try to get them from Kinetica
                if ( ( workers == null ) || ( workers.Count == 0 ) )
                {
                    workers = new Utils.WorkerList( kdb );
                }

                // If we end up with multiple workers, either given by the
                // user or obtained from Kinetica, then use those
                if ( ( workers != null ) && ( workers.Count > 0 ) )
                {
                    // Add worker queues per worker
                    foreach ( System.Uri worker_url in workers )
                    {
                        string insert_records_worker_url_str = (worker_url.ToString() + "insert/records");
                        System.Uri url = new System.Uri( insert_records_worker_url_str );
                        Utils.WorkerQueue<T> worker_queue = new Utils.WorkerQueue<T>( url, batch_size,
                                                                                      has_primary_key,
                                                                                      update_on_existing_pk );
                        this.worker_queues.Add( worker_queue );
                    }

                    // Get the worker rank information from Kinetica
                    this.routing_table = kdb.adminShowShards().rank;
                    // Check that enough worker URLs are specified
                    for ( int i = 0; i < routing_table.Count; ++i )
                    {
                        if ( this.routing_table[i] > this.worker_queues.Count )
                            throw new KineticaException( "Not enough worker URLs specified." );
                    }
                }
                else // multihead-ingest is NOT turned on; use the regular Kinetica IP address
                {
                    string insert_records_url_str = ( kdb.URL.ToString() + "insert/records" );
                    System.Uri url = new System.Uri( insert_records_url_str );
                    Utils.WorkerQueue<T> worker_queue = new Utils.WorkerQueue<T>( url, batch_size, has_primary_key, update_on_existing_pk );
                    this.worker_queues.Add( worker_queue );
                    this.routing_table = null;
                }
            }
            catch ( Exception ex )
            {
                throw new KineticaException( ex.ToString() );
            }

            // Create the random number generator
            this.random = new Random( (int) DateTime.Now.Ticks );
        }   // end constructor KineticaIngestor


        /// <summary>
        /// Returns the count of records inserted so far.  An atomic operation.
        /// </summary>
        /// <returns>The number of records inserted into Kinetica through this
        /// ingestor so far.</returns>
        public Int64 getCountInserted()
        {
            return System.Threading.Interlocked.Read( ref this.count_inserted );
        }


        /// <summary>
        /// Returns the count of records updated so far.  An atomic operation.
        /// </summary>
        /// <returns>The number of records updated into Kinetica through this
        /// ingestor so far.</returns>
        public Int64 getCountUpdated()
        {
            return System.Threading.Interlocked.Read( ref this.count_updated );
        }


        /// <summary>
        /// Ensures that all queued records are inserted into Kinetica.  If an error
        /// occurs while inserting the records from any queue, the recoreds will no
        /// longer be in that queue nor in Kinetica; catch <see cref="InsertException{T}" />
        /// to get the list of records that were being inserted if needed (for example,
        /// to retry).  Other queues may also still contain unflushed records if this
        /// occurs.
        /// </summary>
        /// <exception cref="InsertException{T}" />
        public void flush()
        {
            foreach ( Utils.WorkerQueue<T> worker_queue in this.worker_queues )
            {
                // Flush the queue
                IList<T> queue = worker_queue.flush();
                // Actually insert the records
                flush( queue, worker_queue.url );
            }
        }  // end public flush


        /// <summary>
        /// Insert the given list of records to the database residing at the given URL.
        /// Upon any error, thrown InsertException with the queue of records passed into it.
        /// </summary>
        /// <param name="queue">The list or records to insert.</param>
        /// <param name="url">The address of the Kinetica worker.</param>
        private void flush( IList<T> queue, System.Uri url )
        {
            if ( queue.Count == 0 )
                return; // nothing to do since the queue is empty

            try
            {
                // Create the /insert/records request and response objects
                // -------------------------------------------------------
                // Encode the records into binary
                IList<byte[]> encoded_queue = new List<byte[]>();
                foreach ( var record in queue ) encoded_queue.Add( this.kineticaDB.AvroEncode( record ) );
                RawInsertRecordsRequest request = new RawInsertRecordsRequest( this.table_name, encoded_queue, this.options);

                InsertRecordsResponse response = new InsertRecordsResponse();

                // Make the /insert/records call

                if ( url == null )
                {
                    response = this.kineticaDB.insertRecordsRaw( request );
                }
                else
                {
                    response = this.kineticaDB.SubmitRequest<InsertRecordsResponse>( url, request );
                }

                // Save the counts of inserted and updated records
                System.Threading.Interlocked.Add( ref this.count_inserted, response.count_inserted );
                System.Threading.Interlocked.Add( ref this.count_updated, response.count_updated );
            }
            catch ( Exception ex )
            {
                throw new InsertException<T>( url, queue, ex.Message );
            }
        }  // end private flush()



        /// <summary>
        /// Queues a record for insertion into Kinetica.  If the queue reaches
        /// the <member cref="batch_size" />, all records in the queue will be
        /// inserted into Kinetica before the method returns.  If an error occurs
        /// while inserting the records, the records will no longer be in the queue
        /// nor in Kinetica; catch <see cref="InsertException{T}"/>  to get the list
        /// of records that were being inserted if needed (for example, to retry).
        /// </summary>
        /// <param name="record">The record to insert.</param>
        /// <exception cref="InsertException{T}" />
        public void insert( T record )
        {
            // Create the record keys
            Utils.RecordKey primary_key = null;  // used to check for uniqueness
            Utils.RecordKey shard_key = null;    // used to find which worker to send this record to

            // Build the primary key, if any
            if ( this.primary_key_builder != null )
                primary_key = this.primary_key_builder.build( record );

            // Build the shard/routing key, if any
            if ( this.shard_key_builder != null )
                shard_key = this.shard_key_builder.build( record );

            // Find out which worker to send the record to; then add the record
            // to the approrpriate worker's record queue
            Utils.WorkerQueue<T> worker_queue;
            if ( this.routing_table == null )
            {   // no information regarding multiple workers, so get the first/only one
                worker_queue = this.worker_queues[0];
            }
            else if ( shard_key == null )
            {   // there is no shard/routing key, so get a random worker
                worker_queue = this.worker_queues[ random.Next( this.worker_queues.Count ) ];
            }
            else
            {   // Get the worker based on the sharding/routing key
                int worker_index = shard_key.route( this.routing_table );
                worker_queue = this.worker_queues[worker_index];
            }

            // Insert the record into the queue
            IList<T> queue = worker_queue.insert( record, primary_key );

            // If inserting the queue resulted in flushing the queue, then flush it
            // properly
            if ( queue != null )
            {
                this.flush( queue, worker_queue.url );
            }
        }  // end insert( record )



        /// <summary>
        /// Queues a list of records for insertion into Kientica.  If any queue
        /// reaches the <member cref="batch_size"/>, all records in that queue
        /// will be inserted into Kinetica before the method returns.  If an
        /// error occurs while inserting the queued records,  the records will
        /// no longer be in that queue nor in Kinetica; catch <see cref="InsertException{T}"/> 
        /// to get the list of records that were being inserted (including any
        /// from the queue in question and any remaining in the list not yet
        /// queued) if needed (for example, to retry).  Note that depending on
        /// the number of records, multiple calls to Kinetica may occur.
        /// </summary>
        /// <param name="records">The records to insert.</param>
        /// <exception cref="InsertException{T}"/>
        public void insert( IList<T> records)
        {
            // Insert one record at a time
            for ( int i = 0; i < records.Count; ++i )
            {
                try
                {
                    this.insert( records[ i ] );
                }
                catch ( InsertException<T> ex )
                {
                    // Add the remaining records to the insertion exception
                    // record queue
                    IList<T> queue = ex.records;

                    for ( int j = i + 1; j < records.Count; ++j )
                    {
                        queue.Add( records[ j ] );
                    }

                    // Rethrow
                    throw ex;
                }  // end try-catch
            }  // end outer for loop
        }  // end insert( records )



    }  // end class KineticaIngestor<T>




}  // end namespace kinetica
