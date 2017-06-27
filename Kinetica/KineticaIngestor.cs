using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;


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


        /// <summary>
        /// A list of worker URLs to use for multi-head ingest.
        /// </summary>
        public sealed class WorkerList : List<System.Uri>
        {
            /// <summary>
            /// Creates an empty {@link WorkerList} that can be populated manually
            /// with worker URLs to support multi-head ingest.Note that worker URLs
            /// must be added in rank order, starting with rank 1, and all worker
            /// ranks must be included; otherwise insertion may fail for certain
            /// data types.
            /// </summary>
            public WorkerList() { }

            /// <summary>
            /// Creates a <see cref="WorkerList"/> object and automatically populates it with the
            /// worker URLs from GPUdb to support multi-head ingest. ( If the
            /// specified GPUdb instance has multi-head ingest disabled, the worker
            /// list will be empty and multi-head ingest will not be used.) Note that
            /// in some cases, workers may be configured to use more than one IP
            /// address, not all of which may be accessible to the client; this
            /// constructor uses the first IP returned by the server for each worker.
            /// </summary>
            /// 
            /// <param name="db">The <see cref="Kinetica"/> instance from which to
            /// obtain the worker URLs.</param>
            /// <param name="ip_regex">Optional IP regex to match.</param>
            public WorkerList( Kinetica db, Regex ip_regex = null )
            {
                // Get the system properties from the database server
                IDictionary<string, string> system_properties = db.showSystemProperties().property_map;

                // Find out if multi-head ingest is turned on or not
                string multi_head_ingestion_param;
                system_properties.TryGetValue( ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS, out multi_head_ingestion_param );
                if ( multi_head_ingestion_param == null )
                    throw new KineticaException( "Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_ENABLE_WORKER_HTTP_SERVERS );
                bool is_multi_head_ingest_enabled = multi_head_ingestion_param.Equals( ShowSystemPropertiesResponse.PropertyMap.TRUE );

                // Nothing to do if multi-head ingestion is disabled
                if ( !is_multi_head_ingest_enabled )
                    return;

                // Get the worker IPs and ports
                string worker_ips_str, worker_ports_str;
                system_properties.TryGetValue( ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS, out worker_ips_str );
                system_properties.TryGetValue( ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS, out worker_ports_str );

                // Check that we got them
                if ( worker_ips_str.Length == 0 )
                    throw new KineticaException( "Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS  );
                if ( worker_ports_str.Length == 0 )
                    throw new KineticaException( "Missing value for " + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS );

                // Parse the IPs and the ports
                // ---------------------------
                // Split the strings
                string[] worker_ip_lists = worker_ips_str.Split( ';' );
                string[] worker_ports    = worker_ports_str.Split( ';' );

                // Check that there are the same number of IPs and ports supplied
                if ( worker_ip_lists.Length != worker_ports.Length )
                    throw new KineticaException( "Inconsistent number of values for "
                                                 + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_IPS
                                                 + " and "
                                                 + ShowSystemPropertiesResponse.PropertyMap.CONF_WORKER_HTTP_SERVER_PORTS );
                // Create the URLs using the IPs and the ports, but
                // ignore the very first rank (rank-0)
                for ( int i = 1; i < worker_ip_lists.Length; ++i )
                {
                    string ip_list = worker_ip_lists[ i ];

                    // Need to split each of the IP lists on a comma
                    string[] ips = ip_list.Split( ',' );

                    bool matching_ip_found = false;

                    // Find at least one IP to work with
                    foreach ( string ip in ips )
                    {
                        // Try to create the URL
                        try
                        {
                            // If a regular expression is given, then see if this one is a match
                            if ( ip_regex != null )
                                matching_ip_found = ip_regex.IsMatch( ip );
                            else  // no regex given, so take the first IP encountered for this worker
                                matching_ip_found = true;

                            if ( matching_ip_found )
                            {
                                UriBuilder uri_builder = new UriBuilder( "http", ip, Int32.Parse( worker_ports[ i ] ), "insert/records" );
                                Uri url = uri_builder.Uri;

                                // Add the URL to this WorkerList
                                this.Add( url );
                                break;  // don't keep trying to match IPs in this group
                            }  // end inner if
                        }  // end try
                        catch ( Exception ex )
                        {
                            throw new KineticaException( ex.Message );
                        }
                    }  // end inner foreach

                    if ( !matching_ip_found )
                        throw new KineticaException( $"No matching IP found for worker #{i}." );
                }  // end outer for

                // Check that this list is not empty
                if ( this.Count == 0 )
                    throw new KineticaException( "No worker HTTP servers found." );
            }  // end constructor

        }  // end class WorkerList



        /// <summary>
        /// A key based on a given record that serves as either a primary key
        /// or a shard key.  The <see cref="RecordKeyBuilder{T}"/> class creates
        /// these record keys.
        /// </summary>
        private sealed class RecordKey
        {
            /// <summary>
            /// YYYY-MM-DD
            /// </summary>
            private static readonly Regex DATE_REGEX = new Regex( "\\A(\\d{4})-(\\d{2})-(\\d{2})$" );
            /// <summary>
            /// Decimal number with upto 19 digits of precision and 4 digits of scale
            /// </summary>
            private static readonly Regex DECIMAL_REGEX = new Regex( "\\A\\s*[+-]?((?<int>\\d+)(\\.(?<frac>\\d{0,4}))?|\\.(?<frac>\\d{1,4}))\\s*\\z" );
            /// <summary>
            /// xxx.xxx.xxx.xxx (where xxx is in the range [0, 255])
            /// </summary>
            private static readonly Regex IPV4_REGEX = new Regex( "\\A(?<a>\\d{1,3})\\.(?<b>\\d{1,3})\\.(?<c>\\d{1,3})\\.(?<d>\\d{1,3})$" );
            /// <summary>
            /// HH:MM:SS[.mmm]
            /// </summary>
            private static readonly Regex TIME_REGEX = new Regex( "\\A(?<hour>\\d{1,2}):(?<minute>\\d{2}):(?<seconds>\\d{2})(\\.(?<milliseconds>\\d{3}))?$" );
            /// <summary>
            /// The day farthest back into the past from the unix epoch (1970-01-01)
            /// </summary>
            private static readonly DateTime MIN_DATE = (new System.Globalization.GregorianCalendar()).MinSupportedDateTime;
            //private readonly DateTime MIN_DATE = DateTime.MinValue;
            /// <summary>
            /// Minimum supported year by Kinetica is 1000
            /// </summary>
            private static readonly int MIN_SUPPORTED_YEAR = 1000;
            /// <summary>
            /// Maximum supported year by Kinetica is 2900
            /// </summary>
            private static readonly int MAX_SUPPORTED_YEAR = 2900;
            /// <summary>
            /// The UTC timezone
            /// </summary>
            private static readonly TimeZoneInfo UTC = TimeZoneInfo.Utc;

            private readonly byte[] buffer;
            private readonly int buffer_size;
            private int current_size;
            private int hash_code;
            private bool is_valid;
            private long routingHash;

            /// <summary>
            /// Allocate the buffer for the record key with the given size.
            /// </summary>
            /// <param name="size">The size of the buffer.  Must be greater
            /// than or equal to 1.</param>
            public RecordKey( int size )
            {
                if ( size < 1 )
                    throw new KineticaException( "Buffer size must be greater than or equal to 1.  "
                                                  + "Size given: " + size );
                buffer_size  = size;
                current_size = 0;
                buffer       = new byte[size];
                this.is_valid = true;
            }

            /// <summary>
            /// Returns whether the key is valid or not.
            /// </summary>
            /// <returns>A bool indicating whether the key is valid at the moment.</returns>
            public bool isValid()
            {
                return this.is_valid;
            }

            /// <summary>
            /// Returns key's hash code.
            /// </summary>
            /// <returns>The hash code for the key.</returns>
            public int hashCode()
            {
                return this.hash_code;
            }



            /// <summary>
            /// Check whether the buffer is already full.
            /// </summary>
            /// <param name="throw_if_full">Optional value; if true, then
            /// throw if the buffer is full.  Default value is true.</param>
            /// <returns>Indicates if the buffer is full.</returns>
            private bool isBufferFull( bool throw_if_full = true )
            {
                if ( this.current_size == this.buffer_size )
                {
                    if ( throw_if_full )
                        throw new KineticaException( "The buffer is already full!" );
                    return true; // yes, the buffer is full, and we haven't thrown
                }
                return false; // buffer is NOT full
            }  // end isBufferFull

            /// <summary>
            /// Check whether the buffer will overflow if we attempt to add
            /// <paramref name="n"/> more bytes.
            /// </summary>
            /// <param name="n">The number of bytes we're attempting to add to
            /// the buffer.</param>
            /// <param name="throw_if_overflow">Optional boolean flag; if true,
            /// then throw an exception if the buffer has insufficient room in it.
            /// Default value is true.</param>
            /// <returns>Boolean flag indicating if the buffer will overflow upon an
            /// attempt to put <paramref name="n"/> more bytes in it.</returns>
            private bool willBufferOverflow( int n, bool throw_if_overflow = true )
            {
                // Note: We're not checking for a negative value for n here
                if ( (this.current_size + n) > this.buffer_size )
                {
                    if ( throw_if_overflow )
                        throw new KineticaException( $"The buffer (of size {buffer_size}) does not have sufficient room in it to put {n} more byte(s) (current size is {this.current_size})." );
                    return true; // yes, the buffer WILL overflow, but we haven't thrown
                }
                return false; // buffer will NOT overflow
            }  // end willBufferOverflow


            /// <summary>
            /// Adds a byte to the byte buffer and increments the current size
            /// by one.  Use ONLY this method to add to the buffer; do not add
            /// to buffer in other methods directly.
            /// </summary>
            /// <param name="b">The byte to be added to the buffer.</param>
            private void add( byte b )
            {
                // Add the byte to the buffer and increment the size
                buffer.SetValue( b, current_size++ );
            } // end add()



            /// <summary>
            /// Add an integer to the buffer.
            /// </summary>
            /// <param name="value">The integer value to be added.  Can be null.</param>
            public void addInt( int? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 4 );  // int is four bytes long

                // Handle nulls
                if ( value == null )
                {
                    // Add four zero bytes for the null value
                    this.add( ( byte ) 0 );  // 1st 0
                    this.add( ( byte ) 0 );  // 2nd 0
                    this.add( ( byte ) 0 );  // 3rd 0
                    this.add( ( byte ) 0 );  // 4th 0
                    return;
                }

                // Put the integer into the array, but first convert to bytes
                byte[] int_bytes = BitConverter.GetBytes( (int)value );

                // Add the four bytes
                foreach ( byte b in int_bytes )
                    this.add( b );
            }  // end addInt


            /// <summary>
            /// Add an 8-bit integer to the buffer.
            /// </summary>
            /// <param name="value">The integer value to be added.  Can be null.</param>
            public void addInt8( int? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 1 );  // int8 is one byte long

                // Handle nulls
                if ( value == null )
                {
                    // Add one zero byte for the null value
                    this.add( ( byte ) 0 ); 
                    return;
                }

                // Put the integer into the array, but first convert to byte
                this.add( (byte)value );
            }  // end addInt8


            /// <summary>
            /// Add a short (two bytes) to the buffer.
            /// </summary>
            /// <param name="value">The short value to be added.  Can be null.</param>
            public void addInt16( int? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 2 );  // int16 is two bytes long

                // Handle nulls
                if ( value == null )
                {
                    // Add two zero bytes for the null value
                    this.add( ( byte ) 0 );  // 1st 0
                    this.add( ( byte ) 0 );  // 2nd 0
                    return;
                }

                // Put the short into the array, but first convert to bytes
                byte[] short_bytes = BitConverter.GetBytes( (short)value );

                // Add the two bytes
                foreach ( byte b in short_bytes )
                    this.add( b );
            }  // end addInt16



            /// <summary>
            /// Add a long to the buffer.
            /// </summary>
            /// <param name="value">The long value to be added.  Can be null.</param>
            public void addLong( long? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 8 );  // int is eight bytes long

                // Handle nulls
                if ( value == null )
                {
                    // Add four zero bytes for the null value
                    this.add( ( byte ) 0 );  // 1st 0
                    this.add( ( byte ) 0 );  // 2nd 0
                    this.add( ( byte ) 0 );  // 3rd 0
                    this.add( ( byte ) 0 );  // 4th 0
                    this.add( ( byte ) 0 );  // 5th 0
                    this.add( ( byte ) 0 );  // 6th 0
                    this.add( ( byte ) 0 );  // 7th 0
                    this.add( ( byte ) 0 );  // 8th 0
                    return;
                }

                // Put the long into the array, but first convert to bytes
                byte[] long_bytes = BitConverter.GetBytes( (long)value );

                // Add the eight bytes
                foreach ( byte b in long_bytes )
                    this.add( b );
            }  // end addLong


            /// <summary>
            /// Add a float to the buffer.
            /// </summary>
            /// <param name="value">The float value to be added.  Can be null.</param>
            public void addFloat( float? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 4 );  // int is four bytes long

                // Handle nulls
                if ( value == null )
                {
                    // Add four zero bytes for the null value
                    this.add( ( byte ) 0.0f );  // 1st 0
                    this.add( ( byte ) 0.0f );  // 2nd 0
                    this.add( ( byte ) 0.0f );  // 3rd 0
                    this.add( ( byte ) 0.0f );  // 4th 0
                    return;
                }

                // Put the integer into the array, but first convert to bytes
                byte[] float_bytes = BitConverter.GetBytes( (float)value );

                // Add the four bytes
                foreach ( byte b in float_bytes )
                    this.add( b );
            }  // end addFloat



            /// <summary>
            /// Add a double to the buffer.
            /// </summary>
            /// <param name="value">The double value to be added.  Can be null.</param>
            public void addDouble( double? value )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( 8 );  // int is eight bytes long

                // Handle nulls
                if ( value == null )
                {
                    // Add four zero bytes for the null value
                    this.add( ( byte ) 0.0 );  // 1st 0
                    this.add( ( byte ) 0.0 );  // 2nd 0
                    this.add( ( byte ) 0.0 );  // 3rd 0
                    this.add( ( byte ) 0.0 );  // 4th 0
                    this.add( ( byte ) 0.0 );  // 5th 0
                    this.add( ( byte ) 0.0 );  // 6th 0
                    this.add( ( byte ) 0.0 );  // 7th 0
                    this.add( ( byte ) 0.0 );  // 8th 0
                    return;
                }

                // Put the integer into the array, but first convert to bytes
                byte[] double_bytes = BitConverter.GetBytes( (double)value );

                // Add the eight bytes
                foreach ( byte b in double_bytes )
                    this.add( b );
            }  // end addDouble



            /// <summary>
            /// Add a string to the buffer.  Hash the string value and add it
            /// as a long internally.
            /// </summary>
            /// <param name="value">The string value to be added.  Can be null.</param>
            public void addString( string value )
            {
                // Handle nulls
                if ( value == null )
                {
                    this.addLong( 0L );
                    return;
                }

                // Hash the value
                MurMurHash3.LongPair murmur = new MurMurHash3.LongPair();
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                byte[] input = encoding.GetBytes( value );
                MurMurHash3.murmurhash3_x64_128( input, 0, (uint)input.Length, 10, out murmur );

                // Add the hashed value to the buffer
                this.addLong( murmur.val1 );
            }  // end addString



            /// <summary>
            /// Appends a charN value to the buffer.  If <paramref name="value"/> is longer than
            /// N in length, it gets truncated.  If it is shorter, then the remaining characters
            /// get padded with byte 0.
            /// </summary>
            /// <param name="value">The value to be added to the byte buffer.</param>
            /// <param name="N">The number of characters to be added to the byte buffer.  If N
            /// is greater than what would fit in the buffer, then an exception is thrown.</param>
            public void addCharN( string value, int N )
            {
                // Check if the given number of characters will fit in the buffer
                this.willBufferOverflow( N );
                //// Check if the given number of characters will fit in the buffer
                //if ( ( this.current_size + N ) > buffer_size )
                //    throw new KineticaException( $"The given {N} character(s) will not fit in the buffer (of size {buffer_size}) which has {this.current_size} bytes in it already." );

                // Handle nulls
                if ( value == null )
                {
                    for ( int i = 0; i < N; ++i )
                    {
                        this.add( (byte) 0 );
                    }
                    return;
                }

                // Encode the string into bytes (using the UTF-8 encoding)
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes( value );
                int byte_count = bytes.GetLength( 0 );

                // Truncate longer strings to the given length
                if ( byte_count > N )
                    byte_count = N;

                // Put the characters in the byte buffer in the little endian
                // order (which means it will be right to left)
                // ----------------------------------------------------------
                // First, pad with any zeroes "at the end"
                for ( int i = N; i > byte_count; --i )
                {
                    this.add( (byte) 0 );
                }

                // Then, put all the characters (in reverse order)
                for ( int i = ( byte_count - 1 ); i >= 0; --i )
                {
                    this.add( bytes[i] );
                }
            }  // end addCharN()


            /// <summary>
            /// Adds a string to the buffer that has the 'date' property.
            /// Internally, the date is stored as an integer.
            /// </summary>
            /// <param name="value">The date string to be added.  Must have
            /// the YYYY-MM-DD format.</param>
            public void addDate( string value )
            {
                // Check and throw if the buffer is already full
                this.isBufferFull( true );

                // Handle nulls
                if ( value == null )
                {
                    this.addInt( 0 );
                    return;
                }

                // Check that the given value matches the YYYY-MM-DD pattern
                Match match = DATE_REGEX.Match( value );
                if ( !match.Success )
                {
                    // No match, so the key is invalid
                    this.is_valid = false;
                    this.addInt( 0 );
                    return;
                }

                // We'll need to parse the string into year, month, and day
                int year, month, day;
                DateTime date;
                System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();

                // Parse the string value
                try
                {
                    year  = int.Parse( match.Groups[ 1 ].ToString() );
                    month = int.Parse( match.Groups[ 2 ].ToString() );
                    day   = int.Parse( match.Groups[ 3 ].ToString() );
                    date  = new DateTime( year, month, day, calendar );
                }
                catch ( Exception ex )
                {
                    // Upon any error, set this key to be invalid
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Kinetica does not support years outside the range [1000, 2900]
                if ( ( year < MIN_SUPPORTED_YEAR ) || ( year > MAX_SUPPORTED_YEAR ) )
                {
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Deduce the integer representing the date
                int date_integer = ( ((year - MIN_SUPPORTED_YEAR) << 21)
                                     | (month << 17)
                                     | (day << 12)
                                     | (calendar.GetDayOfYear( date ) << 3)
                                     | (int)calendar.GetDayOfWeek( date ) );
                this.addInt( date_integer );
            }  // end addDate()


            /// <summary>
            /// Adds a string to the buffer that has the 'decimal' property.
            /// Internally, the date is stored as a long.
            /// </summary>
            /// <param name="value">The date string to be added.  Must have
            /// upto 19 digits of precision and four digits of scale format.</param>
            public void addDecimal( string value )
            {
                // Check and throw if the buffer is already full
                this.isBufferFull( true );

                // Handle nulls
                if ( value == null )
                {
                    this.addLong( 0L );
                    return;
                }

                // Check that the given value matches the decimal regular expression pattern
                Match match = DECIMAL_REGEX.Match( value );
                if ( !match.Success )
                {
                    // No match, so the key is invalid
                    this.is_valid = false;
                    this.addLong( 0L );
                    return;
                }

                // Parse the string value
                long decimal_value;
                try
                {
                    // Extract the integral and fractional parts
                    string integral_part_str   = match.Groups[ "int"  ].Value;
                    string fractional_part_str = match.Groups[ "frac" ].Value;
                    long integral_part;
                    long fractional_part;
                    bool has_integral_part   = long.TryParse( integral_part_str,   out integral_part   );
                    bool has_fractional_part = long.TryParse( fractional_part_str, out fractional_part );
                    // Shift the integral part to the left, if there is any fractional part
                    int fractional_part_len = fractional_part_str.Length;
                    integral_part = integral_part * (long)Math.Pow(10, fractional_part_len );
                    // Put the two parts together to create a long form of the decimal value
                    decimal_value = integral_part + fractional_part;
                }
                catch ( Exception ex )
                {
                    // Upon any error, set this key to be invalid
                    this.addLong( 0L );
                    this.is_valid = false;
                    return;
                }

                // Deduce the integer representing the date
                this.addLong( decimal_value );
            }  // end addDecimal()


            /// <summary>
            /// Adds a string to the buffer that has the 'ipv4' property.
            /// Internally, the IP is stored as an integer.
            /// </summary>
            /// <param name="value">The IPv4 string to be added.  Must have
            /// the XXX.XXX.XXX.XXX format.</param>
            public void addIPv4( string value )
            {
                // Check and throw if the buffer is already full
                this.isBufferFull( true );

                // Handle nulls
                if ( value == null )
                {
                    this.addInt( 0 );
                    return;
                }

                // Check that the given value matches the XXX.XXX.XXX.XXX pattern
                Match match = IPV4_REGEX.Match( value );
                if ( !match.Success )
                {
                    // No match, so the key is invalid
                    this.is_valid = false;
                    this.addInt( 0 );
                    return;
                }

                // We'll need to parse the string into four integers
                int a, b, c, d;

                // Parse the string value
                try
                {
                    a = int.Parse( match.Groups[ "a" ].Value );
                    b = int.Parse( match.Groups[ "b" ].Value );
                    c = int.Parse( match.Groups[ "c" ].Value );
                    d = int.Parse( match.Groups[ "d" ].Value );
                }
                catch ( Exception ex )
                {
                    // Upon any error, set this key to be invalid
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Each byte has to be within the range [0, 255] (the regex does
                // not support negative numbers, so no worries about those)
                if ( ( a > 255 ) || ( b > 255 ) || ( c > 255 ) || ( d > 255 ) )
                {
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Deduce the integer representing the date
                int ipv4_integer = ( (a << 24) | (b << 16) | (c <<  8) |  d );
                this.addInt( ipv4_integer );
            }  // end addIPv4()


            /// <summary>
            /// Adds a string to the buffer that has the 'time' property.
            /// Internally, the time is stored as an integer.
            /// </summary>
            /// <param name="value">The time string to be added.  Must have
            /// the HH:MM:SS[.mmm] format.</param>
            public void addTime( string value )
            {
                // Check and throw if the buffer is already full
                this.isBufferFull( true );

                // Handle nulls
                if ( value == null )
                {
                    this.addInt( 0 );
                    return;
                }

                // Check that the given value matches the HH:MM:SS[.mmm] pattern
                Match match = TIME_REGEX.Match( value );
                if ( !match.Success )
                {
                    // No match, so the key is invalid
                    this.is_valid = false;
                    this.addInt( 0 );
                    return;
                }

                // We'll need to parse the string into four integers
                uint hour, minute, second, milliseconds;

                // Parse the string value
                try
                {
                    hour   = uint.Parse( match.Groups["hour"].Value );
                    minute = uint.Parse( match.Groups["minute"].Value );
                    second = uint.Parse( match.Groups["seconds"].Value );
                    Group msec_group = match.Groups["milliseconds"];
                    if ( msec_group.Success )
                        milliseconds = uint.Parse( msec_group.Value );
                    else
                        milliseconds = 0;
                }
                catch ( Exception ex )
                {
                    // Upon any error, set this key to be invalid
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Validate the hour, minute, second values
                if ( ( hour > 23 ) || ( minute > 59 ) || ( second > 59 ) )
                {
                    this.addInt( 0 );
                    this.is_valid = false;
                    return;
                }

                // Deduce the integer representing the time
                int time_integer = (int)( (hour << 26) | (minute << 20) | (second << 14) | (milliseconds << 4) );
                this.addInt( time_integer );
            }  // end addTime()


            /// <summary>
            /// Adds a long to the buffer that has the 'timestamp' property.
            /// </summary>
            /// <param name="value">The timestamp to be added (in milliseconds).</param>
            public void addTimeStamp( long? value )
            {
                // Handle nulls
                if ( value == null )
                {
                    this.addInt( 0 );
                    return;
                }

                // 
                System.Globalization.GregorianCalendar calendar = new System.Globalization.GregorianCalendar();
                DateTime time = MIN_DATE.AddMilliseconds( (double) value );
                long timestamp = (long) ( ((time.Year - 1900) << 53)
                                          | ((time.Month + 1) << 49)
                                          | (time.Day << 44)
                                          | (time.Hour << 39)
                                          | (time.Minute << 33)
                                          | (time.Second << 27)
                                          | (time.Millisecond << 17)
                                          | (time.DayOfYear << 8)
                                          | ((int)time.DayOfWeek << 5) );
                this.addLong( timestamp );
            }  // end addTimeStamp()



            /// <summary>
            /// Compute the hash of the key in the buffer.  Use the Murmurhash3
            /// algorithm to compute the hash.  If not all of the values have been
            /// added to the key (i.e. if the buffer is not full), then throw an
            /// exception.
            /// </summary>
            public void computHashes()
            {
                // Check all the values for the key have been added
                if ( this.current_size != this.buffer_size )
                    throw new KineticaException( "The RecordKey buffer is not full; check that all the relevant values have been added." );

                // Hash the value
                MurMurHash3.LongPair murmur = new MurMurHash3.LongPair();
                MurMurHash3.murmurhash3_x64_128( this.buffer, 0, ( uint ) this.buffer_size, 10, out murmur );

                // Save the hash value
                this.routingHash = murmur.val1;
                this.hash_code = ( int ) ( this.routingHash ^ ((this.routingHash >> 32) & 0x0000ffffL));
            }  // end computHashes



            /// <summary>
            /// Given a routing table consisting of worker rank indices, choose a
            /// worker rank based on the hash of the record key.
            /// </summary>
            /// <param name="routingTable">A list of integers which represent worker ranks.</param>
            /// <returns>The appropriate entry from<paramref name="routingTable"/>.</returns>
            public int route( IList<int> routingTable )
            {
                // Return 1 less than the value of the nth element of routingTable where
                //    n == (record key hash) % (number of elements in routingTable)
                // (because the 1st worker rank is the 0th element in the worker list)
                return (routingTable[ Math.Abs( ( int ) ( this.routingHash % routingTable.Count ) ) ] - 1);
            }  // end route

        }  // end class RecordKey


        /// <summary>
        /// Builds or creates <see cref="RecordKey"/> objects based on a given record.
        /// </summary>
        /// <typeparam name="T">The type of record to build keys off of.</typeparam>
        private sealed class RecordKeyBuilder<T>
        {
            /// <summary>
            /// Enumerate all the data types supported by Kinetica.
            /// </summary>
            private enum ColumnType
            {
                CHAR1,
                CHAR2,
                CHAR4,
                CHAR8,
                CHAR16,
                CHAR32,
                CHAR64,
                CHAR128,
                CHAR256,
                DATE,
                DECIMAL,
                DOUBLE,
                FLOAT,
                INT,
                INT8,
                INT16,
                IPV4,
                LONG,
                STRING,
                TIME,
                TIMESTAMP
            }  // end enum ColumnType


            // Class members
            private KineticaType ktype;
            private IList<int> routing_column_indices;
            private IList<ColumnType> column_types;
            private int buffer_size;

            public RecordKeyBuilder( bool is_primary_key, KineticaType ktype )
            {
                this.ktype = ktype;

                this.buffer_size = 0;
                routing_column_indices = new List<int>();
                column_types = new List<ColumnType>();

                // We need to check if the type has all of the following: x, y, timestamp, track ID
                // (this will tell us if it's a track type table, and if so, the track ID
                // column would be a routing column)
                bool has_timestamp = false;
                bool has_x = false;
                bool has_y = false;
                int track_id_column_idx = -1;  // not found yet

                // Add indices of any primary or shard key (based on is_primary_key)
                // to the list of routing columns
                IList<KineticaType.Column> columns = ktype.getColumns();
                for ( int i = 0; i < columns.Count; ++i )
                {
                    // Get the column
                    KineticaType.Column column = columns[ i ];

                    // Check if it is one of: x, y, timestamp, track ID
                    switch ( column.getName() )
                    {
                        case "TRACKID":
                            track_id_column_idx = i;
                            break;

                        case "TIMESTAMP":
                            has_timestamp = true;
                            break;

                        case "x":
                            has_x = true;
                            break;

                        case "y":
                            has_y = true;
                            break;
                    }  // end switch on column name

                    // Check if this column has been declared as a primary/shard key
                    // And if so, and if appropriate, add it to the routing key column list
                    if ( is_primary_key && column.getProperties().Contains( ColumnProperty.PRIMARY_KEY ) )
                    {
                        routing_column_indices.Add( i );
                    }
                    else if ( !is_primary_key && column.getProperties().Contains( ColumnProperty.SHARD_KEY ) )
                    {
                        routing_column_indices.Add( i );
                    }
                }  // end for loop

                // Check if this is a track-type table; if so, add the track ID column's index to the list
                if ( !is_primary_key
                    && has_timestamp && has_x && has_y && ( track_id_column_idx != -1 ) )
                {
                    if ( routing_column_indices.Count == 0 )
                    {
                        routing_column_indices.Add( track_id_column_idx );
                    }
                    else if ( ( routing_column_indices.Count != 1 )
                              || ( routing_column_indices[0] != track_id_column_idx ) )
                    {
                        // Track type tables can't have any other routing key
                        throw new KineticaException( "Cannot have a shard key other than 'TRACKID' for track tables." );
                    }
                }  // end if a track type table


                // For each index of routing columns, save the column type, and increase
                // the buffer size appropriately
                foreach ( int i in routing_column_indices )
                {
                    // Get the column information
                    KineticaType.Column column = columns[ i ];

                    switch ( column.getType() )
                    {
                        // Float and double are the simplest
                        case KineticaType.Column.ColumnType.FLOAT:
                        {
                            column_types.Add( ColumnType.FLOAT );
                            this.buffer_size += 4;
                            break;
                        }
                        case KineticaType.Column.ColumnType.DOUBLE:
                        {
                            column_types.Add( ColumnType.DOUBLE );
                            this.buffer_size += 8;
                            break;
                        }

                        case KineticaType.Column.ColumnType.INT:
                        {
                            // Integer has byte, short and int
                            if ( column.getProperties().Contains( ColumnProperty.INT8 ) )
                            {   // byte
                                column_types.Add( ColumnType.INT8 );
                                this.buffer_size += 1;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.INT16 ) )
                            {   // short
                                column_types.Add( ColumnType.INT16 );
                                this.buffer_size += 2;
                            }
                            else // regular 4-byte integer
                            {
                                column_types.Add( ColumnType.INT );
                                this.buffer_size += 4;
                            }
                            break;
                        }  // end case integer

                        case KineticaType.Column.ColumnType.LONG:
                        {
                            // Long has the regular long and timestamp
                            if ( column.getProperties().Contains( ColumnProperty.TIMESTAMP ) )
                            {   // it's a timestamp
                                column_types.Add( ColumnType.TIMESTAMP );
                            }
                            else // regular long
                            {
                                column_types.Add( ColumnType.LONG );
                            }
                            this.buffer_size += 8;
                            break;
                        }  // end case long

                        case KineticaType.Column.ColumnType.STRING:
                        {
                            if ( column.getProperties().Contains( ColumnProperty.CHAR1 ) )
                            {
                                column_types.Add( ColumnType.CHAR1 );
                                this.buffer_size += 1;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR2 ) )
                            {
                                column_types.Add( ColumnType.CHAR2 );
                                this.buffer_size += 2;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR4 ) )
                            {
                                column_types.Add( ColumnType.CHAR4 );
                                this.buffer_size += 4;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR8 ) )
                            {
                                column_types.Add( ColumnType.CHAR8 );
                                this.buffer_size += 8;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR16 ) )
                            {
                                column_types.Add( ColumnType.CHAR16 );
                                this.buffer_size += 16;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR32 ) )
                            {
                                column_types.Add( ColumnType.CHAR32 );
                                this.buffer_size += 32;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR64 ) )
                            {
                                column_types.Add( ColumnType.CHAR64 );
                                this.buffer_size += 64;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR128 ) )
                            {
                                column_types.Add( ColumnType.CHAR128 );
                                this.buffer_size += 128;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.CHAR256 ) )
                            {
                                column_types.Add( ColumnType.CHAR256 );
                                this.buffer_size += 256;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.DATE ) )
                            {
                                column_types.Add( ColumnType.DATE );
                                this.buffer_size += 4;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.DECIMAL ) )
                            {
                                column_types.Add( ColumnType.DECIMAL );
                                this.buffer_size += 8;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.IPV4 ) )
                            {
                                column_types.Add( ColumnType.IPV4 );
                                this.buffer_size += 4;
                            }
                            else if ( column.getProperties().Contains( ColumnProperty.TIME ) )
                            {
                                column_types.Add( ColumnType.TIME );
                                this.buffer_size += 4;
                            }
                            else // regular string
                            {
                                column_types.Add( ColumnType.STRING );
                                this.buffer_size += 8;
                            }
                            break;
                        }  // end case string

                        // Other types are not allowed for routing columns
                        case KineticaType.Column.ColumnType.BYTES:
                        case KineticaType.Column.ColumnType.DEFAULT:
                            throw new KineticaException( $"Cannot use column '{column.getName()}' as a key."  );
                    }  // end switch on the column's primitive data type
                }  // end foreach
            }  // end constructor RecordKeyBuilder


            /// <summary>
            /// Build a RecordKey object based on a record.
            /// </summary>
            /// <param name="record">The object based on which the key is to
            /// be built.</param>
            /// <returns>The record key that helps is routing this record
            /// correctly.</returns>
            public RecordKey build( T record )
            {
                // Can't build a key if the buffer size is zero!
                if ( this.buffer_size == 0 )
                    return null;

                // Create the empty key
                RecordKey key = new RecordKey( this.buffer_size );

                // Add each routing column's value to the key
                for ( int i = 0; i < this.routing_column_indices.Count; ++i )
                {
                    // Get the column (with type and name)
                    KineticaType.Column column = this.ktype.getColumns()[ this.routing_column_indices[ i ] ];

                    // Get the value out of the record using the column's name and reflection
                    var value = record.GetType().GetProperty( column.getName() ).GetValue( record, null );

                    switch ( this.column_types[i] )
                    {
                        case ColumnType.CHAR1:
                            key.addCharN( (string) value, 1 );
                            break;

                        case ColumnType.CHAR2:
                            key.addCharN( ( string ) value, 2 );
                            break;

                        case ColumnType.CHAR4:
                            key.addCharN( ( string ) value, 4 );
                            break;

                        case ColumnType.CHAR8:
                            key.addCharN( ( string ) value, 8 );
                            break;

                        case ColumnType.CHAR16:
                            key.addCharN( ( string ) value, 16 );
                            break;

                        case ColumnType.CHAR32:
                            key.addCharN( ( string ) value, 32 );
                            break;

                        case ColumnType.CHAR64:
                            key.addCharN( ( string ) value, 64 );
                            break;

                        case ColumnType.CHAR128:
                            key.addCharN( ( string ) value, 128 );
                            break;

                        case ColumnType.CHAR256:
                            key.addCharN( ( string ) value, 256 );
                            break;

                        case ColumnType.DATE:
                            key.addDate( (string) value );
                            break;

                        case ColumnType.DECIMAL:
                            key.addDecimal( (string) value );
                            break;

                        case ColumnType.DOUBLE:
                            key.addDouble( ( double? ) value );
                            break;

                        case ColumnType.FLOAT:
                            key.addFloat( ( float? ) value );
                            break;

                        case ColumnType.INT:
                            key.addInt( ( int? ) value );
                            break;

                        case ColumnType.INT8:
                            key.addInt8( ( int? ) value );
                            break;

                        case ColumnType.INT16:
                            key.addInt16( ( int? ) value );
                            break;

                        case ColumnType.IPV4:
                            key.addIPv4( ( string ) value );
                            break;

                        case ColumnType.LONG:
                            key.addLong( ( long? ) value );
                            break;

                        case ColumnType.STRING:
                            key.addString( ( string ) value );
                            break;

                        case ColumnType.TIME:
                            key.addTime( ( string ) value );
                            break;

                        case ColumnType.TIMESTAMP:
                            key.addTimeStamp( ( long? ) value );
                            break;
                    }  // end switch
                }  // end for loop

                // Compute the hash for the key and return it
                key.computHashes();
                return key;
            }  // end build()



            /// <summary>
            /// Returns whether this builder builds any routing keys. That is,
            /// if there are any routing columns in the relevant record type.
            /// </summary>
            /// <returns>Bool value indicating if this builder builds keys.</returns>
            public bool hasKey()
            {
                // Does it have any routing columns?
                return !(this.routing_column_indices.Count == 0);
            }


            /// <summary>
            /// Returns if <paramref name="other"/> is equivalent to this builder.
            /// </summary>
            /// <param name="other">The other key builder.</param>
            /// <returns>Whether the other builder is the same as this one.</returns>
            public bool hasSameKey( RecordKeyBuilder<T> other)
            {
                return this.column_types.Equals( other.column_types );
            }

        }  // end class RecordKeyBuilder



        private sealed class WorkerQueue<T>
        {
            public System.Uri url { get; private set; }
            private int capacity;
            private bool has_primary_key;
            private bool update_on_existing_pk;
            private IList<T> queue;
            private Dictionary<RecordKey, int> primary_key_map;


            /// <summary>
            /// Creates an insertion queue for a given worker.
            /// </summary>
            /// <param name="url"></param>
            /// <param name="capacity"></param>
            /// <param name="has_primary_key"></param>
            /// <param name="update_on_existing_pk"></param>
            public WorkerQueue( System.Uri url, int capacity, bool has_primary_key, bool update_on_existing_pk )
            {
                this.url      = url;
                this.capacity = capacity;
                this.has_primary_key       = has_primary_key;
                this.update_on_existing_pk = update_on_existing_pk;

                queue = new List<T>();

                // If the type has primary keys, then initialize with a
                // capacity of 75% of the final capacity
                if ( this.has_primary_key )
                    primary_key_map = new Dictionary<RecordKey, int>( (int)Math.Round( this.capacity/0.75 ) );
            }  // end constructor WorkerQueue<T>



            /// <summary>
            /// Returns the current queue and creates a new empty one.
            /// </summary>
            /// <returns>A list of records to be inserted.</returns>
            public IList<T> flush()
            {
                IList<T> old_queue = this.queue;
                queue = new List<T>( this.capacity );

                // Clear the primary key map if one exists
                if ( this.primary_key_map != null )
                    this.primary_key_map.Clear();

                return old_queue;
            }  // end flush



            /// <summary>
            /// Inserts a record into the queue (if all conditions are
            /// favourable).  Returns the queue if it becomes full upon insertion.
            /// </summary>
            /// <param name="record">The record to insert into the queue.</param>
            /// <param name="key">A primary key, if any.</param>
            /// <returns>The list of records (if the queue is full), or null.</returns>
            public IList<T> insert( T record, RecordKey key )
            {
                if ( this.has_primary_key && key.isValid() )
                {
                    // We are to update the record even if the primary key already exists
                    if ( this.update_on_existing_pk )
                    {
                        int key_idx;

                        if ( this.primary_key_map.TryGetValue( key, out key_idx ) )
                        {
                            // Key exists, so we need to replace the associated record
                            this.queue[key_idx] = record;
                        }
                        else  // key does not exist; add the record and
                        {     // update the key->record mapping
                            this.queue.Add( record );
                            this.primary_key_map.Add( key, ( this.queue.Count - 1 ) );
                        }
                    }
                    else // do NOT update/add the record if the key already exists
                    {
                        if ( this.primary_key_map.ContainsKey( key ) )
                            return null;  // yup, the key already exists

                        // The key does not exist, so add the record and
                        // update the key->record map
                        this.queue.Add( record );
                        this.primary_key_map.Add( key, ( this.queue.Count - 1 ) );
                    }
                }
                else  // simply add the record
                {
                    queue.Add( record );
                }

                // If the queue is full, then flush and return the 'old' queue
                if ( queue.Count == capacity )
                    return flush();
                else // no records to return
                    return null;
            }  // end insert
        }  // end class WorkerQueue




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
        private RecordKeyBuilder<T> primary_key_builder;
        private RecordKeyBuilder<T> shard_key_builder;
        private IList<int> routing_table;
        private IList<WorkerQueue<T>> worker_queues;
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
                                 WorkerList workers = null )
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
            this.primary_key_builder = new RecordKeyBuilder<T>( true,  this.ktype );
            this.shard_key_builder   = new RecordKeyBuilder<T>( false, this.ktype );

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
            this.worker_queues = new List<WorkerQueue<T>>();
            try
            {
                // If no workers are given, try to get them from Kinetica
                if ( ( workers == null ) || ( workers.Count == 0 ) )
                {
                    workers = new WorkerList( kdb );
                }

                // If we end up with multiple workers, either given by the
                // user or obtained from Kinetica, then use those
                if ( ( workers != null ) && ( workers.Count > 0 ) )
                {
                    // Add worker queues per worker
                    foreach ( System.Uri worker_url in workers )
                    {
                        WorkerQueue<T> worker_queue = new WorkerQueue<T>( worker_url, batch_size, has_primary_key, update_on_existing_pk );
                        this.worker_queues.Add( worker_queue );
                    }

                    // Get the worker rank information from Kinetica
                    this.routing_table = kdb.adminGetShardAssignments().shard_assignments_rank;
                    // Check that enough worker URLs are specified
                    for ( int i = 0; i < routing_table.Count; ++i )
                    {
                        if ( this.routing_table[i] > this.worker_queues.Count )
                            throw new KineticaException( "Not enough worker URLs specified." );
                    }
                }
                else // multihead-ingest is NOT turned on; use the regular Kinetica IP address
                {
                    System.Uri url = new System.Uri( kdb.Url + "/insert/records" );
                    WorkerQueue<T> worker_queue = new WorkerQueue<T>( url, batch_size, has_primary_key, update_on_existing_pk );
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
            foreach ( WorkerQueue<T> worker_queue in this.worker_queues )
            {
                // Flush the the queue
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
                throw new InsertException<T>( url, queue, ex.ToString() );
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
            RecordKey primary_key = null;  // used to check for uniqueness
            RecordKey shard_key = null;    // used to find which worker to send this record to

            // Build the primary key, if any
            if ( this.primary_key_builder != null )
                primary_key = this.primary_key_builder.build( record );

            // Build the shard/routing key, if any
            if ( this.shard_key_builder != null )
                shard_key = this.shard_key_builder.build( record );

            // Find out which worker to send the record to; then add the record
            // to the approrpriate worker's record queue
            WorkerQueue<T> worker_queue;
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



        ///// <summary>
        ///// Used only for debugging purposes; delete it after done debugging
        ///// </summary>
        //public KineticaIngestor()
        //{
        //    // debug~~~~~~~~~~~ doing TESTING below
        //    //RecordKey key1 = new RecordKey( -6 );  // gets caught, as expected
        //    //RecordKey key2 = new RecordKey( 0 ); // gets caught, as expected
        //    RecordKey key3 = new RecordKey( 1 );
        //    RecordKey key4 = new RecordKey( 2000 );
        //    RecordKey key5 = new RecordKey( 20 );

        //    //// CharN
        //    //// =====
        //    //// Try nulls
        //    //key4.addCharN( null, 1 );
        //    //key3.addCharN( null, 1 );

        //    //// Try small chars
        //    //key3.addCharN( "ss", 2 );
        //    //key4.addCharN( "a", 1 );
        //    //key4.addCharN( "ss", 2 );
        //    //key4.addCharN( "ddd", 3 );
        //    //key4.addCharN( "ffff", 4 );
        //    //key4.addCharN( "12345", 8 );
        //    ////key4.addCharN( "s", 1 );

        //    //Console.WriteLine( "Adding char10..." );
        //    //key5.addCharN( "12345678910", 10 );
        //    //Console.WriteLine( "Adding char8..." );
        //    //key5.addCharN( "12345678", 8 );
        //    //Console.WriteLine( "Adding char6..." );
        //    //key5.addCharN( "12345678", 6 );
        //    //Console.WriteLine( "Adding an integer..." );

        //    //// int
        //    //// ====
        //    //key5.addInt( 5 );
        //    //Console.WriteLine( "Adding a null integer..." );
        //    //key5.addInt( null );
        //    //Console.WriteLine( "Adding a null integer; but buffer will overflow..." );
        //    //key5.addInt( null );

        //    //// Int8
        //    //// ====
        //    //Console.WriteLine( "Adding a null int8..." );
        //    //key5.addInt8( null );
        //    //Console.WriteLine( "Adding a regular int8..." );
        //    //key5.addInt8( 8 );
        //    //Console.WriteLine( "Adding a regular int8; but buffer will overflow..." );
        //    //key5.addInt8( 9 );

        //    //// Int16
        //    //// =====
        //    //Console.WriteLine( "Adding a regular int16..." );
        //    //key5.addInt16( 16 );
        //    //Console.WriteLine( "Adding a null int16..." );
        //    //key5.addInt16( null );
        //    //Console.WriteLine( "Adding a regular int16; but buffer will overflow..." );
        //    //key5.addInt16( 9 );

        //    //// Long
        //    //// ====
        //    //Console.WriteLine( "Adding a regular long..." );
        //    //key5.addLong( 8889999 );
        //    //Console.WriteLine( "Adding a null long..." );
        //    //key5.addLong( null );
        //    //Console.WriteLine( "Adding a regular long; but buffer will overflow..." );
        //    //key5.addLong( 2893074 );

        //    //// Float
        //    //// =====
        //    //Console.WriteLine( "Adding a regular float..." );
        //    //key5.addFloat( 111.22222f );
        //    //Console.WriteLine( "Adding a null float..." );
        //    //key5.addFloat( null );
        //    //Console.WriteLine( "Adding a regular float; but buffer will overflow..." );
        //    //key5.addFloat( 28930.74f );

        //    //// Double
        //    //// ======
        //    //Console.WriteLine( "Adding a regular double..." );
        //    //key5.addDouble( 111.22222 );
        //    //Console.WriteLine( "Adding a null double..." );
        //    //key5.addDouble( null );
        //    //Console.WriteLine( "Adding a regular double; but buffer will overflow..." );
        //    //key5.addDouble( 28930.74 );

        //    //// Date
        //    //// ====
        //    ////Console.WriteLine( "Adding a mangled date..." );
        //    ////key5.addDate( "lskdjfl" );
        //    ////Console.WriteLine( "is the key valid?: " + key5.isValid() );
        //    //Console.WriteLine( "Adding an ok date..." );
        //    //key5.addDate( "1980-05-01" );
        //    //Console.WriteLine( "is the key valid?: " + key5.isValid() );
        //    //Console.WriteLine( "Adding a null date..." );
        //    //key5.addDate( null );
        //    //Console.WriteLine( "is the key valid?: " + key5.isValid() );
        //    ////Console.WriteLine( "Adding a mangled date..." );
        //    ////key5.addDate( "" );
        //    //////key5.addDate( "lkds-928-lknkkk" );
        //    ////Console.WriteLine( "is the key valid?: " + key5.isValid() );
        //    //Console.WriteLine( "Adding an unviable date..." );
        //    //key5.addDate( "1934-02-32" );
        //    //Console.WriteLine( "is the key valid?: " + key5.isValid() );

        //    //// Decimals
        //    //// ========
        //    //string val = " +12345.";
        //    //Console.WriteLine( "Adding a valid decimal value: " + val );
        //    //key4.addDecimal( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = " -.1234";
        //    //Console.WriteLine( "Adding a valid decimal value: " + val );
        //    //key4.addDecimal( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "1234567890.1234";
        //    //Console.WriteLine( "Adding a valid decimal value: " + val );
        //    //key4.addDecimal( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = " 1234.1234 ";
        //    //Console.WriteLine( "Adding a valid decimal value: " + val );
        //    //key4.addDecimal( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //Console.WriteLine( "Adding a null decimal value" );
        //    //key4.addDecimal( null );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = " 1234.1234 ";
        //    //Console.WriteLine( "Adding a valid decimal value: " + val );
        //    //key4.addDecimal( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    ////Console.WriteLine( "Adding an invalid decimal value... empty string" );
        //    ////key4.addDecimal( "" );
        //    ////Console.WriteLine( "is the key valid?: " + key4.isValid() );
        //    ////val = "12,234.99";
        //    ////Console.WriteLine( "Adding an invalid decimal value... " + val );
        //    ////key4.addDecimal( val );
        //    ////Console.WriteLine( "is the key valid?: " + key4.isValid() );


        //    //// IPv4
        //    //// ====
        //    //string ipv4 = "0.0.0.0";
        //    //Console.WriteLine( "Adding a valid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "0.255.0.244";
        //    //Console.WriteLine( "Adding a valid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.255.255.25";
        //    //Console.WriteLine( "Adding a valid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = null;
        //    //Console.WriteLine( "Adding a null IPv4 value" );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.*.255.123";
        //    //Console.WriteLine( "Adding a invalid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.255.1111.255";
        //    //Console.WriteLine( "Adding a invalid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.255.xxx.255";
        //    //Console.WriteLine( "Adding a invalid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.255.255";
        //    //Console.WriteLine( "Adding a invalid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //ipv4 = "255.255.255.-25";
        //    //Console.WriteLine( "Adding a invalid IPv4 value: " + ipv4 );
        //    //key4.addIPv4( ipv4 );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );


        //    //// Regular strings
        //    //// ===============
        //    //string val = null;
        //    //Console.WriteLine( "Adding a null string value" );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "1234";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "asdfjkl;owieurmknckjsdhf";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "-1234";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "qwertyuiop[]\\";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "zxcvbnm,./";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "slkdfj,mxcgnoeurtyw;ole,mfn9845729038-p21kernmdnf";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "abcdefghijklmnopqrstuvwxyz";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "123456789012345";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "1234567890123456";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //val = "12345678901234567";
        //    //Console.WriteLine( "Adding a valid string value: " + val + " of length " + val.Length );
        //    //key4.addString( val );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //// Time
        //    //// ====
        //    //string time = null;
        //    //Console.WriteLine( "Adding a null time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "12:34:56";
        //    //Console.WriteLine( "Adding a valid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "12:34:56.123";
        //    //Console.WriteLine( "Adding a valid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "00:00:00.000";
        //    //Console.WriteLine( "Adding a valid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "22:23:23.-12";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "22:23:70";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "23:61:23";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "24:23:23";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "12:-12:12";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "0.1.2.3";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "slkdfj";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "333:54.1234";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "3:33:54.1234";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "32:3:54.1234";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "32:34:5.1234";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //time = "32:34:56.1234";
        //    //Console.WriteLine( "Adding an invalid time value: " + time );
        //    //key4.addTime( time );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );


        //    //// TimeStamp
        //    //// =========
        //    //long? timestamp = null;
        //    //Console.WriteLine( "Adding a null timestamp value: " + timestamp );
        //    //key4.addTimeStamp( timestamp );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );

        //    //timestamp = 12345678909L;
        //    //Console.WriteLine( "Adding a valid timestamp value: " + timestamp );
        //    //key4.addTimeStamp( timestamp );
        //    //Console.WriteLine( "is the key valid?: " + key4.isValid() );


        //    //// Test RecordKeyBuilder
        //    //// =====================
        //    //IDictionary<string, IList<string>> column_properties = new Dictionary<string, IList<string>>();
        //    //// Make a string char4 (and nullable)
        //    //List<string> D_props = new List<string>();
        //    //D_props.Add( ColumnProperty.CHAR4 );
        //    //D_props.Add( ColumnProperty.NULLABLE );
        //    //column_properties.Add( "D", D_props );
        //    //// Let's try another nullable column
        //    //List<string> E_props = new List<string>();
        //    //E_props.Add( ColumnProperty.NULLABLE );
        //    //column_properties.Add( "E", E_props );
        //    //// And two primary keys (one nullable)
        //    //List<string> A_props = new List<string>();
        //    //A_props.Add( ColumnProperty.PRIMARY_KEY );
        //    //A_props.Add( ColumnProperty.INT8 );
        //    //column_properties.Add( "A", A_props );
        //    //List<string> B_props = new List<string>();
        //    //B_props.Add( ColumnProperty.PRIMARY_KEY );
        //    //column_properties.Add( "B", B_props );
        //    //// And a shard key (must be one of the primary keys, if specified--which we have)
        //    //B_props.Add( ColumnProperty.SHARD_KEY );

        //    //// Create the KineticaType object which facilitates creating types in the database
        //    //KineticaType type1 = KineticaType.fromClass( typeof( record_type_1 ), column_properties );

        //    //// This one will have the same record class, but different properties (no primary keys, for example)
        //    //IDictionary<string, IList<string>> column_properties_2 = new Dictionary<string, IList<string>>();
        //    //// Make a string char4 (and nullable)
        //    //List<string> D_props2 = new List<string>();
        //    //D_props2.Add( ColumnProperty.CHAR2 );
        //    //column_properties_2.Add( "D", D_props2 );
        //    //// And two primary keys (one nullable)
        //    //List<string> A_props2 = new List<string>();
        //    //A_props2.Add( ColumnProperty.INT8 );
        //    //column_properties_2.Add( "A", A_props2 );
        //    //List<string> B_props2 = new List<string>();
        //    //B_props2.Add( ColumnProperty.SHARD_KEY );
        //    //column_properties_2.Add( "B", B_props2 );

        //    //// Create the KineticaType object which facilitates creating types in the database
        //    //KineticaType type2 = KineticaType.fromClass( typeof( record_type_1 ), column_properties_2 );

        //    //// Create the series record type
        //    //KineticaType type_series = KineticaType.fromClass( typeof( record_type_series ) );

        //    //// Create the almost-series-but-not-really record type
        //    //KineticaType not_type_series = KineticaType.fromClass( typeof( record_type_not_series ) );

        //    //// Create record builders for each type, with primary keys or not primary keys
        //    //Console.WriteLine( $"builder 1, PK true, PK: A,B..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_1> builder1_PK   = new RecordKeyBuilder<record_type_1>( true,  type1 );  // ok
        //    //Console.WriteLine( $"builder 1, PK false, but PK: A,B..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_1> builder1_noPK = new RecordKeyBuilder<record_type_1>( false, type1 );  // ok??
        //    //Console.WriteLine( $"builder 2, PK true, but no PK..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_1> builder2_PK   = new RecordKeyBuilder<record_type_1>( true,  type2 );  // ok??
        //    //Console.WriteLine( $"builder 2, PK false, and no PK; shard: B..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_1> builder2_noPK = new RecordKeyBuilder<record_type_1>( false, type2 );  // ok
        //    //Console.WriteLine( $"builder series, PK true, but no PK..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_series> builder_s_PK      = new RecordKeyBuilder<record_type_series>( true,  type_series );  // ok??
        //    //Console.WriteLine( $"builder series, PK false..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_series> builder_s_noPK    = new RecordKeyBuilder<record_type_series>( false, type_series );  // ok
        //    //Console.WriteLine( $"builder NOT series, PK true, but no PK..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_not_series> builder_notS_PK   = new RecordKeyBuilder<record_type_not_series>( true,  not_type_series );  // ok??
        //    //Console.WriteLine( $"builder NOT series, PK false..." ); // debug~~~~~~~~~
        //    //RecordKeyBuilder<record_type_not_series> builder_notS_noPK = new RecordKeyBuilder<record_type_not_series>( false, not_type_series );  // ok

        //    //// Create some records to build keys off them
        //    //record_type_1 r1 = new record_type_1();
        //    //r1.A = 8; r1.B = 12; r1.C = "r1.C"; r1.D = "r1.D"; r1.E = 123.234f; r1.F = 123.23342; r1.TIMESTAMP = 92834729758875L;
        //    //record_type_1 r2 = new record_type_1();
        //    //r2.A = 8; r2.B = 12; r2.C = "r2.C"; r2.D = "r2.D"; r2.E = 123.234f; r2.F = 123.23342; r2.TIMESTAMP = 92834729758875L;
        //    //record_type_series s1 = new record_type_series();
        //    //s1.x = 10; s1.y = 20; s1.TRACKID = "s1.t1"; s1.TIMESTAMP = 9873459827498L;
        //    //record_type_series s2 = new record_type_series();
        //    //s2.x = 11; s2.y = 22; s2.TRACKID = "s2.t1"; s2.TIMESTAMP = 9873459827498L;
        //    //record_type_not_series n1 = new record_type_not_series();
        //    //n1.x = 33; n1.y = 44; n1.TRACKID = "n1.t1"; n1.Timestamp = 9873459827498L;
        //    //record_type_not_series n2 = new record_type_not_series();
        //    //n2.x = 333; n2.y = 444; n2.TRACKID = "n2.t1"; n2.Timestamp = 9873459827498L;

        //    //// Create the record keys based on the records made above
        //    //Console.WriteLine();
        //    //Console.WriteLine();
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#1 r1... " );
        //    //RecordKey k1 = builder1_PK.build( r1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#1 r2... " );
        //    //RecordKey k2 = builder1_PK.build( r2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#2 r1... " );
        //    //RecordKey k3 = builder1_noPK.build( r1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#2 r2... " );
        //    //RecordKey k4 = builder1_noPK.build( r2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#3 r1... " );
        //    //RecordKey k5 = builder2_PK.build( r1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#3 r2... " );
        //    //RecordKey k6 = builder2_PK.build( r2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#4 r1... " );
        //    //RecordKey k7 = builder2_noPK.build( r1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#4 r2... " );
        //    //RecordKey k8 = builder2_noPK.build( r2 );

        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#5 r1... " );
        //    ////RecordKey k9 = builder_s_PK.build( r1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#5 r2... " );
        //    ////RecordKey k9 = builder_s_PK.build( r2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#6 r1... " );
        //    ////RecordKey k9 = builder_s_noPK.build( r1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#6 r2... " );
        //    ////RecordKey k9 = builder_s_noPK.build( r2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#7 r1... " );
        //    ////RecordKey k9 = builder_notS_PK.build( r1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#7 r2... " );
        //    ////RecordKey k10 = builder_notS_PK.build( r2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#8 r1... " );
        //    ////RecordKey k9 = builder_notS_noPK.build( r1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#8 r2... " );
        //    ////RecordKey k10 = builder_notS_noPK.build( r2 );

        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#1 s1... " );
        //    ////RecordKey k9 = builder1_PK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#1 s2... " );
        //    ////RecordKey k10 = builder1_PK.build( s2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#2 s1... " );
        //    ////RecordKey k9 = builder1_noPK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#2 s2... " );
        //    ////RecordKey k10 = builder1_noPK.build( s2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#3 s1... " );
        //    ////RecordKey k9 = builder2_PK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#3 s2... " );
        //    ////RecordKey k10 = builder2_PK.build( s2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#4 s1... " );
        //    ////RecordKey k9 = builder2_noPK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#4 s2... " );
        //    ////RecordKey k10 = builder2_noPK.build( s2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#7 s1... " );
        //    ////RecordKey k9 = builder_notS_PK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#7 s2... " );
        //    ////RecordKey k10 = builder_notS_PK.build( s2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#8 s1... " );
        //    ////RecordKey k9 = builder_notS_noPK.build( s1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#8 s2... " );
        //    ////RecordKey k10 = builder_notS_noPK.build( s2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#5 s1... " );
        //    //RecordKey k9 = builder_s_PK.build( s1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#5 s2... " );
        //    //RecordKey k10 = builder_s_PK.build( s2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#6 s1... " );
        //    //RecordKey k11 = builder_s_noPK.build( s1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#6 s2... " );
        //    //RecordKey k12 = builder_s_noPK.build( s2 );

        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#1 n1... " );
        //    ////RecordKey k9 = builder1_PK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#1 n2... " );
        //    ////RecordKey k10 = builder1_PK.build( n2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#2 n1... " );
        //    ////RecordKey k9 = builder1_noPK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#2 n2... " );
        //    ////RecordKey k10 = builder1_noPK.build( n2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#3 n1... " );
        //    ////RecordKey k9 = builder2_PK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#3 n2... " );
        //    ////RecordKey k10 = builder2_PK.build( n2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#4 n1... " );
        //    ////RecordKey k9 = builder2_noPK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#4 n2... " );
        //    ////RecordKey k10 = builder2_noPK.build( n2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#5 n1... " );
        //    ////RecordKey k9 = builder_s_PK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#5 n2... " );
        //    ////RecordKey k10 = builder_s_PK.build( n2 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#6 n1... " );
        //    ////RecordKey k9 = builder_s_noPK.build( n1 );
        //    ////Console.WriteLine();
        //    ////Console.WriteLine( "b#6 n2... " );
        //    ////RecordKey k10 = builder_s_noPK.build( n2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#7 n1... " );
        //    //RecordKey k13 = builder_notS_PK.build( n1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#7 n2... " );
        //    //RecordKey k14 = builder_notS_PK.build( n2 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#8 n1... " );
        //    //RecordKey k15 = builder_notS_noPK.build( n1 );
        //    //Console.WriteLine();
        //    //Console.WriteLine( "b#8 n2... " );
        //    //RecordKey k16 = builder_notS_noPK.build( n2 );

        //    //// END test RecordKeyBuilder
        //    //// =====================

        //    // ~~~~~~~~~~~end debug~~~~~~~~~~~~~~~

        //}  // end constructor KineticaIngestor()


        //// Classes for testing purposes
        //private class record_type_series
        //{
        //    public double x { get; set; }
        //    public double y { get; set; }
        //    public string TRACKID { get; set; }
        //    public long TIMESTAMP { get; set; }

        //    public override string ToString()
        //    {
        //        return $"{{ x={x}, y={y}, TRACKID={TRACKID}, TIMESTAMP={TIMESTAMP} }}";
        //    }
        //}  // end class record_type_series

        //// Classes for testing purposes
        //private class record_type_not_series
        //{
        //    public double x { get; set; }
        //    public double y { get; set; }
        //    public string TRACKID { get; set; }
        //    public long Timestamp { get; set; }

        //    public override string ToString()
        //    {
        //        return $"{{ x={x}, y={y}, TRACKID={TRACKID}, Timestamp={Timestamp} }}";
        //    }
        //}  // end class record_type_not_series

        //private class record_type_1
        //{
        //    public int A { get; set; }
        //    public int B { get; set; }
        //    public string C { get; set; }
        //    public string D { get; set; }
        //    public float E { get; set; }
        //    public double? F { get; set; }
        //    public long TIMESTAMP { get; set; }

        //    public override string ToString()
        //    {
        //        string f;
        //        if ( F != null )
        //            f = $"{F}";
        //        else
        //            f = "<null>";
        //        return $"{{ A={A}, B={B}, C={C}, D={D}, E={E}, F={f}, TIMESTAMP={TIMESTAMP} }}";
        //    }
        //}  // end class record_type_1


    }  // end class KineticaIngestor<T>




}  // end namespace kinetica
