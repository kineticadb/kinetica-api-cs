/*
 *  This file was autogenerated by the Kinetica schema processor.
 *
 *  DO NOT EDIT DIRECTLY.
 */

using System.Collections.Generic;



namespace kinetica
{

    /// <summary>A set of parameters for <see
    /// cref="Kinetica.alterSystemProperties(IDictionary{string, string},IDictionary{string, string})"
    /// />.
    /// <br />
    /// The <see
    /// cref="Kinetica.alterSystemProperties(IDictionary{string, string},IDictionary{string, string})"
    /// /> endpoint is primarily used to simplify the testing of the system and
    /// is not expected to be used during normal execution.  Commands are given
    /// through the <see cref="property_updates_map" /> whose keys are commands
    /// and values are strings representing integer values (for example '8000')
    /// or boolean values ('true' or 'false').</summary>
    public class AlterSystemPropertiesRequest : KineticaData
    {

        /// <summary>Map containing the properties of the system to be updated.
        /// Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SM_OMP_THREADS">SM_OMP_THREADS</see>:</term>
        ///         <description>Set the number of OpenMP threads that will be
        /// used to service filter & aggregation requests against collections
        /// to the specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.KERNEL_OMP_THREADS">KERNEL_OMP_THREADS</see>:</term>
        ///         <description>Set the number of kernel OpenMP threads to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CONCURRENT_KERNEL_EXECUTION">CONCURRENT_KERNEL_EXECUTION</see>:</term>
        ///         <description>Enables concurrent kernel execution if the
        /// value is <i>true</i> and disables it if the value is <i>false</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SUBTASK_CONCURRENCY_LIMIT">SUBTASK_CONCURRENCY_LIMIT</see>:</term>
        ///         <description>Sets the maximum number of simultaneous
        /// threads allocated to a given request, on each rank. Note that
        /// thread allocation may also be limted by resource group limits
        /// and/or system load.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Sets the chunk size of all new sets to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.EXECUTION_MODE">EXECUTION_MODE</see>:</term>
        ///         <description>Sets the execution_mode for kernel executions
        /// to the specified string value. Possible values are host, device,
        /// default (engine decides) or an integer value that indicates max
        /// chunk size to exec on host</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FLUSH_TO_DISK">FLUSH_TO_DISK</see>:</term>
        ///         <description>Flushes any changes to any tables to the
        /// persistent store.  These changes include updates to the vector
        /// store, object store, and text search store, Value string is
        /// ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CLEAR_CACHE">CLEAR_CACHE</see>:</term>
        ///         <description>Clears cached results.  Useful to allow
        /// repeated timing of endpoints. Value string is ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.COMMUNICATOR_TEST">COMMUNICATOR_TEST</see>:</term>
        ///         <description>Invoke the communicator test and report timing
        /// results. Value string is is a semicolon separated list of
        /// <key>=<value> expressions.  Expressions are: num_transactions=<num>
        /// where num is the number of request reply transactions to invoke per
        /// test; message_size=<bytes> where bytes is the size of the messages
        /// to send in bytes; check_values=<enabled> where if enabled is true
        /// the value of the messages received are verified.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SET_MESSAGE_TIMERS_ENABLED">SET_MESSAGE_TIMERS_ENABLED</see>:</term>
        ///         <description>Enables the communicator test to collect
        /// additional timing statistics when the value string is <i>true</i>.
        /// Disables the collection when the value string is <i>false</i>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.BULK_ADD_TEST">BULK_ADD_TEST</see>:</term>
        ///         <description>Invoke the bulk add test and report timing
        /// results. Value string is ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.NETWORK_SPEED">NETWORK_SPEED</see>:</term>
        ///         <description>Invoke the network speed test and report
        /// timing results. Value string is a semicolon-separated list of
        /// <key>=<value> expressions.  Valid expressions are: seconds=<time>
        /// where time is the time in seconds to run the test; data_size=<size>
        /// where size is the size in bytes of the block to be transferred;
        /// threads=<number of threads>; to_ranks=<space-separated list of
        /// ranks> where the list of ranks is the ranks that rank 0 will send
        /// data to and get data from. If to_ranks is unspecified then all
        /// worker ranks are used.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.REQUEST_TIMEOUT">REQUEST_TIMEOUT</see>:</term>
        ///         <description>Number of minutes after which filtering (e.g.,
        /// /filter) and aggregating (e.g., /aggregate/groupby) queries will
        /// timeout.  The default value is '20'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.MAX_GET_RECORDS_SIZE">MAX_GET_RECORDS_SIZE</see>:</term>
        ///         <description>The maximum number of records the database
        /// will serve for a given data retrieval call.  The default value is
        /// '20000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.ENABLE_AUDIT">ENABLE_AUDIT</see>:</term>
        ///         <description>Enable or disable auditing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_HEADERS">AUDIT_HEADERS</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// headers.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_BODY">AUDIT_BODY</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// bodies.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_DATA">AUDIT_DATA</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_ENABLED">CHUNK_CACHE_ENABLED</see>:</term>
        ///         <description>Enable chunk level query caching. Flushes the
        /// chunk cache when value is false</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_SIZE">CHUNK_CACHE_SIZE</see>:</term>
        ///         <description>Size of the chunk cache in bytes.  The default
        /// value is '10000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SYNCHRONOUS_COMPRESSION">SYNCHRONOUS_COMPRESSION</see>:</term>
        ///         <description>compress vector on set_compression (instead of
        /// waiting for background thread).  The default value is
        /// 'false'.</description>
        ///     </item>
        /// </list>
        /// <br />
        /// A set of string constants for the parameter <see
        /// cref="property_updates_map" />.</summary>
        public struct PropertyUpdatesMap
        {

            /// <summary>Set the number of OpenMP threads that will be used to
            /// service filter & aggregation requests against collections to
            /// the specified integer value.</summary>
            public const string SM_OMP_THREADS = "sm_omp_threads";

            /// <summary>Set the number of kernel OpenMP threads to the
            /// specified integer value.</summary>
            public const string KERNEL_OMP_THREADS = "kernel_omp_threads";

            /// <summary>Enables concurrent kernel execution if the value is
            /// <i>true</i> and disables it if the value is <i>false</i>.
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
            ///     </item>
            /// </list></summary>
            public const string CONCURRENT_KERNEL_EXECUTION = "concurrent_kernel_execution";
            public const string TRUE = "true";
            public const string FALSE = "false";

            /// <summary>Sets the maximum number of simultaneous threads
            /// allocated to a given request, on each rank. Note that thread
            /// allocation may also be limted by resource group limits and/or
            /// system load.</summary>
            public const string SUBTASK_CONCURRENCY_LIMIT = "subtask_concurrency_limit";

            /// <summary>Sets the chunk size of all new sets to the specified
            /// integer value.</summary>
            public const string CHUNK_SIZE = "chunk_size";

            /// <summary>Sets the execution_mode for kernel executions to the
            /// specified string value. Possible values are host, device,
            /// default (engine decides) or an integer value that indicates max
            /// chunk size to exec on host</summary>
            public const string EXECUTION_MODE = "execution_mode";

            /// <summary>Flushes any changes to any tables to the persistent
            /// store.  These changes include updates to the vector store,
            /// object store, and text search store, Value string is
            /// ignored</summary>
            public const string FLUSH_TO_DISK = "flush_to_disk";

            /// <summary>Clears cached results.  Useful to allow repeated
            /// timing of endpoints. Value string is ignored</summary>
            public const string CLEAR_CACHE = "clear_cache";

            /// <summary>Invoke the communicator test and report timing
            /// results. Value string is is a semicolon separated list of
            /// <key>=<value> expressions.  Expressions are:
            /// num_transactions=<num> where num is the number of request reply
            /// transactions to invoke per test; message_size=<bytes> where
            /// bytes is the size of the messages to send in bytes;
            /// check_values=<enabled> where if enabled is true the value of
            /// the messages received are verified.</summary>
            public const string COMMUNICATOR_TEST = "communicator_test";

            /// <summary>Enables the communicator test to collect additional
            /// timing statistics when the value string is <i>true</i>.
            /// Disables the collection when the value string is <i>false</i>
            /// Supported values:
            /// <list type="bullet">
            ///     <item>
            ///         <term><see
            /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
            ///     </item>
            ///     <item>
            ///         <term><see
            /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
            ///     </item>
            /// </list></summary>
            public const string SET_MESSAGE_TIMERS_ENABLED = "set_message_timers_enabled";

            /// <summary>Invoke the bulk add test and report timing results.
            /// Value string is ignored.</summary>
            public const string BULK_ADD_TEST = "bulk_add_test";

            /// <summary>Invoke the network speed test and report timing
            /// results. Value string is a semicolon-separated list of
            /// <key>=<value> expressions.  Valid expressions are:
            /// seconds=<time> where time is the time in seconds to run the
            /// test; data_size=<size> where size is the size in bytes of the
            /// block to be transferred; threads=<number of threads>;
            /// to_ranks=<space-separated list of ranks> where the list of
            /// ranks is the ranks that rank 0 will send data to and get data
            /// from. If to_ranks is unspecified then all worker ranks are
            /// used.</summary>
            public const string NETWORK_SPEED = "network_speed";

            /// <summary>Number of minutes after which filtering (e.g., <see
            /// cref="Kinetica.filter(string,string,string,IDictionary{string, string})"
            /// />) and aggregating (e.g., <see
            /// cref="Kinetica.aggregateGroupBy(string,IList{string},long,long,IDictionary{string, string})"
            /// />) queries will timeout.  The default value is '20'.</summary>
            public const string REQUEST_TIMEOUT = "request_timeout";

            /// <summary>The maximum number of records the database will serve
            /// for a given data retrieval call.  The default value is
            /// '20000'.</summary>
            public const string MAX_GET_RECORDS_SIZE = "max_get_records_size";

            /// <summary>Enable or disable auditing.</summary>
            public const string ENABLE_AUDIT = "enable_audit";

            /// <summary>Enable or disable auditing of request
            /// headers.</summary>
            public const string AUDIT_HEADERS = "audit_headers";

            /// <summary>Enable or disable auditing of request
            /// bodies.</summary>
            public const string AUDIT_BODY = "audit_body";

            /// <summary>Enable or disable auditing of request data.</summary>
            public const string AUDIT_DATA = "audit_data";

            /// <summary>Enable chunk level query caching. Flushes the chunk
            /// cache when value is false</summary>
            public const string CHUNK_CACHE_ENABLED = "chunk_cache_enabled";

            /// <summary>Size of the chunk cache in bytes.  The default value
            /// is '10000000'.</summary>
            public const string CHUNK_CACHE_SIZE = "chunk_cache_size";

            /// <summary>compress vector on set_compression (instead of waiting
            /// for background thread).  The default value is
            /// 'false'.</summary>
            public const string SYNCHRONOUS_COMPRESSION = "synchronous_compression";
        } // end struct PropertyUpdatesMap


        /// <summary>Map containing the properties of the system to be updated.
        /// Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SM_OMP_THREADS">SM_OMP_THREADS</see>:</term>
        ///         <description>Set the number of OpenMP threads that will be
        /// used to service filter & aggregation requests against collections
        /// to the specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.KERNEL_OMP_THREADS">KERNEL_OMP_THREADS</see>:</term>
        ///         <description>Set the number of kernel OpenMP threads to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CONCURRENT_KERNEL_EXECUTION">CONCURRENT_KERNEL_EXECUTION</see>:</term>
        ///         <description>Enables concurrent kernel execution if the
        /// value is <i>true</i> and disables it if the value is <i>false</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SUBTASK_CONCURRENCY_LIMIT">SUBTASK_CONCURRENCY_LIMIT</see>:</term>
        ///         <description>Sets the maximum number of simultaneous
        /// threads allocated to a given request, on each rank. Note that
        /// thread allocation may also be limted by resource group limits
        /// and/or system load.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Sets the chunk size of all new sets to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.EXECUTION_MODE">EXECUTION_MODE</see>:</term>
        ///         <description>Sets the execution_mode for kernel executions
        /// to the specified string value. Possible values are host, device,
        /// default (engine decides) or an integer value that indicates max
        /// chunk size to exec on host</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FLUSH_TO_DISK">FLUSH_TO_DISK</see>:</term>
        ///         <description>Flushes any changes to any tables to the
        /// persistent store.  These changes include updates to the vector
        /// store, object store, and text search store, Value string is
        /// ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CLEAR_CACHE">CLEAR_CACHE</see>:</term>
        ///         <description>Clears cached results.  Useful to allow
        /// repeated timing of endpoints. Value string is ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.COMMUNICATOR_TEST">COMMUNICATOR_TEST</see>:</term>
        ///         <description>Invoke the communicator test and report timing
        /// results. Value string is is a semicolon separated list of
        /// <key>=<value> expressions.  Expressions are: num_transactions=<num>
        /// where num is the number of request reply transactions to invoke per
        /// test; message_size=<bytes> where bytes is the size of the messages
        /// to send in bytes; check_values=<enabled> where if enabled is true
        /// the value of the messages received are verified.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SET_MESSAGE_TIMERS_ENABLED">SET_MESSAGE_TIMERS_ENABLED</see>:</term>
        ///         <description>Enables the communicator test to collect
        /// additional timing statistics when the value string is <i>true</i>.
        /// Disables the collection when the value string is <i>false</i>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.BULK_ADD_TEST">BULK_ADD_TEST</see>:</term>
        ///         <description>Invoke the bulk add test and report timing
        /// results. Value string is ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.NETWORK_SPEED">NETWORK_SPEED</see>:</term>
        ///         <description>Invoke the network speed test and report
        /// timing results. Value string is a semicolon-separated list of
        /// <key>=<value> expressions.  Valid expressions are: seconds=<time>
        /// where time is the time in seconds to run the test; data_size=<size>
        /// where size is the size in bytes of the block to be transferred;
        /// threads=<number of threads>; to_ranks=<space-separated list of
        /// ranks> where the list of ranks is the ranks that rank 0 will send
        /// data to and get data from. If to_ranks is unspecified then all
        /// worker ranks are used.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.REQUEST_TIMEOUT">REQUEST_TIMEOUT</see>:</term>
        ///         <description>Number of minutes after which filtering (e.g.,
        /// /filter) and aggregating (e.g., /aggregate/groupby) queries will
        /// timeout.  The default value is '20'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.MAX_GET_RECORDS_SIZE">MAX_GET_RECORDS_SIZE</see>:</term>
        ///         <description>The maximum number of records the database
        /// will serve for a given data retrieval call.  The default value is
        /// '20000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.ENABLE_AUDIT">ENABLE_AUDIT</see>:</term>
        ///         <description>Enable or disable auditing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_HEADERS">AUDIT_HEADERS</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// headers.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_BODY">AUDIT_BODY</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// bodies.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_DATA">AUDIT_DATA</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_ENABLED">CHUNK_CACHE_ENABLED</see>:</term>
        ///         <description>Enable chunk level query caching. Flushes the
        /// chunk cache when value is false</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_SIZE">CHUNK_CACHE_SIZE</see>:</term>
        ///         <description>Size of the chunk cache in bytes.  The default
        /// value is '10000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SYNCHRONOUS_COMPRESSION">SYNCHRONOUS_COMPRESSION</see>:</term>
        ///         <description>compress vector on set_compression (instead of
        /// waiting for background thread).  The default value is
        /// 'false'.</description>
        ///     </item>
        /// </list>
        ///   </summary>
        public IDictionary<string, string> property_updates_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Optional parameters.  The default value is an empty {@link
        /// Dictionary}.</summary>
        public IDictionary<string, string> options { get; set; } = new Dictionary<string, string>();


        /// <summary>Constructs an AlterSystemPropertiesRequest object with
        /// default parameters.</summary>
        public AlterSystemPropertiesRequest() { }

        /// <summary>Constructs an AlterSystemPropertiesRequest object with the
        /// specified parameters.</summary>
        /// 
        /// <param name="property_updates_map">Map containing the properties of
        /// the system to be updated. Error if empty.
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SM_OMP_THREADS">SM_OMP_THREADS</see>:</term>
        ///         <description>Set the number of OpenMP threads that will be
        /// used to service filter & aggregation requests against collections
        /// to the specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.KERNEL_OMP_THREADS">KERNEL_OMP_THREADS</see>:</term>
        ///         <description>Set the number of kernel OpenMP threads to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CONCURRENT_KERNEL_EXECUTION">CONCURRENT_KERNEL_EXECUTION</see>:</term>
        ///         <description>Enables concurrent kernel execution if the
        /// value is <i>true</i> and disables it if the value is <i>false</i>.
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SUBTASK_CONCURRENCY_LIMIT">SUBTASK_CONCURRENCY_LIMIT</see>:</term>
        ///         <description>Sets the maximum number of simultaneous
        /// threads allocated to a given request, on each rank. Note that
        /// thread allocation may also be limted by resource group limits
        /// and/or system load.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_SIZE">CHUNK_SIZE</see>:</term>
        ///         <description>Sets the chunk size of all new sets to the
        /// specified integer value.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.EXECUTION_MODE">EXECUTION_MODE</see>:</term>
        ///         <description>Sets the execution_mode for kernel executions
        /// to the specified string value. Possible values are host, device,
        /// default (engine decides) or an integer value that indicates max
        /// chunk size to exec on host</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FLUSH_TO_DISK">FLUSH_TO_DISK</see>:</term>
        ///         <description>Flushes any changes to any tables to the
        /// persistent store.  These changes include updates to the vector
        /// store, object store, and text search store, Value string is
        /// ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CLEAR_CACHE">CLEAR_CACHE</see>:</term>
        ///         <description>Clears cached results.  Useful to allow
        /// repeated timing of endpoints. Value string is ignored</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.COMMUNICATOR_TEST">COMMUNICATOR_TEST</see>:</term>
        ///         <description>Invoke the communicator test and report timing
        /// results. Value string is is a semicolon separated list of
        /// <key>=<value> expressions.  Expressions are: num_transactions=<num>
        /// where num is the number of request reply transactions to invoke per
        /// test; message_size=<bytes> where bytes is the size of the messages
        /// to send in bytes; check_values=<enabled> where if enabled is true
        /// the value of the messages received are verified.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SET_MESSAGE_TIMERS_ENABLED">SET_MESSAGE_TIMERS_ENABLED</see>:</term>
        ///         <description>Enables the communicator test to collect
        /// additional timing statistics when the value string is <i>true</i>.
        /// Disables the collection when the value string is <i>false</i>
        /// Supported values:
        /// <list type="bullet">
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.TRUE">TRUE</see></term>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.FALSE">FALSE</see></term>
        ///     </item>
        /// </list></description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.BULK_ADD_TEST">BULK_ADD_TEST</see>:</term>
        ///         <description>Invoke the bulk add test and report timing
        /// results. Value string is ignored.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.NETWORK_SPEED">NETWORK_SPEED</see>:</term>
        ///         <description>Invoke the network speed test and report
        /// timing results. Value string is a semicolon-separated list of
        /// <key>=<value> expressions.  Valid expressions are: seconds=<time>
        /// where time is the time in seconds to run the test; data_size=<size>
        /// where size is the size in bytes of the block to be transferred;
        /// threads=<number of threads>; to_ranks=<space-separated list of
        /// ranks> where the list of ranks is the ranks that rank 0 will send
        /// data to and get data from. If to_ranks is unspecified then all
        /// worker ranks are used.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.REQUEST_TIMEOUT">REQUEST_TIMEOUT</see>:</term>
        ///         <description>Number of minutes after which filtering (e.g.,
        /// /filter) and aggregating (e.g., /aggregate/groupby) queries will
        /// timeout.  The default value is '20'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.MAX_GET_RECORDS_SIZE">MAX_GET_RECORDS_SIZE</see>:</term>
        ///         <description>The maximum number of records the database
        /// will serve for a given data retrieval call.  The default value is
        /// '20000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.ENABLE_AUDIT">ENABLE_AUDIT</see>:</term>
        ///         <description>Enable or disable auditing.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_HEADERS">AUDIT_HEADERS</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// headers.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_BODY">AUDIT_BODY</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// bodies.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.AUDIT_DATA">AUDIT_DATA</see>:</term>
        ///         <description>Enable or disable auditing of request
        /// data.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_ENABLED">CHUNK_CACHE_ENABLED</see>:</term>
        ///         <description>Enable chunk level query caching. Flushes the
        /// chunk cache when value is false</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.CHUNK_CACHE_SIZE">CHUNK_CACHE_SIZE</see>:</term>
        ///         <description>Size of the chunk cache in bytes.  The default
        /// value is '10000000'.</description>
        ///     </item>
        ///     <item>
        ///         <term><see
        /// cref="AlterSystemPropertiesRequest.PropertyUpdatesMap.SYNCHRONOUS_COMPRESSION">SYNCHRONOUS_COMPRESSION</see>:</term>
        ///         <description>compress vector on set_compression (instead of
        /// waiting for background thread).  The default value is
        /// 'false'.</description>
        ///     </item>
        /// </list>
        ///   </param>
        /// <param name="options">Optional parameters.  The default value is an
        /// empty {@link Dictionary}.</param>
        /// 
        public AlterSystemPropertiesRequest( IDictionary<string, string> property_updates_map,
                                             IDictionary<string, string> options = null)
        {
            this.property_updates_map = property_updates_map ?? new Dictionary<string, string>();
            this.options = options ?? new Dictionary<string, string>();
        } // end constructor

    } // end class AlterSystemPropertiesRequest



    /// <summary>A set of results returned by <see
    /// cref="Kinetica.alterSystemProperties(IDictionary{string, string},IDictionary{string, string})"
    /// />.</summary>
    public class AlterSystemPropertiesResponse : KineticaData
    {

        /// <summary>map of values updated, For speed tests a map of values
        /// measured to the measurement  </summary>
        public IDictionary<string, string> updated_properties_map { get; set; } = new Dictionary<string, string>();

        /// <summary>Additional information.  </summary>
        public IDictionary<string, string> info { get; set; } = new Dictionary<string, string>();

    } // end class AlterSystemPropertiesResponse




}  // end namespace kinetica
